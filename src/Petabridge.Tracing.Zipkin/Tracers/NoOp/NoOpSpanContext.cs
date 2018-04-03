// -----------------------------------------------------------------------
// <copyright file="NoOpSpanContext.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Petabridge.Tracing.Zipkin;

namespace Phobos.Tracing.Zipkin
{
    /// <summary>
    ///     Used when there's no active span context.
    /// </summary>
    public sealed class NoOpSpanContext : IZipkinSpanContext
    {
        public static readonly NoOpSpanContext Instance = new NoOpSpanContext();

        public static readonly IEnumerable<KeyValuePair<string, string>> Empty = new Dictionary<string, string>();

        private NoOpSpanContext()
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

        public TraceId TraceId => new TraceId(0, 0);
        public long SpanId => 0;
        public long? ParentId => null;
        public bool Debug => true;
        public bool Sampled => false;
        public bool Shared => true;
    }
}