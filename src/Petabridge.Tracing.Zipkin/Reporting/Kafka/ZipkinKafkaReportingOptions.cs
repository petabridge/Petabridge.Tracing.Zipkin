// -----------------------------------------------------------------------
// <copyright file="ZipkinKafkaReportingOptions.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2019 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Petabridge.Tracing.Zipkin.Reporting.Kafka
{
    /// <summary>
    ///     All of the options used to configure <see cref="Span" /> reporting via Kafka.
    /// </summary>
    /// <example>
    /// var tracer = new ZipkinTracer(new ZipkinTracerOptions(new Endpoint("AppKafka"),
    ///     ZipkinKafkaSpanReporter.Create(new ZipkinKafkaReportingOptions(new[] {"localhost:19092"},
    ///     debugLogging: true))));
    /// </example>
    public sealed class ZipkinKafkaReportingOptions
    {
        public const int DefaultBatchSize = 30;

        /// <summary>
        ///     The default topic name used by Zipkin, according to
        ///     https://github.com/openzipkin/zipkin/blob/master/zipkin-autoconfigure/collector-kafka/README.md
        /// </summary>
        public const string DefaultKafkaTopicName = "zipkin";

        public static readonly TimeSpan DefaultReportingInterval = TimeSpan.FromMilliseconds(100);

        public ZipkinKafkaReportingOptions(IReadOnlyList<string> bootstrapServers,
            string topicName = DefaultKafkaTopicName,
            int maximumBatchSize = DefaultBatchSize, TimeSpan? maxBatchInterval = null,
            bool debugLogging = false, bool errorLogging = true)
        {
            TopicName = topicName;
            BootstrapServers = bootstrapServers;
            MaximumBatchSize = maximumBatchSize;
            MaxBatchInterval = maxBatchInterval ?? DefaultReportingInterval;
            DebugLogging = debugLogging;
            ErrorLogging = errorLogging;
            Serializer = new JsonSpanSerializer();
        }

        /// <summary>
        ///     The name of the Kafka topic that will be consumed by Zipkin
        ///     for span reporting purposes.
        /// </summary>
        /// <remarks>
        ///     Defaults to "zipkin".
        /// </remarks>
        public string TopicName { get; }

        /// <summary>
        ///     The set of servers we will use to initially contact Kafka
        /// </summary>
        public IReadOnlyList<string> BootstrapServers { get; }

        /// <summary>
        ///     The maximum number of <see cref="Span" /> instances allowed in a single batch transmission.
        /// </summary>
        public int MaximumBatchSize { get; }

        /// <summary>
        ///     The maximum allowed time interval between batches.
        /// </summary>
        public TimeSpan MaxBatchInterval { get; }

        /// <summary>
        ///     Enables debug logging via the Akka.NET logging channels
        /// </summary>
        public bool DebugLogging { get; }

        /// <summary>
        ///     Enables error logging via the Akka.NET logging channels. Defaults to <c>true</c>.
        /// </summary>
        public bool ErrorLogging { get; }

        /// <summary>
        ///     The serializer used for encoding <see cref="Span" /> instances on the wire.
        ///     Defaults to <see cref="JsonSpanSerializer" />
        /// </summary>
        public ISpanSerializer Serializer { get; set; }

        /// <summary>
        ///     Creates a configuration object in the style expected by the Confluent.Kafka driver.
        /// </summary>
        /// <returns>A new dictionary instance each time.</returns>
        public IReadOnlyDictionary<string, object> ToDriverConfig()
        {
            return new Dictionary<string, object>
            {
                {"bootstrap.servers", string.Join(",", BootstrapServers)},
                {"request.required.acks", "0"} // don't wait for broker to send ACKs back (it's just trace data)
            };
        }
    }
}