// -----------------------------------------------------------------------
// <copyright file="SpanSpecs.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Petabridge.Tracing.Zipkin.Tracers;
using Phobos.Tracing.Zipkin;
using Xunit;

namespace Petabridge.Tracing.Zipkin.Tests
{
    public class SpanSpecs
    {
        public SpanSpecs()
        {
            Tracer = new MockZipkinTracer();
        }

        protected readonly MockZipkinTracer Tracer;

        [Fact(DisplayName = "Should be able to create child spans")]
        public void ShouldCreateChildSpansWithSameTraceId()
        {
            var span1 = (Span) Tracer.BuildSpan("op1").Start();
            var span2 = (Span) Tracer.BuildSpan("op1.op2").AsChildOf(span1).Start();

            span2.Finish();
            span1.Finish();

            // same trace IDs
            span1.TypedContext.TraceId.Should().Be(span2.TypedContext.TraceId);

            // different span IDs
            span1.TypedContext.SpanId.Should().NotBe(span2.TypedContext.SpanId);
            span1.OperationName.Should().NotBe(span2.OperationName);
        }

        [Fact(DisplayName = "Should be able to create a basic span")]
        public void ShouldCreateNewSpan()
        {
            var span = (Span) Tracer.BuildSpan("op1").Start();
            span.Finish();

            span.SpanKind.Should().BeNull();
            span.Debug.Should().BeFalse();
            span.OperationName.Should().Be("op1");
            span.LocalEndpoint.Should().BeEquivalentTo(Tracer.LocalEndpoint);
            span.RemoteEndpoint.Should().BeNull();
            span.Annotations.Should().BeEmpty();
            span.Tags.Should().BeEmpty();
            span.TypedContext.ParentId.Should().BeNull();
            span.Finished.Should().NotBeNull();
            span.Shared.Should().BeFalse();

            // verify that the Report function works
            Tracer.CollectedSpans.Count.Should().Be(1);
            Tracer.CollectedSpans.TryDequeue(out var innerSpan);
            innerSpan.Should().Be(span);
        }

        [Fact(DisplayName = "Should not leak the TraceId across unrelated spans")]
        public void ShouldNotLeakTraceIdAcrossUnrelatedSpans()
        {
            var span1 = Tracer.BuildSpan("op1").Start();
            var span2 = Tracer.BuildSpan("op2").Start();
            span1.Finish();
            span2.Finish();

            span1.TypedContext.TraceId.Should().NotBe(span2.TypedContext.TraceId);
        }

        [Fact(DisplayName = "Should be able to use the Debug flag on spans")]
        public void ShouldUseDebugFlag()
        {
            var span1 = Tracer.BuildSpan("op1").SetDebugMode(false).Start();
            span1.Finish();

            Tracer.CollectedSpans.TryDequeue(out var innerSpan);
            innerSpan.Debug.Should().BeTrue();
        }

        [Fact(DisplayName = "If a NoOpSpanContext is added as a reference to a valid span, should just discard and not throw error")]
        public void SamplingBugFixShouldNotThrowExceptionNoOpSpanContextAddedAsReference()
        {
            var span1 = NoOpSpan.Instance;
            var span2 = Tracer.BuildSpan("foo").AsChildOf(span1).Start();

            // should safely ignore the span
            span2.TypedContext.ParentId.Should().BeNull();
        }
    }
}