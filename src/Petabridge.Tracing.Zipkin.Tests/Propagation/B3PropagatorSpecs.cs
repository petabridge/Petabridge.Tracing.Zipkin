﻿// -----------------------------------------------------------------------
// <copyright file="B3PropagatorSpecs.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using OpenTracing.Propagation;
using OpenTracing.Util;
using Petabridge.Tracing.Zipkin.Propagation;
using Petabridge.Tracing.Zipkin.Reporting.NoOp;
using Petabridge.Tracing.Zipkin.Tracers;
using Xunit;

namespace Petabridge.Tracing.Zipkin.Tests.Propagation
{
    public class B3PropagatorSpecs
    {
        public B3PropagatorSpecs()
        {
            Tracer = new MockZipkinTracer(propagtor: new B3Propagator());
        }

        public readonly MockZipkinTracer Tracer;

        [Property(DisplayName = "Should be able to extract and inject spans via B3 headers")]
        public Property ShouldExtractAndInjectSpansViaB3(long traceIdHigh, long traceIdLow, long spanId, long? parentId,
            bool debug)
        {
            var traceId = new TraceId(traceIdHigh, traceIdLow);
            var context = new SpanContext(traceId, spanId.ToString("x16"), parentId?.ToString("x16"), debug);
            var carrier = new Dictionary<string, string>();

            Tracer.Inject(context, BuiltinFormats.HttpHeaders, new TextMapInjectAdapter(carrier));
            var extracted =
                (SpanContext) Tracer.Extract(BuiltinFormats.HttpHeaders, new TextMapExtractAdapter(carrier));

            return (context == extracted).Label($"Expected [{context}] to be equal to [{extracted}]");
        }

        [Property(DisplayName = "Should be able to inject and extract sampling data via B3")]
        public Property ShouldSupportSamplingStatusViaB3(bool debug, bool sampled)
        {
            var traceId = Tracer.IdProvider.NextTraceId();
            var context = new SpanContext(traceId, Tracer.IdProvider.NextSpanId(), null, debug, sampled);
            var carrier = new Dictionary<string, string>();

            Tracer.Inject(context, BuiltinFormats.HttpHeaders, new TextMapInjectAdapter(carrier));
            var extracted =
                (SpanContext) Tracer.Extract(BuiltinFormats.HttpHeaders, new TextMapExtractAdapter(carrier));

            return (context.Shared == extracted.Shared)
                .Label($"Shared settings should match. Expected [{context.Shared}] but found [{extracted.Shared}]")
                .And((context.Debug == extracted.Debug).Label(
                    $"Debug settings should match. Expected [{context.Debug}] but found [{extracted.Debug}]"))
                .And((context.Sampled == extracted.Sampled).Label(
                    $"Sampled settings should match. Expected [{context.Sampled}] but found [{extracted.Sampled}]"));
        }

        /// <summary>
        ///     Verify fix for https://github.com/petabridge/Petabridge.Tracing.Zipkin/issues/56
        /// </summary>
        [Fact(DisplayName =
            "Bugfix for issue 56 - Propagator should not throw when attempting to inject non-Zipkin context.")]
        public void BugFix56NonZipkingContextShouldNotThrowUponInjectionAttempt()
        {
            // uses the MockZipkinTracer
            var carrier = new Dictionary<string, string>();
            Tracer.Inject(NoOpZipkinSpanContext.Instance, BuiltinFormats.HttpHeaders,
                new TextMapInjectAdapter(carrier));
            carrier.Count.Should().Be(0);

            // uses the real Zipkin tracer with a No-Op reporter
            var testTracer = new ZipkinTracer(new ZipkinTracerOptions(
                new Endpoint("test"), new NoOpReporter())
            {
                ScopeManager = new AsyncLocalScopeManager()
            });
            testTracer.Inject(NoOpZipkinSpanContext.Instance, BuiltinFormats.HttpHeaders,
                new TextMapInjectAdapter(carrier));
            carrier.Count.Should().Be(0);
        }

        /// <summary>
        ///     Verify for https://github.com/petabridge/Petabridge.Tracing.Zipkin/issues/55
        /// </summary>
        [Fact(DisplayName = "B3Propagator should not extract SpanContext when none found")]
        public void ShouldNotExtractAnyTraceIdWhenNoneFound()
        {
            // pass in an empty carrier
            var carrier = new Dictionary<string, string>();
            var extracted =
                (SpanContext) Tracer.Extract(BuiltinFormats.HttpHeaders, new TextMapExtractAdapter(carrier));

            extracted.Should().BeNull();
        }

        [Fact(DisplayName = "B3Propagator should extract headers without case sensitivity")]
        public void B3HeaderExtractionShouldBeCaseInsensitive()
        {
            var carrier = new Dictionary<string, string>();

            
            var traceId = Tracer.IdProvider.NextTraceId();
            var spanId = Tracer.IdProvider.NextSpanId();
            var parentId = Tracer.IdProvider.NextSpanId();

            // all upper case
            carrier[B3Propagator.B3TraceId.ToUpperInvariant()] = traceId.ToString();

            // all upper case
            carrier[B3Propagator.B3SpanId.ToLowerInvariant()] = spanId;

            // mixed case
            carrier["X-B3-ParentSPANid"] = parentId;

            var extracted =
                (SpanContext)Tracer.Extract(BuiltinFormats.HttpHeaders, new TextMapExtractAdapter(carrier));

            extracted.TraceId.Should().Be(traceId.ToString());
            extracted.SpanId.Should().Be(spanId);
            extracted.ParentId.Should().Be(parentId);
        }
    }
}