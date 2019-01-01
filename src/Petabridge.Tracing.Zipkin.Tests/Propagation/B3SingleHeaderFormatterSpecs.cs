using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Petabridge.Tracing.Zipkin.Propagation;
using Petabridge.Tracing.Zipkin.Tracers;
using Xunit;
using static Petabridge.Tracing.Zipkin.Propagation.B3SingleHeaderFormatter;

namespace Petabridge.Tracing.Zipkin.Tests.Propagation
{
    public class B3SingleHeaderFormatterSpecs
    {
        public B3SingleHeaderFormatterSpecs()
        {
            Tracer = new MockZipkinTracer(propagtor: new B3Propagator());
        }

        public readonly MockZipkinTracer Tracer;

        [Fact(DisplayName = "Should write 64bit B3 single format header without sampling")]
        public void ShouldWriteB3SingleHeaderWithoutSampling()
        {
            var context = new SpanContext(new TraceId(1), 3);

            var b3Header = WriteB3SingleFormat(context);
            b3Header.Should().Be(context.TraceId + "-" + context.SpanId);
            b3Header.Should().Be(Encoding.UTF8.GetString(WriteB3SingleFormatAsBytes(context)));
        }

        [Fact(DisplayName = "Should write 64bit B3 single format header with sampling")]
        public void ShouldWriteB3SingleHeaderWithSampling()
        {
            var context = new SpanContext(new TraceId(1), 3, sampled:true);

            var b3Header = WriteB3SingleFormat(context);
            b3Header.Should().Be(context.TraceId + "-" + context.SpanId + "-1");
            b3Header.Should().Be(Encoding.UTF8.GetString(WriteB3SingleFormatAsBytes(context)));
        }

        [Fact(DisplayName = "Should write 64bit B3 single format header with debug")]
        public void ShouldWriteB3SingleHeaderWithDebug()
        {
            var context = new SpanContext(new TraceId(1), 3, debug: true);

            var b3Header = WriteB3SingleFormat(context);
            b3Header.Should().Be(context.TraceId + "-" + context.SpanId + "-d");
            b3Header.Should().Be(Encoding.UTF8.GetString(WriteB3SingleFormatAsBytes(context)));
        }

        [Fact(DisplayName = "Should write 128bit B3 single format header without sampling")]
        public void ShouldWriteB3SingleHeaderWithoutSampling128Bit()
        {
            var context = new SpanContext(new TraceId(1,1), 3);

            var b3Header = WriteB3SingleFormat(context);
            b3Header.Should().Be(context.TraceId + "-" + context.SpanId);
            b3Header.Should().Be(Encoding.UTF8.GetString(WriteB3SingleFormatAsBytes(context)));
        }
    }
}
