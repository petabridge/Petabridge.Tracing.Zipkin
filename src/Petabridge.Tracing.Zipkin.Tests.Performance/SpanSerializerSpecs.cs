using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBench;
using Petabridge.Tracing.Zipkin.Reporting;
using Petabridge.Tracing.Zipkin.Tracers;

namespace Petabridge.Tracing.Zipkin.Tests.Performance
{
    public class SpanSerializerSpecs
    {
        public const string CounterName = "SerializedSpans";

        public const int SpanCount = 100000;

        private readonly MockZipkinTracer _mockTracer = new MockZipkinTracer(new Endpoint("actorsystem", "127.0.0.1", 8008));
        private readonly ISpanSerializer _serializer = new JsonSpanSerializer();
        private Span _testSpan;
        private Counter _opsCounter;

        [PerfSetup]
        public void Setup(BenchmarkContext context)
        {
            _opsCounter = context.GetCounter(CounterName);

            var startTime = new DateTimeOffset(new DateTime(2018, 1, 12, 13, 12, 14));
            var endTime = startTime.AddMilliseconds(10);

            // create a reasonably complex span
            var span = new Span(_mockTracer, "op1",
                    new SpanContext(new TraceId(7776525154056436086, 6707114971141086261), "-7118946577185884628", null,
                        true),
                    startTime, SpanKind.CLIENT)
                .SetRemoteEndpoint(new Endpoint("actorsystem", "127.0.0.1", 8009)).SetTag("foo1", "bar")
                .SetTag("numberOfPets", 2)
                .SetTag("timeInChair", "long").Log(startTime.AddMilliseconds(1), "foo");
            _testSpan = (Span) span;
            _testSpan.Finish(endTime);
        }

        [PerfBenchmark(NumberOfIterations = 5, RunMode = RunMode.Iterations, RunTimeMilliseconds = 1000)]
        [CounterMeasurement(CounterName)]
        [GcMeasurement(GcMetric.TotalCollections, GcGeneration.AllGc)]
        [MemoryMeasurement(MemoryMetric.TotalBytesAllocated)]
        public void SerializeSpans()
        {
            for (var i = 0; i < SpanCount; i++)
                using(var mem = new MemoryStream())
                _serializer.Serialize(mem, _testSpan);

            _opsCounter.Increment(SpanCount);
        }
    }
}
