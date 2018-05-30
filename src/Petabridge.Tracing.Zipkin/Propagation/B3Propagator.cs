// -----------------------------------------------------------------------
// <copyright file="B3Propagator.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using OpenTracing.Propagation;
using Petabridge.Tracing.Zipkin.Exceptions;

namespace Petabridge.Tracing.Zipkin.Propagation
{
    /// <inheritdoc />
    /// <summary>
    ///     Propagation system using B3 Headers supported by Zipkin
    /// </summary>
    /// <remarks>
    ///     See https://github.com/openzipkin/b3-propagation for implementation details
    /// </remarks>
    public sealed class B3Propagator : IPropagator<ITextMap>
    {
        internal const string B3TraceId = "X-B3-TraceId";
        internal const string B3SpanId = "X-B3-SpanId";
        internal const string B3ParentId = "X-B3-ParentSpanId";
        internal const string B3Sampled = "X-B3-Sampled";
        internal const string B3Debug = "X-B3-Flags";

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
            var traceId = default(TraceId);
            long spanId = 0;
            long? parentId = null;
            var debug = false;
            var sampled = false;
            const bool shared = false;
            foreach (var entry in carrier)
                switch (entry.Key)
                {
                    case B3TraceId:
                        if (!TraceId.TryParse(entry.Value, out traceId))
                            throw new ZipkinFormatException(
                                $"TraceId in format [{entry.Value}] is incompatible. Please use an X16 encoded 128bit or 64bit id.");
                        break;
                    case B3SpanId:
                        if (!long.TryParse(entry.Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                            out spanId))
                            throw new ZipkinFormatException(
                                $"SpanId in format [{entry.Value}] is incompatible. Please use an X16 encoded 64bit id.");
                        break;
                    case B3ParentId:
                        if (!long.TryParse(entry.Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture,
                            out var p))
                            throw new ZipkinFormatException(
                                $"ParentId in format [{entry.Value}] is incompatible. Please use an X16 encoded 64bit id.");
                        parentId = p;
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

            return new SpanContext(traceId, spanId, parentId?.ToString("x16"), debug, sampled, shared);
        }
    }
}