using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using OpenTracing;

namespace Petabridge.Tracing.Zipkin
{
    /// <summary>
    /// Builder interface for constructing new <see cref="Span"/> instances.
    /// </summary>
    public sealed class SpanBuilder : ISpanBuilder
    {
        /// <summary>
        /// INTERNAL API. Used to describe a relationship between <see cref="Span"/> instances.
        /// </summary>
        private struct SpanReference : IEquatable<SpanReference>
        {
            public SpanReference(string referenceType, ISpanContext spanContext)
            {
                ReferenceType = referenceType;
                SpanContext = spanContext;
            }

            public string ReferenceType { get; }

            public ISpanContext SpanContext { get; }

            public bool Equals(SpanReference other)
            {
                return string.Equals(ReferenceType, other.ReferenceType) 
                    && SpanContext.Equals(other.SpanContext);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is SpanReference && Equals((SpanReference) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (ReferenceType.GetHashCode() * 397) ^ SpanContext.GetHashCode();
                }
            }

            public static bool operator ==(SpanReference left, SpanReference right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(SpanReference left, SpanReference right)
            {
                return !left.Equals(right);
            }
        }

        private readonly IZipkinTracer _tracer;
        private List<SpanReference> _references;
        private Dictionary<string, string> _initialTags;
        private bool _ignoreActive;
        private bool _enableDebug;
        private DateTimeOffset? _start;
        private readonly string _operationName;

        public SpanBuilder(IZipkinTracer tracer, string operationName)
        {
            _tracer = tracer;
            _operationName = operationName;
        }

        public ISpanBuilder AsChildOf(ISpanContext parent)
        {
            return AddReference(References.ChildOf, parent);
        }

        public ISpanBuilder AsChildOf(ISpan parent)
        {
            return AsChildOf(parent.Context);
        }

        public ISpanBuilder AddReference(string referenceType, ISpanContext referencedContext)
        {
            if (_references == null) _references = new List<SpanReference>();
            _references.Add(new SpanReference(referenceType, referencedContext));
            return this;
        }

        public ISpanBuilder IgnoreActiveSpan()
        {
            _ignoreActive = true;
            return this;
        }

        public ISpanBuilder EnableDebugMode()
        {
            _enableDebug = true;
            return this;
        }

        public ISpanBuilder WithTag(string key, string value)
        {
            if(_initialTags == null) _initialTags = new Dictionary<string, string>();
            _initialTags[key] = value;
            return this;
        }

        public ISpanBuilder WithTag(string key, bool value)
        {
            return WithTag(key, Convert.ToString(value));
        }

        public ISpanBuilder WithTag(string key, int value)
        {
            return WithTag(key, Convert.ToString(value));
        }

        public ISpanBuilder WithTag(string key, double value)
        {
            return WithTag(key, Convert.ToString(value, CultureInfo.InvariantCulture));
        }

        public ISpanBuilder WithStartTimestamp(DateTimeOffset timestamp)
        {
            _start = timestamp;
            return this;
        }

        public IScope StartActive(bool finishSpanOnDispose)
        {
            return _tracer.ScopeManager.Activate(Start(), finishSpanOnDispose);
        }

        public ISpan Start()
        {
            if (_start == null)
            {
                _start = _tracer.TimeProvider.Now;
            }

            ISpanContext activeSpanContext = _tracer.ActiveSpan?.Context;
            SpanContext parentContext = null;

            if (_references != null && (_ignoreActive || activeSpanContext == null))
            {
                parentContext = FindBestFittingReference(_references);
            }
            else if(activeSpanContext != null)
            {
                parentContext = (SpanContext)activeSpanContext;
            }

            return new Span(_tracer, _operationName, new SpanContext(_tracer.IdProvider.NextTraceId(), _tracer.IdProvider.NextSpanId(), parentContext?.SpanId, debug:_enableDebug));
        }

        private static SpanContext FindBestFittingReference(IReadOnlyList<SpanReference> references)
        {
            foreach (var reference in references)
            {
                if (reference.ReferenceType.Equals(References.ChildOf))
                    return (SpanContext)reference.SpanContext;
            }

            return (SpanContext)references.First().SpanContext;
        }
    }
}
