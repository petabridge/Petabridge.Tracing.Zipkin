// -----------------------------------------------------------------------
// <copyright file="IZipkinTracer.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using OpenTracing;

namespace Petabridge.Tracing.Zipkin
{
    /// <summary>
    ///     Interface for instrumenting
    /// </summary>
    public interface IZipkinTracer : ITracer
    {
        /// <summary>
        ///     The service endpoint that this <see cref="ITracer" /> is currently running on.
        /// </summary>
        Endpoint LocalEndpoint { get; }

        /// <summary>
        ///     The clock used by this <see cref="IZipkinTracer" />.
        /// </summary>
        ITimeProvider TimeProvider { get; }

        /// <summary>
        ///     Factory using for emitting <see cref="TraceId" /> and SpanId objects.
        /// </summary>
        ISpanIdProvider IdProvider { get; }

        /// <summary>
        ///     Pipes the completed span to a <see cref="ITraceReporter" /> for delivery.
        /// </summary>
        /// <param name="span">A completed span.</param>
        void Report(Span span);

        /// <summary>
        /// Used to build <see cref="Span"/> instances.
        /// </summary>
        /// <param name="operationName">The name of the operation we are tracing.</param>
        /// <returns>A new <see cref="IZipkinSpanBuilder"/> instance.</returns>
        new IZipkinSpanBuilder BuildSpan(string operationName);
    }
}