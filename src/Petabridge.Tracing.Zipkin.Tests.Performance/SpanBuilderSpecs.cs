// -----------------------------------------------------------------------
// <copyright file="SpanBuilderSpecs.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using NBench;
using Petabridge.Tracing.Zipkin.Tracers;

namespace Petabridge.Tracing.Zipkin.Tests.Performance
{
    public class SpanBuilderSpecs
    {
        public const string CounterName = "CompletedSpans";

        public const int SpanCount = 100000;

        private readonly MockZipkinTracer _mockTracer = new MockZipkinTracer();
        private Counter _opsCounter;

        [PerfSetup]
        public void Setup(BenchmarkContext context)
        {
            _opsCounter = context.GetCounter(CounterName);
        }

        [PerfBenchmark(NumberOfIterations = 5, RunMode = RunMode.Iterations, RunTimeMilliseconds = 1000)]
        [CounterMeasurement(CounterName)]
        [GcMeasurement(GcMetric.TotalCollections, GcGeneration.AllGc)]
        [MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
        public void CreateSpans()
        {
            for (var i = 0; i < SpanCount; i++)
                using (_mockTracer.BuildSpan("test1").WithTag("foo", "bar").StartActive())
                {
                   
                }

            _opsCounter.Increment(SpanCount);
        }
    }
}