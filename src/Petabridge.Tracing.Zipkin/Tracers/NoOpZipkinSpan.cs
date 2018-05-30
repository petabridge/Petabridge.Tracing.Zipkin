using System;
using System.Collections.Generic;
using System.Text;
using OpenTracing;
using OpenTracing.Tag;

namespace Petabridge.Tracing.Zipkin.Tracers
{
    /// <inheritdoc />
    /// <summary>
    ///     INTERNAL API.
    ///     Used when there's no scope available.
    /// </summary>
    public sealed class NoOpZipkinSpan : IZipkinSpan
    {
        public static readonly NoOpZipkinSpan Instance = new NoOpZipkinSpan();

        private NoOpZipkinSpan()
        {
        }

        public ISpan SetTag(string key, string value)
        {
            return this;
        }

        public ISpan SetTag(string key, bool value)
        {
            return this;
        }

        public ISpan SetTag(string key, int value)
        {
            return this;
        }

        public ISpan SetTag(string key, double value)
        {
            return this;
        }

        public ISpan SetTag(BooleanTag tag, bool value)
        {
            return this;
        }

        public ISpan SetTag(IntOrStringTag tag, string value)
        {
            return this;
        }

        public ISpan SetTag(IntTag tag, int value)
        {
            return this;
        }

        public ISpan SetTag(StringTag tag, string value)
        {
            return this;
        }

        public ISpan Log(IEnumerable<KeyValuePair<string, object>> fields)
        {
            return this;
        }

        public ISpan Log(DateTimeOffset timestamp, IEnumerable<KeyValuePair<string, object>> fields)
        {
            return this;
        }

        public ISpan Log(IDictionary<string, object> fields)
        {
            return this;
        }

        public ISpan Log(DateTimeOffset timestamp, IDictionary<string, object> fields)
        {
            return this;
        }

        public ISpan Log(string @event)
        {
            return this;
        }

        public ISpan Log(DateTimeOffset timestamp, string @event)
        {
            return this;
        }

        public ISpan SetBaggageItem(string key, string value)
        {
            return this;
        }

        public string GetBaggageItem(string key)
        {
            return string.Empty;
        }

        public ISpan SetOperationName(string operationName)
        {
            return this;
        }

        public void Finish()
        {
        }

        public void Finish(DateTimeOffset finishTimestamp)
        {
        }

        public ISpanContext Context => NoOpZipkinSpanContext.Instance;
        public IZipkinSpanContext TypedContext => NoOpZipkinSpanContext.Instance;
        public bool Debug => TypedContext.Debug;
        public bool Shared => TypedContext.Shared;
        public bool Sampled => TypedContext.Sampled;
        public SpanKind? SpanKind => null;
    }
}
