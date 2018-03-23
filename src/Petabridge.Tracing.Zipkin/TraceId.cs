using System;

namespace Petabridge.Tracing.Zipkin
{
    /// <summary>
    /// The ID of this trace, with support for both 128bit and 64bit trace ids.S
    /// </summary>
    public struct TraceId : IEquatable<TraceId>
    {
        public TraceId(long traceIdLow) : this(0, traceIdLow) { }

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
    }
}