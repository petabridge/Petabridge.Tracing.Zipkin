// -----------------------------------------------------------------------
// <copyright file="ZipkinTracerOptions.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
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
    }
}