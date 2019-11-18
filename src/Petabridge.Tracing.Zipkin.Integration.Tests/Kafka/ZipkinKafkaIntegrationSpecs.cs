// -----------------------------------------------------------------------
// <copyright file="ZipkinKafkaIntegrationSpecs.cs" company="Petabridge, LLC">
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
using Petabridge.Tracing.Zipkin.Reporting.Kafka;
using Xunit;
using Xunit.Abstractions;

namespace Petabridge.Tracing.Zipkin.Integration.Tests.Kafka
{
    public class ZipkinKafkaIntegrationSpecs : TestKit, IClassFixture<ZipkinKafkaFixture>
    {
        public ZipkinKafkaIntegrationSpecs(ITestOutputHelper helper, ZipkinKafkaFixture fixture) : base(output: helper)
        {
            _appName = Sys.Name + ZipkinAppCounter.IncrementAndGet();
            Tracer = new ZipkinTracer(new ZipkinTracerOptions(
                new Endpoint(_appName), ZipkinKafkaSpanReporter.Create(fixture.KafkaUri, Sys))
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

        [Fact(DisplayName = "End2End: Should be able to post Trace to Zipkin via Kafka")]
        public async Task ShouldPostTraceToZipkin()
        {
            HttpResponseMessage traceResp = null;
            var retries = 3;
            for (var i = 1; i <= retries; i++)
            {
                /*
                 * Might have to try re-uploading the entire span. Takes Kafka a bit longer to spin up than stand-alone HTTP.
                 *
                 */
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