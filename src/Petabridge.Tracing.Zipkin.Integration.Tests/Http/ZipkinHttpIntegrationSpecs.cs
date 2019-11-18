// -----------------------------------------------------------------------
// <copyright file="ZipkinHttpIntegrationSpecs.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Akka.TestKit.Xunit2;
using Akka.Util.Internal;
using FluentAssertions;
using OpenTracing.Util;
using Petabridge.Tracing.Zipkin.Integration.Tests.Serialization;
using Petabridge.Tracing.Zipkin.Reporting.Http;
using Petabridge.Tracing.Zipkin.Reporting.NoOp;
using Xunit;
using Xunit.Abstractions;

namespace Petabridge.Tracing.Zipkin.Integration.Tests.Http
{
    public class ZipkinHttpIntegrationSpecs : TestKit, IClassFixture<ZipkinFixture>
    {
        public ZipkinHttpIntegrationSpecs(ITestOutputHelper helper, ZipkinFixture fixture) : base(output: helper)
        {
            _appName = Sys.Name + ZipkinAppCounter.IncrementAndGet();
            Tracer = new ZipkinTracer(new ZipkinTracerOptions(
                new Endpoint(_appName),
                ZipkinHttpSpanReporter.Create(new ZipkinHttpReportingOptions($"http://{fixture.ZipkinUrl}"), Sys))
            {
                ScopeManager = new AsyncLocalScopeManager()
            });

            _httpBaseUri = new Uri($"http://{fixture.ZipkinUrl}/");
            _zipkinClient = new HttpClient();
        }

        private readonly Uri _httpBaseUri;
        private readonly HttpClient _zipkinClient;
        private readonly string _appName;
        private static readonly AtomicCounter ZipkinAppCounter = new AtomicCounter(0);

        protected ZipkinTracer Tracer { get; }

        protected override void AfterAll()
        {
            Tracer?.Dispose();
        }

        [Fact(DisplayName = "HttpReporter: Should be able to post Trace to Zipkin")]
        public async Task HttpReporterShouldPostCorrectlyToZipkin()
        {
            var testTracer = new ZipkinTracer(new ZipkinTracerOptions(
                new Endpoint(_appName), new NoOpReporter())
            {
                ScopeManager = new AsyncLocalScopeManager()
            });

            string traceId;
            Span parentSpan = null;
            Span childSpan = null;

            using (var parentScope = testTracer.BuildSpan("parent").StartActive())
            {
                parentSpan = (Span) parentScope.Span;
                using (var childScope = testTracer.BuildSpan("child").StartActive())
                {
                    childSpan = (Span) childScope.Span;
                }

                traceId = parentSpan.TypedContext.TraceId;
            }

            // sanity check
            parentSpan.TypedContext.ParentId.Should().BeNullOrEmpty();
            childSpan.TypedContext.ParentId.Should().Be(parentSpan.Context.SpanId);

            // create an HTTP reporting engine
            var httpReporter = new ZipkinHttpApiTransmitter(_zipkinClient,
                ZipkinHttpApiTransmitter.GetFullZipkinUri(_httpBaseUri.AbsoluteUri));

            // manually transmit data to Zipkin for parent span
            var resp1 = await httpReporter.TransmitSpans(new[] {parentSpan}, TimeSpan.FromSeconds(3));
            resp1.IsSuccessStatusCode.Should().BeTrue(
                $"Expected success status code, but instead found [{resp1.StatusCode}][{resp1.ReasonPhrase}]");

            // manually transmit data to Zipkin for child span
            var resp2 = await httpReporter.TransmitSpans(new[] {childSpan}, TimeSpan.FromSeconds(3));
            resp2.IsSuccessStatusCode.Should().BeTrue(
                $"Expected success status code, but instead found [{resp2.StatusCode}][{resp2.ReasonPhrase}]");

            var fullUri = new Uri(_httpBaseUri, $"api/v2/trace/{traceId}");
            HttpResponseMessage traceResp = null;
            var retries = 3;
            for (var i = 1; i <= retries; i++)
                try
                {
                    await Task.Delay(1000); // give it some time to get uploaded
                    traceResp =
                        await _zipkinClient.GetAsync(fullUri);

                    traceResp.IsSuccessStatusCode.Should()
                        .BeTrue(
                            $"Expected success status code, but instead found [{traceResp.StatusCode}][{traceResp.ReasonPhrase}]");
                }
                catch (Exception ex)
                {
                    if (i == retries)
                        throw;
                }

            var json = await traceResp.Content.ReadAsStringAsync();
            var traces = ZipkinDeserializer.Deserialize(json);

            traces.Count.Should().Be(2);
        }

        [Fact(DisplayName = "End2End: Should be able to post Trace to Zipkin via HTTP")]
        public async Task ShouldPostTraceToZipkin()
        {
            string traceId;
            using (var active = Tracer.BuildSpan("bar").WithTag("foo", true).WithTag("akka.net", "1.3.8")
                .StartActive(true))
            {
                active.Span.Log("this is an event");
                using (var active2 = Tracer.BuildSpan("foo").StartActive(true))
                {
                    active2.Span.Log("This is a nested span");
                }

                traceId = active.Span.Context.AsInstanceOf<IZipkinSpanContext>().TraceId;
            }

            var fullUri = new Uri(_httpBaseUri, $"api/v2/trace/{traceId}");
            HttpResponseMessage traceResp = null;
            var retries = 3;
            for (var i = 1; i <= retries; i++)
                try
                {
                    await Task.Delay(1000); // give it some time to get uploaded
                    traceResp =
                        await _zipkinClient.GetAsync(fullUri);

                    traceResp.IsSuccessStatusCode.Should()
                        .BeTrue(
                            $"Expected success status code, but instead found [{traceResp.StatusCode}][{traceResp.ReasonPhrase}]");
                }
                catch (Exception ex)
                {
                    if (i == retries)
                        throw;
                }


            var json = await traceResp.Content.ReadAsStringAsync();
            var traces = ZipkinDeserializer.Deserialize(json);

            traces.Count.Should().Be(2);
            var barSpan = traces.Single(x => x.name.Equals("bar"));
            var fooSpan = traces.Single(x => x.name.Equals("foo"));
            fooSpan.parentId.Should().Be(barSpan.id);
        }
    }
}