// -----------------------------------------------------------------------
// <copyright file="MicrosecondConversion.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Petabridge.Tracing.Zipkin.Util
{
    /// <summary>
    ///     Extension methods for adding microsecond support.
    /// </summary>
    public static class MicrosecondConversion
    {
        public const long MicrosPerMillisecond = 1000;
        public const long TicksPerMicro = 10;

        public static long ToUnixMicros(this DateTimeOffset time)
        {
            return time.ToUnixTimeMilliseconds() * MicrosPerMillisecond;
        }

        public static long ToMicros(this TimeSpan time)
        {
            return time.Ticks / TicksPerMicro;
        }
    }
}