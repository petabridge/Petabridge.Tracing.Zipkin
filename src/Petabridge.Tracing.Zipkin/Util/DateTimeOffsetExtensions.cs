// -----------------------------------------------------------------------
// <copyright file="DateTimeOffsetExtensions.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Petabridge.Tracing.Zipkin.Util
{
    /// <summary>
    ///     INTERNAL API.
    ///     Used for generating UNIX epoch timestamps where needed.
    /// </summary>
    internal static class DateTimeOffsetExtensions
    {
        private static readonly long UnixEpoch = new DateTimeOffset(new DateTime(1970, 1, 1)).Ticks;

        public static long ToUnixTimestampMs(DateTimeOffset offset)
        {
            return (offset.Ticks - UnixEpoch) / TimeSpan.TicksPerMillisecond;
        }
    }
}