using System;
using Petabridge.Tracing.Zipkin.Reporting.Kafka;

namespace Petabridge.Tracing.Zipkin.KafkaDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var url = "http://localhost:9411";
            var tracer = new ZipkinTracer(new ZipkinTracerOptions(new Endpoint("AaronsAppKafka"), ZipkinKafkaSpanReporter.Create(new ZipkinKafkaReportingOptions(new[]{ "localhost:9092" }, debugLogging:true))));
            Console.WriteLine("Connected to Zipkin at {0}", url);
            Console.WriteLine("Type some gibberish and press enter to create a trace!");
            Console.WriteLine("Type '/exit to quit.");
            var line = Console.ReadLine();
            IZipkinSpan current = null;
            while (string.IsNullOrEmpty(line) || !line.Equals("/exit"))
            {
                IZipkinSpanBuilder sb = null;
                if (string.IsNullOrEmpty(line))
                {
                    sb = tracer.BuildSpan("no-op").WithTag("empty", true);
                    if (current != null)
                        current.Finish();
                }
                else
                {
                    sb = tracer.BuildSpan("actual-op").WithTag("empty", false);
                    if (current != null)
                    {
                        current.Finish();
                        sb = sb.AsChildOf(current);
                    }
                }

                current = sb.Start();

                if (!string.IsNullOrEmpty(line))
                    current.Log(line);

                line = Console.ReadLine();
            }

            current?.Finish();

            tracer.Dispose();
        }
    }
}
