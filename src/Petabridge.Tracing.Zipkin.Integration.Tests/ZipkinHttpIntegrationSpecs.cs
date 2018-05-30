// -----------------------------------------------------------------------
// <copyright file="ZipkinHttpIntegrationSpecs.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Akka.TestKit.Xunit2;
using Akka.Util.Internal;
using Petabridge.Tracing.Zipkin.Reporting.Http;
using Xunit;
using Xunit.Abstractions;

namespace Petabridge.Tracing.Zipkin.Integration.Tests
{
    public class ZipkinHttpIntegrationSpecs : TestKit, IClassFixture<ZipkinFixture>
    {
        public ZipkinHttpIntegrationSpecs(ITestOutputHelper helper, ZipkinFixture fixture) : base(output: helper)
        {
            _appName = Sys.Name + ZipkinAppCounter.IncrementAndGet();
            Tracer = new ZipkinTracer(new ZipkinTracerOptions(
                new Endpoint(_appName),
                ZipkinHttpSpanReporter.Create(new ZipkinHttpReportingOptions(fixture.ZipkinUrl), Sys)));

            _zipkinClient = new HttpClient {BaseAddress = new Uri($"http://{fixture.ZipkinUrl}")};
        }

        private readonly HttpClient _zipkinClient;
        private readonly string _appName;
        private static readonly AtomicCounter ZipkinAppCounter = new AtomicCounter(0);

        protected ZipkinTracer Tracer { get; }

        protected override void AfterAll()
        {
            Tracer?.Dispose();
        }

        [Fact(DisplayName = "Should be able to post Trace to Zipkin")]
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

                traceId = active.Span.Context.AsInstanceOf<IZipkinSpanContext>().TraceId.ToString();
            }

            await Task.Delay(1000); // give it some time to get uploaded

            var traceResp =
                await _zipkinClient.GetStringAsync($"api/v2/trace/{traceId}");
        }
    }
}