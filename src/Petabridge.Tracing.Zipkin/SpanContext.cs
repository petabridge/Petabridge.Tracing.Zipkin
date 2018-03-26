// -----------------------------------------------------------------------
// <copyright file="SpanContext.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using OpenTracing;

namespace Petabridge.Tracing.Zipkin
{
    /// <summary>
    ///     Zipkin span context.
    /// </summary>
    public sealed class SpanContext : ISpanContext, IEquatable<SpanContext>
    {
        public SpanContext(TraceId traceId, long spanId, long? parentId = null, bool debug = false,
            bool sampled = false, bool shared = false)
        {
            TraceId = traceId;
            SpanId = spanId;
            ParentId = parentId;
            Debug = debug;
            Sampled = sampled;
            Shared = shared;
        }

        /// <summary>
        ///     The trace ID. Intended to be shared across spans for
        ///     a single logical trace.
        /// </summary>
        public TraceId TraceId { get; }

        /// <summary>
        ///     The span ID. Used to identify a single atomic operation that is being
        ///     tracked as part of an ongoing trace.
        /// </summary>
        public long SpanId { get; }

        /// <summary>
        ///     Optional. Identify of the parent span if there is one.
        /// </summary>
        public long? ParentId { get; }

        /// <summary>
        ///     Indicates if this is a Debug trace or not.
        /// </summary>
        public bool Debug { get; }

        /// <summary>
        ///     Indicates if this is a sampled trace or not.
        /// </summary>
        public bool Sampled { get; }

        /// <summary>
        ///     Indicates if this is a shared trace or not.
        /// </summary>
        public bool Shared { get; }

        public bool Equals(SpanContext other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return TraceId.Equals(other.TraceId) && SpanId == other.SpanId && ParentId == other.ParentId;
        }

        public IEnumerable<KeyValuePair<string, string>> GetBaggageItems()
        {
            throw new NotSupportedException("Baggage is not supported in Zipkin.");
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is SpanContext && Equals((SpanContext) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = TraceId.GetHashCode();
                hashCode = (hashCode * 397) ^ SpanId.GetHashCode();
                hashCode = (hashCode * 397) ^ ParentId.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(SpanContext left, SpanContext right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SpanContext left, SpanContext right)
        {
            return !Equals(left, right);
        }
    }
}