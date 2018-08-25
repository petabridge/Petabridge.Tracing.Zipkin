using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using Phobos.Actor.Common;

namespace Petabridge.Tracing.Zipkin.Reporting.Kafka
{
    /// <summary>
    /// INTERNAL API
    ///  Actor responsible for managing the batching of outbound <see cref="T:Petabridge.Tracing.Zipkin.Span" /> instances.
    /// </summary>
    internal sealed class KafkaReportingActor : ReceiveActor, INeverMonitor, INeverTrace
    {
    }
}
