using System;
using System.Collections.Generic;
using System.Text;

namespace Petabridge.Tracing.Zipkin.Reporting.Kafka
{
    /// <summary>
    /// All of the options used to configure <see cref="Span"/> reporting via Kafka.
    /// </summary>
    public sealed class ZipkingKafkaReportingOptions
    {
        public const int DefaultBatchSize = 30;
        public static readonly TimeSpan DefaultReportingInterval = TimeSpan.FromMilliseconds(100);

        public ZipkingKafkaReportingOptions(string topicName, IReadOnlyList<string> bootstrapServers, 
            int maximumBatchSize = DefaultBatchSize, TimeSpan? maxBatchInterval = null, 
            bool debugLogging = false, bool errorLogging = true)
        {
            TopicName = topicName;
            BootstrapServers = bootstrapServers;
            MaximumBatchSize = maximumBatchSize;
            MaxBatchInterval = maxBatchInterval ?? DefaultReportingInterval;
            DebugLogging = debugLogging;
            ErrorLogging = errorLogging;
        }

        /// <summary>
        /// The default topic name used by Zipkin, according to https://github.com/openzipkin/zipkin/blob/master/zipkin-autoconfigure/collector-kafka/README.md
        /// </summary>
        public const string DefaultKafkaTopicName = "zipkin";

        /// <summary>
        /// The name of the Kafka topic that will be consumed by Zipkin
        /// for span reporting purposes.
        /// </summary>
        /// <remarks>
        /// Defaults to "zipkin".
        /// </remarks>
        public string TopicName { get; }

        /// <summary>
        /// The set of servers we will use to initially contact Kafka
        /// </summary>
        public IReadOnlyList<string> BootstrapServers { get; }

        /// <summary>
        ///     The maximum number of <see cref="Span" /> instances allowed in a single batch transmission.
        /// </summary>
        public int MaximumBatchSize { get; }

        /// <summary>
        ///     The maxium allowed time interval between batches.
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
    }
}
