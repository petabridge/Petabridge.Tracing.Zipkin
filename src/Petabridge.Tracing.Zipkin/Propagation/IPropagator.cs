// -----------------------------------------------------------------------
// <copyright file="IPropagator.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

namespace Petabridge.Tracing.Zipkin.Propagation
{
    /// <summary>
    ///     Used to inject and extract <see cref="SpanContext" /> instances based on supported,
    ///     serialized formats.
    /// </summary>
    /// <typeparam name="TCarrier">The carrier format for this span context.</typeparam>
    public interface IPropagator<in TCarrier>
    {
        /// <summary>
        ///     Injects the <see cref="SpanContext" /> into the carrier format.
        /// </summary>
        /// <param name="context">The underlying span's context.</param>
        /// <param name="carrier">The carrier format, used for transmission over the wire.</param>
        void Inject(SpanContext context, TCarrier carrier);

        /// <summary>
        ///     Extracts the <see cref="SpanContext" /> from the carrier format.
        /// </summary>
        /// <param name="carrier"></param>
        /// <returns></returns>
        SpanContext Extract(TCarrier carrier);
    }
}