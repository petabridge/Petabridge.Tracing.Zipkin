// -----------------------------------------------------------------------
// <copyright file="Bug25SpanTagsSpecs.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Petabridge.Tracing.Zipkin.Tracers;
using Xunit;

namespace Petabridge.Tracing.Zipkin.Tests
{
    /// <summary>
    ///     Specs aimed at proving the existence of and fix to
    ///     https://github.com/petabridge/Petabridge.Tracing.Zipkin/issues/25
    /// </summary>
    public class Bug25SpanTagsSpecs
    {
        public Bug25SpanTagsSpecs()
        {
            Tracer = new MockZipkinTracer();
        }

        protected readonly MockZipkinTracer Tracer;

        [Fact(DisplayName = "Should be able to apply and record tags to a SpanBuilder")]
        public void ShouldAddTagsToSpanBeingBuilt()
        {
            var span = (Span) Tracer.BuildSpan("op1").WithTag("foo", "bar").WithTag("baz", 1).Start();
            span.Finish();

            // grab the span from the collector (since this is the state it'll be reported in)
            Tracer.CollectedSpans.TryDequeue(out var finishedSpan).Should().BeTrue();
            finishedSpan.Tags.Count.Should().Be(2);
            finishedSpan.Tags["foo"].Should().Be("bar");
            finishedSpan.Tags["baz"].Should().Be("1");
        }

        [Fact(DisplayName = "Should be able to apply and record tags to a started span")]
        public void ShouldAddTagsToStartedSpan()
        {
            var span = (Span) Tracer.BuildSpan("op1").Start();
            span.SetTag("foo", "bar").SetTag("baz", 1).Finish();

            // grab the span from the collector (since this is the state it'll be reported in)
            Tracer.CollectedSpans.TryDequeue(out var finishedSpan).Should().BeTrue();
            finishedSpan.Tags.Count.Should().Be(2);
            finishedSpan.Tags["foo"].Should().Be("bar");
            finishedSpan.Tags["baz"].Should().Be("1");
        }
    }
}