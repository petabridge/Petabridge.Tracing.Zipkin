// -----------------------------------------------------------------------
// <copyright file="SpanExtensions.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using OpenTracing;
using Phobos.Tracing.Zipkin;

namespace Petabridge.Tracing.Zipkin.Util
{
    /// <summary>
    ///     Extension methods for working with <see cref="ISpanContext" />
    /// </summary>
    public static class SpanExtensions
    {
        /// <summary>
        ///     Determines if this <see cref="ISpanContext" /> is empty or not.
        /// </summary>
        /// <param name="context">The span context.</param>
        /// <returns><c>true</c> if the span is empty, <c>false</c> otherwise.</returns>
        public static bool IsEmpty(this ISpanContext context)
        {
            return context == null || context is NoOpSpanContext;
        }
    }
}