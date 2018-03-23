using System;
using System.Collections.Generic;
using OpenTracing;

namespace Petabridge.Tracing.Zipkin
{
    /// <summary>
    /// Zipkin span context. 
    /// </summary>
    public sealed class SpanContext : ISpanContext
    {
        public SpanContext(long traceLowId, long spanId, long? parentId = null, bool? isSampled = null, bool debug = false) 
            : this(new TraceId(traceLowId), spanId, parentId, isSampled, debug) { }

        public SpanContext(TraceId traceId, long spanId, long? parentId = null, bool? isSampled = null, bool debug = false)
        {
            TraceId = traceId;
            SpanId = spanId;
            ParentId = parentId;
            Sampled = isSampled;
            Debug = debug;
        }

        /// <summary>
        /// The trace ID. Intended to be shared across spans for
        /// a single logical trace.
        /// </summary>
        public TraceId TraceId { get; }

        /// <summary>
        /// The span ID. Used to identify a single atomic operation that is being
        /// tracked as part of an ongoing trace.
        /// </summary>
        public long SpanId { get; }

        /// <summary>
        /// Optional. Identify of the parent span if there is one.
        /// </summary>
        public long? ParentId { get; }

        /// <summary>
        /// Optional. Indicates if this span is from a tracer that is currently using sampling.
        /// </summary>
        public bool? Sampled { get; }

        /// <summary>
        /// Flag to indicate if we're in debug mode or not.
        /// </summary>
        public bool Debug { get; }

        public IEnumerable<KeyValuePair<string, string>> GetBaggageItems()
        {
            throw new NotSupportedException("Baggage is not supported in Zipkin.");
        }
    }
}
