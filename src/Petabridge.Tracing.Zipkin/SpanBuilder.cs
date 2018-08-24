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
using OpenTracing.Tag;
using Petabridge.Tracing.Zipkin.Tracers;
using Petabridge.Tracing.Zipkin.Util;

namespace Petabridge.Tracing.Zipkin
{
    /// <summary>
    ///     Builder interface for constructing new <see cref="Span" /> instances.
    /// </summary>
    public sealed class SpanBuilder : IZipkinSpanBuilder
    {
        private readonly string _operationName;

        private readonly IZipkinTracer _tracer;
        private bool _enableDebug;
        private bool _forceIncludeInSample;
        private bool _ignoreActive;
        private Dictionary<string, string> _initialTags;
        private List<SpanReference> _references;
        private bool _shared;
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

        ISpanBuilder ISpanBuilder.AsChildOf(ISpanContext parent)
        {
            return AsChildOf(parent);
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

        public IZipkinSpanBuilder WithTag(BooleanTag tag, bool value)
        {
            return WithTag(tag.Key, value);
        }

        public IZipkinSpanBuilder WithTag(IntOrStringTag tag, string value)
        {
            return WithTag(tag.Key, value);
        }

        public IZipkinSpanBuilder WithTag(IntTag tag, int value)
        {
            return WithTag(tag.Key, value);
        }

        public IZipkinSpanBuilder WithTag(StringTag tag, string value)
        {
            return WithTag(tag.Key, value);
        }

        ISpanBuilder ISpanBuilder.WithTag(BooleanTag tag, bool value)
        {
            return WithTag(tag, value);
        }

        ISpanBuilder ISpanBuilder.WithTag(IntOrStringTag tag, string value)
        {
            return WithTag(tag, value);
        }

        ISpanBuilder ISpanBuilder.WithTag(IntTag tag, int value)
        {
            return WithTag(tag, value);
        }

        ISpanBuilder ISpanBuilder.WithTag(StringTag tag, string value)
        {
            return WithTag(tag, value);
        }

        ISpanBuilder ISpanBuilder.WithStartTimestamp(DateTimeOffset timestamp)
        {
            return WithStartTimestamp(timestamp);
        }

        public IScope StartActive()
        {
            return StartActive(true);
        }

        public IZipkinSpanBuilder AsChildOf(ISpan parent)
        {
            return AsChildOf(parent.Context);
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

        public IZipkinSpan Start()
        {
            if (_start == null)
                _start = _tracer.TimeProvider.Now;

            var activeSpanContext = _tracer.ActiveSpan?.Context;
            SpanContext parentContext = null;

            if (_references != null && (_ignoreActive || activeSpanContext.IsEmpty()))
                parentContext = FindBestFittingReference(_references);
            else if (!activeSpanContext.IsEmpty())
                parentContext = (SpanContext) activeSpanContext;

            // make a sampling decision (all child spans are included in the sample)

            var includedInSample = !parentContext.IsEmpty() || _forceIncludeInSample ||
                                   _tracer.Sampler.IncludeInSample(_operationName);

            if (_tracer.Sampler.Sampling && !includedInSample)
                return NoOpZipkinSpan.Instance;


            return new Span(_tracer, _operationName,
                new SpanContext(parentContext.IsEmpty() ? _tracer.IdProvider.NextTraceId() : parentContext.ZipkinTraceId,
                    _tracer.IdProvider.NextSpanId(),
                    parentContext?.SpanId, _enableDebug, _tracer.Sampler.Sampling, _shared), _start.Value, _spanKind, tags:_initialTags);
        }

        public IZipkinSpanBuilder WithSpanKind(SpanKind spanKind)
        {
            _spanKind = spanKind;
            return this;
        }

        public IZipkinSpanBuilder SetDebugMode(bool debugOn)
        {
            _enableDebug = true;
            return this;
        }

        public IZipkinSpanBuilder ForceIncludeInSample(bool includeInSample = true)
        {
            _forceIncludeInSample = includeInSample;
            return this;
        }

        public IZipkinSpanBuilder AddReference(string referenceType, ISpanContext referencedContext)
        {
            if (!referencedContext.IsZipkinSpan()) return this; // stop execution here
            if (_references == null) _references = new List<SpanReference>();
            _references.Add(new SpanReference(referenceType, referencedContext));
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
                if (obj is null) return false;
                return obj is SpanReference reference && Equals(reference);
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