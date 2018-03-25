// -----------------------------------------------------------------------
// <copyright file="SpanBuilder.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using OpenTracing;
using Petabridge.Tracing.Zipkin.Util;

namespace Petabridge.Tracing.Zipkin
{
    public interface IZipkinSpanBuilder : ISpanBuilder
    {
        new IZipkinSpanBuilder AsChildOf(ISpanContext parent);

        new IZipkinSpanBuilder AsChildOf(ISpan parent);

        new IZipkinSpanBuilder AddReference(string referenceType, ISpanContext referencedContext);

        new IZipkinSpanBuilder IgnoreActiveSpan();

        new IZipkinSpanBuilder WithTag(string key, string value);

        new IZipkinSpanBuilder WithTag(string key, bool value);

        new IZipkinSpanBuilder WithTag(string key, int value);

        new IZipkinSpanBuilder WithTag(string key, double value);

        new IZipkinSpanBuilder WithStartTimestamp(DateTimeOffset timestamp);

        new Span Start();

        IZipkinSpanBuilder WithSpanKind(SpanKind spanKind);

        IZipkinSpanBuilder EnableDebugMode();
    }

    /// <summary>
    ///     Builder interface for constructing new <see cref="Span" /> instances.
    /// </summary>
    public sealed class SpanBuilder : IZipkinSpanBuilder
    {
        private readonly string _operationName;

        private readonly IZipkinTracer _tracer;
        private bool _enableDebug;
        private bool _shared;
        private bool _sampled;
        private bool _ignoreActive;
        private Dictionary<string, string> _initialTags;
        private List<SpanReference> _references;
        private SpanKind? _spanKind;
        private DateTimeOffset? _start;

        public SpanBuilder(IZipkinTracer tracer, string operationName)
        {
            _tracer = tracer;
            _operationName = operationName;
        }

        public IZipkinSpanBuilder AsChildOf(ISpanContext parent)
        {
            return AddReference(References.ChildOf, parent);
        }

        ISpanBuilder ISpanBuilder.AsChildOf(ISpan parent)
        {
            return AsChildOf(parent);
        }

        ISpanBuilder ISpanBuilder.AddReference(string referenceType, ISpanContext referencedContext)
        {
            return AddReference(referenceType, referencedContext);
        }

        ISpanBuilder ISpanBuilder.IgnoreActiveSpan()
        {
            return IgnoreActiveSpan();
        }

        ISpanBuilder ISpanBuilder.WithTag(string key, string value)
        {
            return WithTag(key, value);
        }

        ISpanBuilder ISpanBuilder.WithTag(string key, bool value)
        {
            return WithTag(key, value);
        }

        ISpanBuilder ISpanBuilder.WithTag(string key, int value)
        {
            return WithTag(key, value);
        }

        ISpanBuilder ISpanBuilder.WithTag(string key, double value)
        {
            return WithTag(key, value);
        }

        ISpanBuilder ISpanBuilder.WithStartTimestamp(DateTimeOffset timestamp)
        {
            return WithStartTimestamp(timestamp);
        }

        ISpanBuilder ISpanBuilder.AsChildOf(ISpanContext parent)
        {
            return AsChildOf(parent);
        }

        public IZipkinSpanBuilder AsChildOf(ISpan parent)
        {
            return AsChildOf(parent.Context);
        }

        public IZipkinSpanBuilder AddReference(string referenceType, ISpanContext referencedContext)
        {
            if (_references == null) _references = new List<SpanReference>();
            _references.Add(new SpanReference(referenceType, referencedContext));
            return this;
        }

        public IZipkinSpanBuilder IgnoreActiveSpan()
        {
            _ignoreActive = true;
            return this;
        }

        public IZipkinSpanBuilder WithTag(string key, string value)
        {
            if (_initialTags == null) _initialTags = new Dictionary<string, string>();
            _initialTags[key] = value;
            return this;
        }

        public IZipkinSpanBuilder WithTag(string key, bool value)
        {
            return WithTag(key, Convert.ToString(value));
        }

        public IZipkinSpanBuilder WithTag(string key, int value)
        {
            return WithTag(key, Convert.ToString(value));
        }

        public IZipkinSpanBuilder WithTag(string key, double value)
        {
            return WithTag(key, Convert.ToString(value, CultureInfo.InvariantCulture));
        }

        public IZipkinSpanBuilder WithStartTimestamp(DateTimeOffset timestamp)
        {
            _start = timestamp;
            return this;
        }

        public IScope StartActive(bool finishSpanOnDispose)
        {
            return _tracer.ScopeManager.Activate(Start(), finishSpanOnDispose);
        }

        ISpan ISpanBuilder.Start()
        {
            return Start();
        }

        public Span Start()
        {
            if (_start == null)
                _start = _tracer.TimeProvider.Now;

            var activeSpanContext = _tracer.ActiveSpan?.Context;
            SpanContext parentContext = null;

            if (_references != null && (_ignoreActive || activeSpanContext.IsEmpty()))
                parentContext = FindBestFittingReference(_references);
            else if (!activeSpanContext.IsEmpty())
                parentContext = (SpanContext) activeSpanContext;

            return new Span(_tracer, _operationName,
                new SpanContext(parentContext.IsEmpty() ? _tracer.IdProvider.NextTraceId() : parentContext.TraceId,
                    _tracer.IdProvider.NextSpanId(),
                    parentContext?.SpanId, _enableDebug, _sampled, _shared), _start.Value, _spanKind);
        }

        public IZipkinSpanBuilder WithSpanKind(SpanKind spanKind)
        {
            _spanKind = spanKind;
            return this;
        }

        public IZipkinSpanBuilder EnableDebugMode()
        {
            _enableDebug = true;
            return this;
        }

        private static SpanContext FindBestFittingReference(IReadOnlyList<SpanReference> references)
        {
            foreach (var reference in references)
                if (reference.ReferenceType.Equals(References.ChildOf))
                    return (SpanContext) reference.SpanContext;

            return (SpanContext) references.First().SpanContext;
        }

        /// <summary>
        ///     INTERNAL API. Used to describe a relationship between <see cref="Span" /> instances.
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
    }
}