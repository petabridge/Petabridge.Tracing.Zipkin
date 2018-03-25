using System.Collections.Generic;
using OpenTracing;

namespace Phobos.Tracing.Zipkin
{
    /// <summary>
    /// Used when there's no active span context.
    /// </summary>
    public sealed class NoOpSpanContext : ISpanContext
    {
        public static readonly NoOpSpanContext Instance = new NoOpSpanContext();
        private NoOpSpanContext() { }

        public static readonly IEnumerable<KeyValuePair<string, string>> Empty = new Dictionary<string, string>();

        public IEnumerable<KeyValuePair<string, string>> GetBaggageItems()
        {
            return Empty;
        }
    }
}