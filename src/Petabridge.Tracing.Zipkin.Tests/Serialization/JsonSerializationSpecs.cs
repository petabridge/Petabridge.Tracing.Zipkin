// -----------------------------------------------------------------------
// <copyright file="JsonSerializationSpecs.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System.IO;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Petabridge.Tracing.Zipkin.Tests.Serialization
{
    public class JsonSerializationSpec : SerializationSpecBase
    {
        public static void Assert(byte[] actual, byte[] expected)
        {
            var actualObj = JObject.Load(new JsonTextReader(new StreamReader(new MemoryStream(actual))));
            var expectedObj = JObject.Load(new JsonTextReader(new StreamReader(new MemoryStream(expected))));

            JToken.DeepEquals(actualObj, expectedObj).Should().BeTrue();
        }

        [Fact]
        public void ShouldMapSingleSpanIntoValidJson()
        {
            var json = @"
                [
                  {
                    ""traceId"": ""string"",
                    ""name"": ""string"",
                    ""parentId"": ""string"",
                    ""id"": ""string"",
                    ""kind"": ""CLIENT"",
                    ""timestamp"": 0,
                    ""duration"": 0,
                    ""debug"": true,
                    ""shared"": true,
                    ""localEndpoint"": {
                      ""serviceName"": ""string"",
                      ""ipv4"": ""string"",
                      ""ipv6"": ""string"",
                      ""port"": 0
                    },
                    ""remoteEndpoint"": {
                      ""serviceName"": ""string"",
                      ""ipv4"": ""string"",
                      ""ipv6"": ""string"",
                      ""port"": 0
                    },
                    ""annotations"": [
                      {
                        ""timestamp"": 0,
                        ""value"": ""string""
                      }
                    ],
                    ""tags"": {
                      ""additionalProp1"": ""string"",
                      ""additionalProp2"": ""string"",
                      ""additionalProp3"": ""string""
                    }
                  }
                ]";
        }
    }
}