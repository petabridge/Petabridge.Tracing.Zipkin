// -----------------------------------------------------------------------
// <copyright file="KafkaReportingActor.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Akka.Actor;
using Akka.Event;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Phobos.Actor.Common;

namespace Petabridge.Tracing.Zipkin.Reporting.Kafka
{
    /// <summary>
    ///     INTERNAL API
    ///     Actor responsible for managing the batching of outbound <see cref="T:Petabridge.Tracing.Zipkin.Span" /> instances.
    /// </summary>
    internal sealed class KafkaReportingActor : ReceiveActor, INeverMonitor, INeverTrace
    {
        private readonly ILoggingAdapter _log = Context.GetLogger();

        private readonly ZipkinKafkaReportingOptions _options;
        private ICancelable _batchTransimissionTimer;

        private KafkaTransmitter _kafkaProducer;

        public KafkaReportingActor(ZipkinKafkaReportingOptions options)
        {
            Contract.Assert(options.Serializer != null);
            _options = options;
            PendingMessages = new List<Span>(_options.MaximumBatchSize);

            Batching();
        }

        public List<Span> PendingMessages { get; }

        public bool BatchSizeReached => PendingMessages.Count >= _options.MaximumBatchSize;

        private void Batching()
        {
            Receive<Span>(s =>
            {
                PendingMessages.Add(s);
                if (BatchSizeReached)
                    ExecuteDelivery();
            });

            Receive<DeliverBatch>(d =>
            {
                if (PendingMessages.Any())
                    ExecuteDelivery();
                else
                    RescheduleBatchTransmission();
            });

            Receive<Message<Null, byte[]>>(msg =>
            {
                if (msg.Error.HasError && _log.IsErrorEnabled)
                    _log.Error(
                        "Error [{0}][{1}] occurred while uploading spans [{3} bytes] to Kafka endpoints [{2}] for topic [{4}]",
                        msg.Error.Code, msg.Error.Reason,
                        string.Join(",", _options.BootstrapServers), msg.Value.Length, msg.Topic);
                else if (_log.IsDebugEnabled)
                    _log.Debug("Successfully posted spans [{0} bytes] to Kafka for topic [{1}]", msg.Value.Length,
                        msg.Topic);
            });

            // Indicates that one of our HTTP requests timed out
            Receive<Status.Failure>(f =>
            {
                if (_log.IsErrorEnabled)
                    _log.Error(f.Cause, "Error occurred while uploading Spans to Kafka endpoints [{0}]",
                        string.Join(",", _options.BootstrapServers));
            });
        }

        private void ExecuteDelivery()
        {
            _kafkaProducer.TransmitSpans(PendingMessages).PipeTo(Self);

            PendingMessages.Clear();
            RescheduleBatchTransmission();
        }

        private void RescheduleBatchTransmission()
        {
            _batchTransimissionTimer?.Cancel(false);
            _batchTransimissionTimer =
                Context.System.Scheduler.ScheduleTellOnceCancelable(_options.MaxBatchInterval, Self,
                    DeliverBatch.Instance, Self);
        }

        protected override void PreStart()
        {
            RescheduleBatchTransmission();
            _kafkaProducer = new KafkaTransmitter(_options.TopicName,
                new Producer<Null, byte[]>(_options.ToDriverConfig(),
                    new NullSerializer(), new ByteArraySerializer()), _options.Serializer);

            if (_options.DebugLogging)
                _log.Debug("Connected to Kafka at [{0}] on topic [{1}] for Zipkin",
                    string.Join(",", _options.BootstrapServers), _options.TopicName);
        }

        protected override void PostStop()
        {
            _batchTransimissionTimer?.Cancel();
            _kafkaProducer?.Dispose();
        }

        /// <summary>
        ///     INTERNAL API.
        ///     Signal
        /// </summary>
        private sealed class DeliverBatch
        {
            public static readonly DeliverBatch Instance = new DeliverBatch();

            private DeliverBatch()
            {
            }
        }
    }
}