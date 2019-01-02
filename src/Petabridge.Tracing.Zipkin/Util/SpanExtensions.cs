// -----------------------------------------------------------------------
// <copyright file="SpanExtensions.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2019 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using OpenTracing;
using Petabridge.Tracing.Zipkin.Tracers;

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
            return context == null || NoOp.Span.Context.Equals(context) ||
                   Equals(NoOpZipkinSpanContext.Instance, context);
        }

        /// <summary>
        ///     Determines if an <see cref="ISpanContext" /> is a valid <see cref="IZipkinSpanContext" /> or not.
        /// </summary>
        /// <param name="context">The span context.</param>
        /// <returns><c>true</c> if the span is valid, <c>false</c> otherwise.</returns>
        public static bool IsZipkinSpan(this ISpanContext context)
        {
            return !IsEmpty(context) && context is IZipkinSpanContext;
        }
    }
}