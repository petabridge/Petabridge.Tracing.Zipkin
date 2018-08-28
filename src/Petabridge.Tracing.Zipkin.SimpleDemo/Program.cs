// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Petabridge.Tracing.Zipkin.SimpleDemo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var url = "http://localhost:9411";
            var tracer = new ZipkinTracer(new ZipkinTracerOptions(url, "AaronsApp", debug: true));
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