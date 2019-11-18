// -----------------------------------------------------------------------
// <copyright file="ZipkinHttpReportingOptions.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2019 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Petabridge.Tracing.Zipkin.Reporting.Http
{
    /// <summary>
    ///     All of the options used to power the <see cref="ZipkinHttpSpanReporter" />.
    /// </summary>
    public sealed class ZipkinHttpReportingOptions
    {
        public const int DefaultBatchSize = 30;
        public static readonly TimeSpan DefaultReportingInterval = TimeSpan.FromMilliseconds(100);
        public static readonly TimeSpan DefaultHttpTimeoutInterval = TimeSpan.FromSeconds(5);

        public ZipkinHttpReportingOptions(string zipkinUrl, int maximumBatchSize = DefaultBatchSize,
            TimeSpan? maxBatchInterval = null, TimeSpan? zipkinTimeoutDuration = null, bool debugLogging = false,
            bool errorLogging = true)
        {
            ZipkinUrl = zipkinUrl;
            MaximumBatchSize = maximumBatchSize;
            MaxBatchInterval = maxBatchInterval ?? DefaultReportingInterval;
            ZipkinHttpTimeout = zipkinTimeoutDuration ?? DefaultHttpTimeoutInterval;
            DebugLogging = debugLogging;
            ErrorLogging = errorLogging;
        }

        /// <summary>
        ///     The base URL of the Zipkin server. I.e. http://localhost:8080
        /// </summary>
        public string ZipkinUrl { get; }

        /// <summary>
        ///     The maximum number of <see cref="Span" /> instances allowed in a single batch transmission.
        /// </summary>
        public int MaximumBatchSize { get; }

        /// <summary>
        ///     The maxium allowed time interval between batches.
        /// </summary>
        public TimeSpan MaxBatchInterval { get; }

        /// <summary>
        ///     Timeout duration for pushing data to the Zipkin HTTP API.
        /// </summary>
        public TimeSpan ZipkinHttpTimeout { get; }

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