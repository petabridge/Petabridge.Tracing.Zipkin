// -----------------------------------------------------------------------
// <copyright file="ProbabilisticSamplerSpecs.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;
using FluentAssertions;
using Petabridge.Tracing.Zipkin.Sampling;
using Petabridge.Tracing.Zipkin.Tracers;
using Xunit;

namespace Petabridge.Tracing.Zipkin.Tests.Sampling
{
    public class ProbabilisticSamplerSpecs
    {
        public ProbabilisticSamplerSpecs()
        {
            // 10% sample rate
            Tracer = new MockZipkinTracer(sampler: new ProbabilisticSampler(0.1));
        }

        protected readonly MockZipkinTracer Tracer;

        [Fact(DisplayName = "ProbabilisticSampler should always include child spans")]
        public void ShouldIncludeAllChildSpansInSample()
        {
            var attemptedSpans = 100;
            var parentSpan = Tracer.BuildSpan("bar").ForceIncludeInSample().Start();
            for (var i = 0; i < attemptedSpans - 1; i++)
            {
                var span = Tracer.BuildSpan("foo" + i).AsChildOf(parentSpan).Start();
                span.Finish();
            }

            parentSpan.Finish();

            var gatheredSpans = Tracer.CollectedSpans.ToArray();
            gatheredSpans.Length.Should().Be(attemptedSpans); // sampling should have filtered out some spans
            gatheredSpans.All(x => x.Sampled).Should().BeTrue();
        }

        [Fact(DisplayName = "ProbabilisticSampler should mark all completed traces as Sampled=true")]
        public void ShouldMarkAllCompletedTracesAsSampled()
        {
            var attemptedSpans = 100;
            for (var i = 0; i < attemptedSpans; i++)
            {
                var span = Tracer.BuildSpan("foo" + i).Start();
                span.Finish();
            }

            var gatheredSpans = Tracer.CollectedSpans.ToArray();
            gatheredSpans.Length.Should().BeLessThan(attemptedSpans); // sampling should have filtered out some spans
            gatheredSpans.All(x => x.Sampled).Should().BeTrue();
        }

        [Fact(DisplayName = "ProbabilisticSampler should be overrideable via SpanBuilder.ForceIncludeInSample")]
        public void ShouldOverrideProbabilisticSamplerViaSpanBuilder()
        {
            var attemptedSpans = 100;
            for (var i = 0; i < attemptedSpans; i++)
            {
                var span = Tracer.BuildSpan("foo" + i).ForceIncludeInSample(true).Start();
                span.Finish();
            }

            var gatheredSpans = Tracer.CollectedSpans.ToArray();
            gatheredSpans.Length.Should().Be(attemptedSpans); // should have forced all spans in the sample
            gatheredSpans.All(x => x.Sampled).Should().BeTrue();
        }

        [Fact(DisplayName = "IZipkinTracer.Sampler.Sampling should be true when running ProbabilisticSampler")]
        public void TracerShouldIndicateSamplingIsActive()
        {
            Tracer.Sampler.Sampling.Should().BeTrue();
        }
    }
}