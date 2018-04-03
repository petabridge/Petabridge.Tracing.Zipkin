using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
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
            var spanContext = new SpanContext(new TraceId(90,0), 1, null, true, true);
            spanContext.Debug.Should().BeTrue();
            spanContext.Sampled.Should().BeFalse();
        }
    }
}
