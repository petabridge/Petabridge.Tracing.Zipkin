// -----------------------------------------------------------------------
// <copyright file="TraceIdSpecs.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using FsCheck;
using FsCheck.Xunit;

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