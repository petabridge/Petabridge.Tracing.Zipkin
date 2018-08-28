// -----------------------------------------------------------------------
// <copyright file="ZipkinTracerOptions.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using OpenTracing;
using OpenTracing.Util;
using Petabridge.Tracing.Zipkin.Reporting.Http;

namespace Petabridge.Tracing.Zipkin
{
    /// <summary>
    ///     The set of options designed for use with the <see cref="ZipkinTracer" />.
    /// </summary>
    public sealed class ZipkinTracerOptions
    {
        /// <summary>
        ///     Creates a set of <see cref="ZipkinTracerOptions" /> that uses a <see cref="ZipkinHttpSpanReporter" />
        ///     created from the provided values.
        /// </summary>
        /// <param name="zipkinHttpUrl">The HTTP API of the Zipkin server.</param>
        /// <param name="localServiceName">The name of the local service.</param>
        /// <param name="localServiceAddress">The bound address of the local service.</param>
        /// <param name="localServicePort">The bound port of the local service.</param>
        /// <param name="debug">Turns on debugging settings.</param>
        public ZipkinTracerOptions(string zipkinHttpUrl, string localServiceName, string localServiceAddress = null,
            int? localServicePort = null, bool debug = false)
        {
            Reporter = ZipkinHttpSpanReporter.Create(new ZipkinHttpReportingOptions(zipkinHttpUrl,
                debugLogging: debug));
            LocalEndpoint = new Endpoint(localServiceName, localServiceAddress, localServicePort);
            DebugMode = debug;
        }

        public ZipkinTracerOptions(Endpoint localEndpoint, ISpanReporter reporter)
        {
            LocalEndpoint = localEndpoint;
            Reporter = reporter;
        }

        /// <summary>
        ///     The reporting engine used for transmitting events to Zipkin.
        ///     Almost always a <see cref="ZipkinHttpSpanReporter" />.
        /// </summary>
        public ISpanReporter Reporter { get; }

        /// <summary>
        ///     The local <see cref="Endpoint" /> for traces that are emitted from this app.
        /// </summary>
        public Endpoint LocalEndpoint { get; }

        /// <summary>
        ///     Are we running this <see cref="ITracer" /> on a client app? Specify <see cref="SpanKind.CLIENT" />
        /// </summary>
        public SpanKind? DefaultSpanKind { get; set; }

        /// <summary>
        ///     Mostly an internal API and will be set automatically with a safe default if not
        ///     explicitly set by user.
        /// </summary>
        public ITimeProvider TimeProvider { get; set; }

        /// <summary>
        ///     Mostly an internal API and will be set automatically with a safe default if not
        ///     explicitly set by user.
        /// </summary>
        public ISpanIdProvider IdProvider { get; set; }

        /// <summary>
        ///     Really useful to set this if you know what contexts your spans will be used inside of
        ///     with this <see cref="IZipkinTracer" />. For instance, if you know 100% of your <see cref="Span" />
        ///     instances will be created inside async / await blocks, use the <see cref="AsyncLocalScopeManager" />.
        /// </summary>
        public IScopeManager ScopeManager { get; set; }

        /// <summary>
        ///     Used to determine which <see cref="Span" /> instances are included in the sample and which are not.
        /// </summary>
        public ITraceSampler Sampler { get; set; }

        /// <summary>
        ///     Toggles Zipkin's "debug" mode on or off.
        /// </summary>
        public bool DebugMode { get; set; }
    }
}