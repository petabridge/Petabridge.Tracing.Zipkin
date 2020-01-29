using FluentAssertions;
using Xunit;

namespace Petabridge.Tracing.Zipkin.Tests
{
    public class TracerSpec
    {
        [Fact]
        public void Should_have_active_span_when_scope_is_used()
        {
            var url = "http://localhost:9411";
            using (var tracer = new ZipkinTracer(new ZipkinTracerOptions(url, "ZipkinTest", debug: true)))
            {
                using (var scope = tracer.BuildSpan("ShouldSendSpans").StartActive(finishSpanOnDispose: true))
                {
                    tracer.ActiveSpan.Should().NotBeNull();
                }

                tracer.ActiveSpan.Should().BeNull();
            }
        }
    }
}