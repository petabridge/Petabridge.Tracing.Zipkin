// -----------------------------------------------------------------------
// <copyright file="SpanContextSpecs.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Xunit;

namespace Petabridge.Tracing.Zipkin.Tests
{
    public class SpanContextSpecs
    {
        [Fact(DisplayName = "If SpanContext.Debug is true, then SpanContext.Sampled should always be false")]
        public void SpanContextSampledShouldAlwaysBeFalseWhenDebugEnabled()
        {
            // explicitly set both DEBUG and SAMPLED to true
            var spanContext = new SpanContext(new TraceId(90, 0), 1, null, true, true);
            spanContext.Debug.Should().BeTrue();
            spanContext.Sampled.Should().BeFalse();
        }
    }
}