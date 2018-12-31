// -----------------------------------------------------------------------
// <copyright file="B3Propagator.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using OpenTracing.Propagation;
using Petabridge.Tracing.Zipkin.Exceptions;

namespace Petabridge.Tracing.Zipkin.Propagation
{
    /// <summary>
    /// Used for parsing the B3 "single header" format
    /// </summary>
    /// <remarks>
    /// See https://github.com/openzipkin/b3-propagation/issues/21 for rationale.
    /// </remarks>
    public sealed class B3SingleHeaderFormatter
    {
        /// <summary>
        /// The maximum length of a fully specified B3 single length header
        /// </summary>
        public const int FORMAT_MAX_LENGTH = 32 + 1 + 16 + 3 + 16; // traceid128-spanid-1-parentid
    }

    /// <inheritdoc />
    /// <summary>
    ///     Propagation system using B3 Headers supported by Zipkin
    /// </summary>
    /// <remarks>
    ///     See https://github.com/openzipkin/b3-propagation for implementation details
    /// </remarks>
    public sealed class B3Propagator : IPropagator<ITextMap>
    {
        internal const string B3TraceId = "x-b3-traceid";
        internal const string B3SpanId = "x-b3-spanid";
        internal const string B3ParentId = "x-b3-parentspanid";
        internal const string B3Sampled = "x-b3-sampled";
        internal const string B3Debug = "x-b3-flags";

        public void Inject(SpanContext context, ITextMap carrier)
        {
            carrier.Set(B3TraceId, context.TraceId);
            carrier.Set(B3SpanId, context.SpanId);
            if (context.ParentId != null)
                carrier.Set(B3ParentId, context.ParentId);

            if (context.Debug)
            {
                carrier.Set(B3Debug, "1");
                carrier.Set(B3Sampled, "0"); // can't have sampling while DEBUG is on
            }
            else
            {
                carrier.Set(B3Debug, "0");
                if (context.Sampled)
                    carrier.Set(B3Sampled, "1");
                else
                    carrier.Set(B3Sampled, "0");
            }
        }

        public SpanContext Extract(ITextMap carrier)
        {
            TraceId? traceId = null;
            string spanId = null;
            string parentId = null;
            var debug = false;
            var sampled = false;
            const bool shared = false;
            foreach (var entry in carrier)
                switch (entry.Key.ToLowerInvariant())
                {
                    case B3TraceId:
                        if (!TraceId.TryParse(entry.Value, out var t))
                            throw new ZipkinFormatException(
                                $"TraceId in format [{entry.Value}] is incompatible. Please use an X16 encoded 128bit or 64bit id.");
                        traceId = t;
                        break;
                    case B3SpanId:
                        spanId = entry.Value;
                        break;
                    case B3ParentId:
                        parentId = entry.Value;
                        break;
                    case B3Debug:
                        if (entry.Value.Equals("1"))
                            debug = true;
                        break;
                    case B3Sampled:
                        if (entry.Value.Equals("1"))
                            sampled = true;
                        break;
                }

            if (traceId != null && spanId != null) // don't care of ParentId is null or not
                return new SpanContext(traceId.Value, spanId, parentId, debug, sampled, shared);
            return null;
        }
    }
}