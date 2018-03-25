// -----------------------------------------------------------------------
// <copyright file="JsonSerializationSpecs.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Text;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Petabridge.Tracing.Zipkin.Reporting;
using Petabridge.Tracing.Zipkin.Tracers;
using Xunit;
using static Petabridge.Tracing.Zipkin.Reporting.JsonSpanSerializer;

namespace Petabridge.Tracing.Zipkin.Tests.Serialization
{
    public class JsonSerializationSpec : SerializationSpecBase
    {
        public JsonSerializationSpec()
        {
            Tracer = new MockZipkinTracer(new Endpoint("actorsystem", "127.0.0.1", 8008));
        }

        protected readonly MockZipkinTracer Tracer;

        public static void Assert(byte[] actual, byte[] expected)
        {
            var actualStr = Encoding.UTF8.GetString(actual);
            var expectedStr = Encoding.UTF8.GetString(expected);
            var actualObj = JArray.Parse(actualStr).Children<JObject>();
            var expectedObj = JArray
                .Parse(expectedStr, new JsonLoadSettings {LineInfoHandling = LineInfoHandling.Ignore})
                .Children<JObject>();
            var enum1 = actualObj.GetEnumerator();
            var enum2 = expectedObj.GetEnumerator();
            try
            {
                while (enum1.MoveNext() && enum2.MoveNext())
                {
                    var a1 = enum1.Current;
                    var a2 = enum2.Current;
                    a1[JsonSpanSerializer.TraceId].Equals(a2[JsonSpanSerializer.TraceId]).Should().BeTrue();
                    if (a1.ContainsKey(Kind) && a2.ContainsKey(Kind))
                        a1[Kind].Equals(a2[Kind]).Should().BeTrue();
                    else if (a1.ContainsKey(Kind) && !a2.ContainsKey(Kind) ||
                             !a1.ContainsKey(Kind) && a2.ContainsKey(Kind))
                        throw new Exception("SpanKind should be defined on both actual and expected or on neither.");

                    a1[SpanId].Equals(a2[SpanId]).Should().BeTrue();
                    a1[Timestamp].Equals(a2[Timestamp]).Should().BeTrue();
                    a1[Duration].Equals(a2[Duration]).Should().BeTrue();
                }
            }
            finally
            {
                enum1.Dispose();
                enum2.Dispose();
            }
        }

        [Fact(DisplayName = "Should be able to serialize span into a valid Zipkin-friendly JSON format.")]
        public void ShouldMapSingleSpanIntoValidJson()
        {
            var json = @"
                [
                  {
                    ""traceId"": ""6bebc46d026c79765d147587ca919c35"",
                    ""name"": ""op1"",
                    ""id"": ""9d346bce4bda362c"",
                    ""kind"": ""CLIENT"",
                    ""timestamp"": 1515784334000000,
                    ""duration"": 10000,
                    ""debug"": true,
                    ""localEndpoint"": {
                      ""serviceName"": ""actorsystem"",
                      ""ipv4"": ""127.0.0.1"",
                      ""ipv6"": ""127.0.0.1"",
                      ""port"": 8008
                    },
                    ""remoteEndpoint"": {
                      ""serviceName"": ""actorsystem"",
                      ""ipv4"": ""127.0.0.1"",
                      ""ipv6"": ""127.0.0.1"",
                      ""port"": 8009
                    },
                    ""annotations"": [
                      {
                        ""timestamp"": 1515784334001000,
                        ""value"": ""foo""
                      }
                    ],
                    ""tags"": {
                      ""foo1"": ""bar"",
                      ""numberOfPets"": ""2"",
                      ""timeInChair"": ""long""
                    }
                  }
                ]";

            var startTime = new DateTimeOffset(new DateTime(2018, 1, 12, 13, 12, 14));
            var endTime = startTime.AddMilliseconds(10);
            var expectedBytes = Encoding.UTF8.GetBytes(json);

            var span = new Span(Tracer, "op1",
                    new SpanContext(new TraceId(7776525154056436086, 6707114971141086261), -7118946577185884628, null,
                        true),
                    startTime, SpanKind.CLIENT)
                .SetRemoteEndpoint(new Endpoint("actorsystem", "127.0.0.1", 8009)).SetTag("foo1", "bar")
                .SetTag("numberOfPets", 2)
                .SetTag("timeInChair", "long").Log(startTime.AddMilliseconds(1), "foo");
            span.Finish(endTime);

            VerifySerialization(new JsonSpanSerializer(), expectedBytes, (Span) span, Assert);
        }
    }
}