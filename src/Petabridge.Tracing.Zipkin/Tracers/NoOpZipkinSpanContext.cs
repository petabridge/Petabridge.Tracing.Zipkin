// -----------------------------------------------------------------------
// <copyright file="NoOpZipkinSpanContext.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace Petabridge.Tracing.Zipkin.Tracers
{
    /// <inheritdoc />
    /// <summary>
    ///     Used when there's no active span context.
    /// </summary>
    internal sealed class NoOpZipkinSpanContext : IZipkinSpanContext
    {
        public static readonly NoOpZipkinSpanContext Instance = new NoOpZipkinSpanContext();

        public static readonly IEnumerable<KeyValuePair<string, string>> Empty = new Dictionary<string, string>();

        private NoOpZipkinSpanContext()
        {
        }

        public IEnumerable<KeyValuePair<string, string>> GetBaggageItems()
        {
            return Empty;
        }

        public bool Equals(IZipkinSpanContext other)
        {
            return ReferenceEquals(other, Instance);
        }

        public TraceId ZipkinTraceId => new TraceId(0, 0);
        public string TraceId => ZipkinTraceId.ToString();
        public string SpanId => "0";
        public string ParentId => null;
        public bool Debug => true;
        public bool Sampled => false;
        public bool Shared => true;
    }
}