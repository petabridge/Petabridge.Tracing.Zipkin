// -----------------------------------------------------------------------
// <copyright file="Span.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using OpenTracing;

namespace Petabridge.Tracing.Zipkin
{
    /// <summary>
    /// Describes the type of span being used.
    /// </summary>
    /// <remarks>
    /// Based on the Zipkin conventions outlined here: https://zipkin.io/pages/instrumenting.html
    /// </remarks>
    public enum SpanKind
    {
        CLIENT,
        SERVER,
        PRODUCER,
        CONSUMER
    }

    /// <summary>
    ///     Describes a single unit of work that is being traced by Zipkin.
    ///     A span should essentially cover one logical operation - if that operation spawns
    ///     or causes other subsequent operations elsewhere inside the distributed system, these
    ///     spans can be connected together through a parent-child relationship.
    /// </summary>
    public class Span : ISpan, IDisposable
    {
        public static readonly IReadOnlyDictionary<string, string> EmptyTags = new Dictionary<string, string>();
        public static readonly IReadOnlyList<Annotation> EmptyAnnotations = new Annotation[0];

        /// <summary>
        ///     The <see cref="IZipkinTracer" /> that we will use to complete this span.
        /// </summary>
        private readonly IZipkinTracer _tracer;

        private List<Annotation> _annotations;
        private Dictionary<string, string> _tags;

        public Span(IZipkinTracer tracer, string operationName, SpanContext context, SpanKind kind)
        {
            _tracer = tracer;
            OperationName = operationName;
            TypedContext = context;
            Started = tracer.TimeProvider.Now;
            SpanKind = kind;
        }

        /// <summary>
        ///     The name of the operation for this <see cref="Span" />
        /// </summary>
        public string OperationName { get; private set; }

        /// <summary>
        ///     The start time of this operation.
        /// </summary>
        public DateTimeOffset Started { get; }

        /// <summary>
        ///     The completion time of this operation.
        /// </summary>
        public DateTimeOffset? Finished { get; private set; }

        /// <summary>
        /// The duration of this operation.
        /// </summary>
        public TimeSpan? Duration
        {
            get
            {
                if (!Finished.HasValue) return null;
                return Finished - Started;
            }
        }

        /// <summary>
        /// The type of span for this operation.
        /// </summary>
        public SpanKind SpanKind { get; }

        public IReadOnlyList<Annotation> Annotations => _annotations ?? EmptyAnnotations;

        public IReadOnlyDictionary<string, string> Tags => _tags ?? EmptyTags;

        /// <summary>
        ///     For people who aren't fond of boxing.
        /// </summary>
        public SpanContext TypedContext { get; }

        public ISpan SetTag(string key, string value)
        {
            _tags = _tags ?? new Dictionary<string, string>();
            _tags[key] = value;
            return this;
        }

        public ISpan SetTag(string key, bool value)
        {
            return SetTag(key, Convert.ToString(value));
        }

        public ISpan SetTag(string key, int value)
        {
            return SetTag(key, Convert.ToString(value));
        }

        public ISpan SetTag(string key, double value)
        {
            return SetTag(key, Convert.ToString(value, CultureInfo.InvariantCulture));
        }

        public ISpan Log(IDictionary<string, object> fields)
        {
            return Log(_tracer.TimeProvider.Now, MergeFields(fields));
        }

        public ISpan Log(DateTimeOffset timestamp, IDictionary<string, object> fields)
        {
            return Log(timestamp, MergeFields(fields));
        }

        public ISpan Log(string @event)
        {
            return Log(_tracer.TimeProvider.Now, @event);
        }

        public ISpan Log(DateTimeOffset timestamp, string @event)
        {
            return Annotate(timestamp, @event);
        }

        private static string MergeFields(IDictionary<string, object> fields)
        {
            return string.Join(" ", fields.Select(entry => entry.Key + ":" + entry.Value));
        }

        public ISpan SetBaggageItem(string key, string value)
        {
            throw new NotSupportedException("Baggage is not supported in Zipkin");
        }

        public string GetBaggageItem(string key)
        {
            throw new NotSupportedException("Baggage is not supported in Zipkin");
        }

        public ISpan SetOperationName(string operationName)
        {
            OperationName = operationName;
            return this;
        }

        internal ISpan Annotate(DateTimeOffset time, string annotationValue)
        {
            _annotations = _annotations ?? new List<Annotation>();
            _annotations.Add(new Annotation(time, annotationValue));
            return this;
        }

        public void Finish()
        {
            Finish(_tracer.TimeProvider.Now);
        }

        public void Finish(DateTimeOffset finishTimestamp)
        {
            if (!Finished.HasValue)
            {
                Finished = finishTimestamp;
                _tracer.Report(this); // send me away
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     For OpenTracing compatibility.
        /// </summary>
        public ISpanContext Context => TypedContext;

        public void Dispose()
        {
            Finish();
        }
    }
}