// -----------------------------------------------------------------------
// <copyright file="ISpanIdProvider.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

namespace Petabridge.Tracing.Zipkin
{
    /// <summary>
    ///     Abstracted API for generating unique TraceID instances.
    /// </summary>
    public interface ISpanIdProvider
    {
        /// <summary>
        ///     Determines if we're using 128 bit IDs or not.
        /// </summary>
        bool Use128Bit { get; }

        /// <summary>
        ///     Generates a new <see cref="TraceId" /> objects.
        /// </summary>
        /// <returns>A new, hopefully unique trace.</returns>
        TraceId NextTraceId();

        /// <summary>
        ///     Generates a new unique identifier for a span.
        /// </summary>
        /// <returns>A new, hoepfully unique trace.</returns>
        long NextSpanId();
    }
}