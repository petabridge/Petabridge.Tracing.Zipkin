// -----------------------------------------------------------------------
// <copyright file="B3SingleHeaderFormatterSpecs.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2019 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

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

        [Fact(DisplayName = "Should parse 64bit B3 single format header")]
        public void ShouldParseB3SingleHeader()
        {
            var context = new SpanContext(new TraceId(1), 3);

            var b3Header = WriteB3SingleFormat(context);
            var b3HeaderParsed = ParseB3SingleFormat(b3Header);

            b3HeaderParsed.Should().Be(context);
            b3HeaderParsed.Sampled.Should().Be(context.Sampled);
            b3HeaderParsed.Debug.Should().Be(context.Debug);
        }

        [Fact(DisplayName = "Should parse 128bit B3 single format header")]
        public void ShouldParseB3SingleHeader128Bit()
        {
            var context = new SpanContext(new TraceId(1, 1), 3);

            var b3Header = WriteB3SingleFormat(context);
            var b3HeaderParsed = ParseB3SingleFormat(b3Header);

            b3HeaderParsed.Should().Be(context);
            b3HeaderParsed.Sampled.Should().Be(context.Sampled);
            b3HeaderParsed.Debug.Should().Be(context.Debug);
        }

        [Fact(DisplayName = "Should parse 128bit B3 single format header with parentId only")]
        public void ShouldParseB3SingleHeaderParentIdOnly128Bit()
        {
            var context = new SpanContext(new TraceId(1, 1), 3, "1aae83b1d7e325ce");

            var b3Header = WriteB3SingleFormat(context);
            var b3HeaderParsed = ParseB3SingleFormat(b3Header);

            b3HeaderParsed.Should().Be(context);
            b3HeaderParsed.Sampled.Should().Be(context.Sampled);
            b3HeaderParsed.Debug.Should().Be(context.Debug);
        }

        [Fact(DisplayName = "Should parse 64bit B3 single format header with debug")]
        public void ShouldParseB3SingleHeaderWithDebug()
        {
            var context = new SpanContext(new TraceId(1), 3, debug: true);

            var b3Header = WriteB3SingleFormat(context);
            var b3HeaderParsed = ParseB3SingleFormat(b3Header);

            b3HeaderParsed.Should().Be(context);
            b3HeaderParsed.Sampled.Should().Be(context.Sampled);
            b3HeaderParsed.Debug.Should().Be(context.Debug);
        }

        [Fact(DisplayName = "Should parse 128bit B3 single format header with debug")]
        public void ShouldParseB3SingleHeaderWithDebug128Bit()
        {
            var context = new SpanContext(new TraceId(1, 1), 3, debug: true);

            var b3Header = WriteB3SingleFormat(context);
            var b3HeaderParsed = ParseB3SingleFormat(b3Header);

            b3HeaderParsed.Should().Be(context);
            b3HeaderParsed.Sampled.Should().Be(context.Sampled);
            b3HeaderParsed.Debug.Should().Be(context.Debug);
        }

        [Fact(DisplayName = "Should parse 64bit B3 single format header with parent id")]
        public void ShouldParseB3SingleHeaderWithParentId()
        {
            var context = new SpanContext(new TraceId(1), 3, Tracer.IdProvider.NextSpanId());

            var b3Header = WriteB3SingleFormat(context);
            var b3HeaderParsed = ParseB3SingleFormat(b3Header);

            b3HeaderParsed.Should().Be(context);
            b3HeaderParsed.Sampled.Should().Be(context.Sampled);
            b3HeaderParsed.Debug.Should().Be(context.Debug);
        }

        [Fact(DisplayName = "Should parse 64bit B3 single format header with sampling")]
        public void ShouldParseB3SingleHeaderWithSampling()
        {
            var context = new SpanContext(new TraceId(1), 3, sampled: true);

            var b3Header = WriteB3SingleFormat(context);
            var b3HeaderParsed = ParseB3SingleFormat(b3Header);

            b3HeaderParsed.Should().Be(context);
            b3HeaderParsed.Sampled.Should().Be(context.Sampled);
            b3HeaderParsed.Debug.Should().Be(context.Debug);
        }

        [Fact(DisplayName = "Should parse 128bit B3 single format header with sampling")]
        public void ShouldParseB3SingleHeaderWithSampling128Bit()
        {
            var context = new SpanContext(new TraceId(1, 1), 3, sampled: true);

            var b3Header = WriteB3SingleFormat(context);
            var b3HeaderParsed = ParseB3SingleFormat(b3Header);

            b3HeaderParsed.Should().Be(context);
            b3HeaderParsed.Sampled.Should().Be(context.Sampled);
            b3HeaderParsed.Debug.Should().Be(context.Debug);
        }

        [Fact(DisplayName = "Should parse 64bit B3 single format header with sampling and parent id")]
        public void ShouldParseB3SingleHeaderWithSamplingAndParentId()
        {
            var context = new SpanContext(new TraceId(1), 3, Tracer.IdProvider.NextSpanId(), sampled: true);

            var b3Header = WriteB3SingleFormat(context);
            var b3HeaderParsed = ParseB3SingleFormat(b3Header);

            b3HeaderParsed.Should().Be(context);
            b3HeaderParsed.Sampled.Should().Be(context.Sampled);
            b3HeaderParsed.Debug.Should().Be(context.Debug);
        }


        [Fact(DisplayName = "Should parse 128bit B3 single format header with sampling and parent id (max length)")]
        public void ShouldParseB3SingleHeaderWithSamplingAndParentId128Bit()
        {
            var context = new SpanContext(new TraceId(1, 1), 3, Tracer.IdProvider.NextSpanId(), sampled: true);

            var b3Header = WriteB3SingleFormat(context);
            var b3HeaderParsed = ParseB3SingleFormat(b3Header);

            b3HeaderParsed.Should().Be(context);
            b3HeaderParsed.Sampled.Should().Be(context.Sampled);
            b3HeaderParsed.Debug.Should().Be(context.Debug);
        }

        [Fact(DisplayName = "Should write 64bit B3 single format header with debug")]
        public void ShouldWriteB3SingleHeaderWithDebug()
        {
            var context = new SpanContext(new TraceId(1), 3, debug: true);

            var b3Header = WriteB3SingleFormat(context);
            b3Header.Should().Be(context.TraceId + "-" + context.SpanId + "-d");
            b3Header.Should().Be(Encoding.UTF8.GetString(WriteB3SingleFormatAsBytes(context)));
        }

        [Fact(DisplayName = "Should write 64bit B3 single format header without sampling")]
        public void ShouldWriteB3SingleHeaderWithoutSampling()
        {
            var context = new SpanContext(new TraceId(1), 3);

            var b3Header = WriteB3SingleFormat(context);
            b3Header.Should().Be(context.TraceId + "-" + context.SpanId);
            b3Header.Should().Be(Encoding.UTF8.GetString(WriteB3SingleFormatAsBytes(context)));
        }

        [Fact(DisplayName = "Should write 128bit B3 single format header without sampling")]
        public void ShouldWriteB3SingleHeaderWithoutSampling128Bit()
        {
            var context = new SpanContext(new TraceId(1, 1), 3);

            var b3Header = WriteB3SingleFormat(context);
            b3Header.Should().Be(context.TraceId + "-" + context.SpanId);
            b3Header.Should().Be(Encoding.UTF8.GetString(WriteB3SingleFormatAsBytes(context)));
        }

        [Fact(DisplayName = "Should write 64bit B3 single format header with a parent id and sampling")]
        public void ShouldWriteB3SingleHeaderWithParentAndSampling()
        {
            var context = new SpanContext(new TraceId(1), 3, Tracer.IdProvider.NextSpanId(), sampled: true);

            var b3Header = WriteB3SingleFormat(context);
            b3Header.Should().Be(context.TraceId + "-" + context.SpanId + "-1-" + context.ParentId);
            b3Header.Should().Be(Encoding.UTF8.GetString(WriteB3SingleFormatAsBytes(context)));
        }

        [Fact(DisplayName = "Should write 64bit B3 single format header with a parent id only")]
        public void ShouldWriteB3SingleHeaderWithParentIdOnly()
        {
            var context = new SpanContext(new TraceId(1), 3, "18974c44954cf23f");

            var b3Header = WriteB3SingleFormat(context);
            b3Header.Should().Be(context.TraceId + "-" + context.SpanId + "-" + context.ParentId);
            b3Header.Should().Be(Encoding.UTF8.GetString(WriteB3SingleFormatAsBytes(context)));
        }

        [Fact(DisplayName = "Should write 128bit B3 single format header with parent id only")]
        public void ShouldWriteB3SingleHeaderWithParentIdOnly128Bit()
        {
            var context = new SpanContext(new TraceId(1, 1), 3, Tracer.IdProvider.NextSpanId());

            var b3Header = WriteB3SingleFormat(context);
            b3Header.Should().Be(context.TraceId + "-" + context.SpanId + "-" + context.ParentId);
            b3Header.Should().Be(Encoding.UTF8.GetString(WriteB3SingleFormatAsBytes(context)));
        }

        [Fact(DisplayName = "Should write 64bit B3 single format header with sampling")]
        public void ShouldWriteB3SingleHeaderWithSampling()
        {
            var context = new SpanContext(new TraceId(1), 3, sampled: true);

            var b3Header = WriteB3SingleFormat(context);
            b3Header.Should().Be(context.TraceId + "-" + context.SpanId + "-1");
            b3Header.Should().Be(Encoding.UTF8.GetString(WriteB3SingleFormatAsBytes(context)));
        }

        [Fact(DisplayName = "Should write 128bit B3 single format header with sampling")]
        public void ShouldWriteB3SingleHeaderWithSampling128Bit()
        {
            var context = new SpanContext(new TraceId(1, 1), 3, sampled: true);

            var b3Header = WriteB3SingleFormat(context);
            b3Header.Should().Be(context.TraceId + "-" + context.SpanId + "-1");
            b3Header.Should().Be(Encoding.UTF8.GetString(WriteB3SingleFormatAsBytes(context)));
        }

        [Fact(DisplayName = "Should write 128bit B3 single format header with sampling and parent id (max length)")]
        public void ShouldWriteB3SingleHeaderWithSamplingAndParentId128Bit()
        {
            var context = new SpanContext(new TraceId(1, 1), 3, Tracer.IdProvider.NextSpanId(), sampled: true);

            var b3Header = WriteB3SingleFormat(context);
            b3Header.Should().Be(context.TraceId + "-" + context.SpanId + "-1-" + context.ParentId);
            b3Header.Should().Be(Encoding.UTF8.GetString(WriteB3SingleFormatAsBytes(context)));
        }
    }
}