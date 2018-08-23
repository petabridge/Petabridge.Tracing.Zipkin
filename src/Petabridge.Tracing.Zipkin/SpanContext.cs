// -----------------------------------------------------------------------
// <copyright file="SpanContext.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Petabridge.Tracing.Zipkin
{
    /// <summary>
    ///     Zipkin span context.
    /// </summary>
    public sealed class SpanContext : IZipkinSpanContext
    {
        [Obsolete("As of Petabridge.Tracing.Zipkin v0.3.0, we now support OpenTracing v0.12 and all drivers " +
                  "are required to use strings as span and trace ids. Please use the string-based overload " +
                  "of this constructor instead.")]
        public SpanContext(TraceId traceId, long spanId, string parentId = null, bool debug = false,
            bool sampled = false, bool shared = false) 
            : this(traceId, spanId.ToString("x16"), parentId, 
                debug, sampled, shared)
        {
        }

        public SpanContext(TraceId traceId, string spanId, string parentId = null, bool debug = false,
            bool sampled = false, bool shared = false)
        {
            ZipkinTraceId = traceId;
            SpanId = spanId;
            ParentId = parentId;
            Debug = debug;
            Sampled = !Debug && sampled;
            Shared = shared;
        }

        /// <inheritdoc />
        /// <summary>
        ///     The trace ID. Intended to be shared across spans for
        ///     a single logical trace.
        /// </summary>
        public TraceId ZipkinTraceId { get; }

        /// <inheritdoc />
        public string TraceId => ZipkinTraceId.ToString();

        /// <summary>
        ///     The span ID. Used to identify a single atomic operation that is being
        ///     tracked as part of an ongoing trace.
        /// </summary>
        public string SpanId { get; }

        /// <summary>
        ///     Optional. Identify of the parent span if there is one.
        /// </summary>
        public string ParentId { get; }

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

        public bool Equals(IZipkinSpanContext other)
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
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is SpanContext && Equals((SpanContext) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ZipkinTraceId.GetHashCode();
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