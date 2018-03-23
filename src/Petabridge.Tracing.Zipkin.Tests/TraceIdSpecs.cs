using System;
using FsCheck;
using FsCheck.Xunit;
using Xunit;

namespace Petabridge.Tracing.Zipkin.Tests
{
    public class TraceIdSpecs
    {
        [Property(DisplayName = "Identical trace IDs should be equal")]
        public Property IdenticalTraceIdsShouldBeEqual(long traceIdHigh, long traceIdLow)
        {
            var traceId1 = new TraceId(traceIdHigh, traceIdLow);
            var traceId2 = new TraceId(traceIdHigh, traceIdLow);

            return traceId1.Equals(traceId2).ToProperty()
                .And(traceId1 == traceId2)
                .And(!(traceId1 != traceId2));
        }
    }
}
