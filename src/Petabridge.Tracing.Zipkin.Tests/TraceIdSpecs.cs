// -----------------------------------------------------------------------
// <copyright file="TraceIdSpecs.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using FsCheck;
using FsCheck.Xunit;

namespace Petabridge.Tracing.Zipkin.Tests
{
    public class TraceIdSpecs
    {
        [Property(DisplayName = "Identical 128bit trace IDs should be equal")]
        public Property Identical128BitTraceIdsShouldBeEqual(long traceIdHigh, long traceIdLow)
        {
            var traceId1 = new TraceId(traceIdHigh, traceIdLow);
            var traceId2 = new TraceId(traceIdHigh, traceIdLow);

            return traceId1.Equals(traceId2).ToProperty()
                .And(traceId1 == traceId2)
                .And(!(traceId1 != traceId2));
        }

        [Property(DisplayName = "Identical 64bit trace IDs should be equal")]
        public Property Identical64BitTraceIdsShouldBeEqual(long traceIdLow)
        {
            var traceId1 = new TraceId(traceIdLow);
            var traceId2 = new TraceId(traceIdLow);

            return traceId1.Equals(traceId2).ToProperty()
                .And(traceId1 == traceId2)
                .And(!(traceId1 != traceId2));
        }

        [Property(DisplayName = "Should be able to parse 128bit TraceIds from their hex representation")]
        public Property ShouldParse128BitTraceIds(long traceIdHigh, long traceIdLow)
        {
            var traceId1 = new TraceId(traceIdHigh, traceIdLow);
            var parsed = TraceId.TryParse(traceId1.ToString(), out var traceId2);

            return parsed.Label($"Should have been able to parse {traceId1}")
                .And(traceId2.Equals(traceId1)
                    .Label("Expected a trace parsed from an original trace to be equal to the original"));
        }

        [Property(DisplayName = "Should be able to parse 64bit TraceIds from their hex representation")]
        public Property ShouldParse64BitTraceIds(long traceIdLow)
        {
            var traceId1 = new TraceId(traceIdLow);
            var parsed = TraceId.TryParse(traceId1.ToString(), out var traceId2);

            return parsed.Label($"Should have been able to parse {traceId1}")
                .And(traceId2.Equals(traceId1)
                    .Label("Expected a trace parsed from an original trace to be equal to the original"));
        }
    }
}