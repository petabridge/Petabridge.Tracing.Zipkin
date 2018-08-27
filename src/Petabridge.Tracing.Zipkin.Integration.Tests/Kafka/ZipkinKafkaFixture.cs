using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Docker.DotNet;
using Xunit;

namespace Petabridge.Tracing.Zipkin.Integration.Tests.Kafka
{
    public class ZipkinKafkaFixture : ZipkinFixture
    {
        protected const string KafkaImageName = "openzipkin/kafaka";
    }
}
