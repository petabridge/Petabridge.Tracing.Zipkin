// -----------------------------------------------------------------------
// <copyright file="IZipkinTracer.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using OpenTracing;

namespace Petabridge.Tracing.Zipkin
{
    /// <inheritdoc />
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
        ///     Strategy used for determining which <see cref="Span" /> instances are included in the sample
        ///     and which are not.
        /// </summary>
        ITraceSampler Sampler { get; }

        /// <summary>
        ///     Pipes the completed span to a <see cref="ISpanReporter" /> for delivery.
        /// </summary>
        /// <param name="span">A completed span.</param>
        void Report(Span span);

        /// <summary>
        ///     Used to build <see cref="Span" /> instances.
        /// </summary>
        /// <param name="operationName">The name of the operation we are tracing.</param>
        /// <returns>A new <see cref="IZipkinSpanBuilder" /> instance.</returns>
        new IZipkinSpanBuilder BuildSpan(string operationName);
    }
}