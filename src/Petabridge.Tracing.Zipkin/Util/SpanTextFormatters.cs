// -----------------------------------------------------------------------
// <copyright file="SpanTextFormatters.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using System.Runtime.CompilerServices;

namespace Petabridge.Tracing.Zipkin.Util
{
    /// <summary>
    ///     INTERNAL API.
    ///     Provides some utility function for formatting some texty bits.
    /// </summary>
    public static class SpanTextFormatters
    {
        /*
         * Why do we have things like TraceId.ToString implementations, multiple of them, sitting in this class?
         * Simply put: so we can compare all of them in performance tests and go with the best option.
         */

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string TraceIdToStringConcat(this TraceId traceId)
        {
            return traceId.TraceIdHigh == 0
                ? traceId.TraceIdLow.ToString("x16", CultureInfo.InvariantCulture)
                : string.Concat(traceId.TraceIdHigh.ToString("x16", CultureInfo.InvariantCulture),
                    traceId.TraceIdLow.ToString("x16", CultureInfo.InvariantCulture));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string TraceIdToStringSimple(this TraceId traceId)
        {
            return traceId.TraceIdHigh == 0
                ? traceId.TraceIdLow.ToString("x16", CultureInfo.InvariantCulture)
                : traceId.TraceIdHigh.ToString("x16", CultureInfo.InvariantCulture) +
                  traceId.TraceIdLow.ToString("x16", CultureInfo.InvariantCulture);
        }
    }
}