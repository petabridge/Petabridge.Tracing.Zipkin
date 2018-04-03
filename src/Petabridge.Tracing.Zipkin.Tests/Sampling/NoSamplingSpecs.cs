// -----------------------------------------------------------------------
// <copyright file="NoSamplingSpecs.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Petabridge.Tracing.Zipkin.Tracers;
using Xunit;

namespace Petabridge.Tracing.Zipkin.Tests.Sampling
{
    public class NoSamplingSpecs
    {
        public NoSamplingSpecs()
        {
            Tracer = new MockZipkinTracer();
        }

        protected readonly MockZipkinTracer Tracer;

        [Fact(DisplayName = "Sampling should be disabled by default")]
        public void SpansShouldHaveSamplingSetToFalseByDefault()
        {
            var span = Tracer.BuildSpan("foo").Start();
            span.Finish();

            span.Sampled.Should().BeFalse();
        }

        [Fact(DisplayName =
            "Sampling should be set to false even when explicitly overridden, but sampler is not running")]
        public void SpansShouldHaveSamplingSetToFalseEvenIfOverridden()
        {
            var span = Tracer.BuildSpan("foo").ForceIncludeInSample(true).Start();
            span.Finish();

            span.Sampled.Should().BeFalse();
        }
    }
}