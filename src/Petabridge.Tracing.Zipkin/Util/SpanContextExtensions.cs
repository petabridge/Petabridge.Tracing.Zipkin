using System;
using System.Collections.Generic;
using System.Text;
using OpenTracing;

namespace Petabridge.Tracing.Zipkin.Util
{
    /// <summary>
    /// Extension methods for working with <see cref="ISpanContext"/>
    /// </summary>
    public static class SpanContextExtensions
    {
        /// <summary>
        /// Determines if this <see cref="ISpanContext"/> is empty or not.
        /// </summary>
        /// <param name="context">The span context.</param>
        /// <returns><c>true</c> if the span is empty, <c>false</c> otherwise.</returns>
        public static bool IsEmpty(this ISpanContext context)
        {
            return context == null;
        }
    }
}
