// -----------------------------------------------------------------------
// <copyright file="TraceId.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2019 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using Petabridge.Tracing.Zipkin.Util;

namespace Petabridge.Tracing.Zipkin
{
    /// <summary>
    ///     The ID of this trace, with support for both 128bit and 64bit trace ids.S
    /// </summary>
    public struct TraceId : IEquatable<TraceId>
    {
        public TraceId(long traceIdLow) : this(0, traceIdLow)
        {
        }

        public TraceId(long traceIdHigh, long traceIdLow)
        {
            TraceIdHigh = traceIdHigh;
            TraceIdLow = traceIdLow;
        }

        public long TraceIdHigh { get; }
        public long TraceIdLow { get; }

        public bool Is128Bit => TraceIdHigh != 0;

        public bool Equals(TraceId other)
        {
            return TraceIdHigh == other.TraceIdHigh && TraceIdLow == other.TraceIdLow;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is TraceId && Equals((TraceId) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (TraceIdHigh.GetHashCode() * 397) ^ TraceIdLow.GetHashCode();
            }
        }

        public static bool operator ==(TraceId left, TraceId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TraceId left, TraceId right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return this.TraceIdToStringConcat();
        }

        /// <summary>
        ///     Attempts to parse a <see cref="TraceId" /> from a HEX string.
        /// </summary>
        /// <param name="hex">A valid HEX string.</param>
        /// <param name="traceId">The TraceId that was parsed from <see cref="hex" />, if any.</param>
        /// <returns><c>true</c> if the parse operation was successful, <c>false</c> otherwise.</returns>
        public static bool TryParse(string hex, out TraceId traceId)
        {
            traceId = default(TraceId);
            if (string.IsNullOrEmpty(hex)) return false;

            switch (hex.Length)
            {
                case 32: //128bit
                    var high = hex.Substring(0, 16);
                    var low = hex.Substring(16);
                    if (long.TryParse(high, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var traceIdHigh)
                        && long.TryParse(low, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var traceIdLow))
                    {
                        traceId = new TraceId(traceIdHigh, traceIdLow);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case 16: //64bit
                    if (long.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var traceIdLow2))
                    {
                        traceId = new TraceId(traceIdLow2);
                        return true;
                    }

                    return false;
                default: //something illegal
                    return false;
            }
        }
    }
}