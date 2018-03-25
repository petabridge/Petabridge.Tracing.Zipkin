using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Petabridge.Tracing.Zipkin.Tests.Serialization
{
    public class JsonSerializationSpec : SerializationSpecBase
    {
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


        public static void Assert(byte[] actual, byte[] expected)
        {
            var actualObj = JObject.Load(new JsonTextReader(new StreamReader(new MemoryStream(actual))));
            var expectedObj = JObject.Load(new JsonTextReader(new StreamReader(new MemoryStream(expected))));

            JToken.DeepEquals(actualObj, expectedObj).Should().BeTrue();
        }
    }
}