// -----------------------------------------------------------------------
// <copyright file="B3Propagator.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;
using OpenTracing.Propagation;
using Petabridge.Tracing.Zipkin.Exceptions;

namespace Petabridge.Tracing.Zipkin.Propagation
{
    /// <summary>
    /// Implements the "single header" B3 propagation format
    /// </summary>
    /// <remarks>
    /// See https://github.com/openzipkin/b3-propagation/issues/21 for full specification
    /// </remarks>
    public sealed class B3SingleHeaderPropagator : IPropagator<ITextMap>
    {
        internal const string B3SingleHeader = "b3";

        public void Inject(SpanContext context, ITextMap carrier)
        {
           carrier.Set(B3SingleHeader, B3SingleHeaderFormatter.WriteB3SingleFormat(context));
        }

        public SpanContext Extract(ITextMap carrier)
        {
            var b3Entry = carrier.Any(x => x.Key.Equals(B3SingleHeader));
            if (!b3Entry)
                return null;
            return B3SingleHeaderFormatter.ParseB3SingleFormat(carrier.Single(x => x.Key.Equals(B3SingleHeader)).Value);
        }
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
        private readonly B3SingleHeaderPropagator _singleHeaderPropagator = new B3SingleHeaderPropagator();
        private readonly bool _useB3SingleHeader;

        /// <inheritdoc />
        /// <summary>
        /// Default constructor for the B3 Propagator. Doesn't use single-header format by default.
        /// </summary>
        public B3Propagator() : this(false)
        {
            
        }

        /// <summary>
        /// Creates a new instance of the B3 Propagator.
        /// </summary>
        /// <param name="useB3SingleHeader">When set to <c>true</c>, enables the propagator to use the B3 single header propagation.</param>
        public B3Propagator(bool useB3SingleHeader)
        {
            _useB3SingleHeader = useB3SingleHeader;
        }

        internal const string B3TraceId = "x-b3-traceid";
        internal const string B3SpanId = "x-b3-spanid";
        internal const string B3ParentId = "x-b3-parentspanid";
        internal const string B3Sampled = "x-b3-sampled";
        internal const string B3Debug = "x-b3-flags";

        public void Inject(SpanContext context, ITextMap carrier)
        {
            if (_useB3SingleHeader)
            {
                _singleHeaderPropagator.Inject(context, carrier);
                return;
            }

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
            // try to extract the single B3 propagation value instead
            var single = _singleHeaderPropagator.Extract(carrier);
            if (single != null)
                return single;

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
                        if (entry.Value.Equals("1") || entry.Value.ToLowerInvariant().Equals("true")) // support older tracers https://github.com/petabridge/Petabridge.Tracing.Zipkin/issues/72
                            sampled = true;
                        break;
                }

            if (traceId != null && spanId != null) // don't care of ParentId is null or not
                return new SpanContext(traceId.Value, spanId, parentId, debug, sampled, shared);
            return null;
        }
    }
}