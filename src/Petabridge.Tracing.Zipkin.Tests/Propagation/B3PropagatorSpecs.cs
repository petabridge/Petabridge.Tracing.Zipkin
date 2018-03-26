// -----------------------------------------------------------------------
// <copyright file="B3PropagatorSpecs.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using FsCheck;
using FsCheck.Xunit;
using OpenTracing.Propagation;
using Petabridge.Tracing.Zipkin.Tracers;

namespace Petabridge.Tracing.Zipkin.Tests.Propagation
{
    public class B3PropagatorSpecs
    {
        public readonly MockZipkinTracer Tracer;

        public B3PropagatorSpecs()
        {
            Tracer = new MockZipkinTracer();
        }

        [Property(DisplayName = "Should be able to extract and inject spans via B3 headers")]
        public Property ShouldExtractAndInjectSpansViaB3(long traceIdHigh, long traceIdLow, long spanId, long? parentId,
            bool debug)
        {
            var traceId = new TraceId(traceIdHigh, traceIdLow);
            var context = new SpanContext(traceId, spanId, parentId, debug);
            var carrier = new Dictionary<string, string>();

            Tracer.Inject(context, BuiltinFormats.HttpHeaders, new TextMapInjectAdapter(carrier));
            var extracted =
                (SpanContext) Tracer.Extract(BuiltinFormats.HttpHeaders, new TextMapExtractAdapter(carrier));

            return (context == extracted).Label($"Expected [{context}] to be equal to [{extracted}]");
        }
    }
}