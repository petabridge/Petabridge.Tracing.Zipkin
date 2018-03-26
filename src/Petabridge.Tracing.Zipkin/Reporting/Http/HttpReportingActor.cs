// -----------------------------------------------------------------------
// <copyright file="HttpReportingActor.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Akka.Actor;
using Akka.Event;
using Phobos.Actor.Common;

namespace Petabridge.Tracing.Zipkin.Reporting.Http
{
    /// <summary>
    ///     INTERNAL API.
    ///     Actor responsible for managing the batching of outbound <see cref="T:Petabridge.Tracing.Zipkin.Span" /> instances.
    /// </summary>
    internal sealed class HttpReportingActor : ReceiveActor, INeverMonitor, INeverTrace
    {
        private readonly ILoggingAdapter _log = Context.GetLogger();

        private readonly ZipkinHttpReportingOptions _options;

        private readonly ZipkinHttpApiTransmitter _transmitter;

        private ICancelable _batchTransimissionTimer;

        public HttpReportingActor(ZipkinHttpReportingOptions options)
        {
            _options = options;
            PendingMessages = new List<Span>(_options.MaximumBatchSize);
            var uri = ZipkinHttpApiTransmitter.GetFullZipkinUri(_options.ZipkinUrl);
            _transmitter = new ZipkinHttpApiTransmitter(new HttpClient(), uri);

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

            Receive<HttpResponseMessage>(rsp =>
            {
                if (_log.IsDebugEnabled)
                    _log.Debug(
                        "Received notification that Span batch was received by Zipkin at [{0}] with success code [{1}]",
                        _transmitter.Uri, rsp.StatusCode);
            });

            // Indicates that one of our HTTP requests timed out
            Receive<Status.Failure>(f =>
            {
                if (_log.IsErrorEnabled)
                    _log.Error(f.Cause, "Error occurred while uploading Spans to [{0}]", _transmitter.Uri);
            });
        }

        private void ExecuteDelivery()
        {
            _transmitter.TransmitSpans(PendingMessages, _options.ZipkinHttpTimeout).PipeTo(Self);

            /*
                     * TransmitSpans will synchronously write out the JSON in a stream before this method
                     * returns, therefore it is safe for us to modify the PendingMessages collection directly.
                     */
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
        }

        protected override void PostStop()
        {
            _batchTransimissionTimer?.Cancel();
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