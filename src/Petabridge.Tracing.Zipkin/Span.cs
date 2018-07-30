﻿// -----------------------------------------------------------------------
// <copyright file="Span.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using OpenTracing;
using OpenTracing.Tag;

namespace Petabridge.Tracing.Zipkin
{
    /// <summary>
    ///     Describes the type of span being used.
    /// </summary>
    /// <remarks>
    ///     Based on the Zipkin conventions outlined here: https://zipkin.io/pages/instrumenting.html
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
    public class Span : IZipkinSpan, IDisposable
    {
        public static readonly IReadOnlyDictionary<string, string> EmptyTags = new Dictionary<string, string>();
        public static readonly IReadOnlyList<Annotation> EmptyAnnotations = new Annotation[0];

        /// <summary>
        ///     The <see cref="IZipkinTracer" /> that we will use to complete this span.
        /// </summary>
        private readonly IZipkinTracer _tracer;

        private List<Annotation> _annotations;
        private Dictionary<string, string> _tags;

        public Span(IZipkinTracer tracer, string operationName, IZipkinSpanContext context, DateTimeOffset started,
            SpanKind? kind = null, Endpoint localEndpoint = null, Dictionary<string, string> tags = null)
        {
            _tracer = tracer;
            OperationName = operationName;
            TypedContext = context;
            Started = started;
            SpanKind = kind;
            LocalEndpoint = localEndpoint ?? _tracer.LocalEndpoint;
            _tags = tags;
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
        ///     The duration of this operation.
        /// </summary>
        public TimeSpan? Duration
        {
            get
            {
                if (!Finished.HasValue) return null;
                return Finished - Started;
            }
        }

        public IReadOnlyList<Annotation> Annotations => _annotations ?? EmptyAnnotations;

        public IReadOnlyDictionary<string, string> Tags => _tags ?? EmptyTags;

        /// <summary>
        ///     The local <see cref="Endpoint" />
        /// </summary>
        public Endpoint LocalEndpoint { get; }

        /// <summary>
        ///     The remote <see cref="Endpoint" />. Has to be set by the <see cref="ISpanBuilder" /> or the <see cref="ISpan" />.
        /// </summary>
        public Endpoint RemoteEndpoint { get; private set; }

        public void Dispose()
        {
            Finish();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Optional. The type of span for this operation. Defaults to <c>null</c>.
        /// </summary>
        public SpanKind? SpanKind { get; private set; }

        /// <summary>
        ///     For people who aren't fond of boxing.
        /// </summary>
        public IZipkinSpanContext TypedContext { get; }

        /// <inheritdoc />
        /// <summary>
        ///     Indicates if the <see cref="T:Petabridge.Tracing.Zipkin.Span" /> is being used to during debugging.
        /// </summary>
        public bool Debug => TypedContext.Debug;

        /// <inheritdoc />
        /// <summary>
        ///     Indicates if the current <see cref="T:Petabridge.Tracing.Zipkin.Span" /> is shared among many other traces.
        /// </summary>
        public bool Shared => TypedContext.Shared;

        /// <inheritdoc />
        /// <summary>
        ///     Indicates if the current <see cref="T:Petabridge.Tracing.Zipkin.Span" /> is part of sampling.
        /// </summary>
        public bool Sampled => TypedContext.Sampled;

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

        public ISpan SetTag(BooleanTag tag, bool value)
        {
            return SetTag(tag.Key, value);
        }

        public ISpan SetTag(IntOrStringTag tag, string value)
        {
            return SetTag(tag.Key, value);
        }

        public ISpan SetTag(IntTag tag, int value)
        {
            return SetTag(tag.Key, value);
        }

        public ISpan SetTag(StringTag tag, string value)
        {
            return SetTag(tag.Key, value);
        }

        public ISpan Log(IEnumerable<KeyValuePair<string, object>> fields)
        {
            return Log(_tracer.TimeProvider.Now, MergeFields(fields));
        }

        public ISpan Log(DateTimeOffset timestamp, IEnumerable<KeyValuePair<string, object>> fields)
        {
            return Log(timestamp, MergeFields(fields));
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

        /// <summary>
        ///     Sets the <see cref="RemoteEndpoint" /> for this span.
        /// </summary>
        /// <param name="remoteEndpoint">The remote endpoint.</param>
        /// <returns>This <see cref="Span" />.</returns>
        public Span SetRemoteEndpoint(Endpoint remoteEndpoint)
        {
            RemoteEndpoint = remoteEndpoint;
            return this;
        }

        public Span SetSpanKind(SpanKind kind)
        {
            SpanKind = kind;
            return this;
        }

        private static string MergeFields(IEnumerable<KeyValuePair<string, object>> fields)
        {
            return string.Join(" ", fields.Select(entry => entry.Key + ":" + entry.Value));
        }

        internal ISpan Annotate(DateTimeOffset time, string annotationValue)
        {
            _annotations = _annotations ?? new List<Annotation>();
            _annotations.Add(new Annotation(time, annotationValue));
            return this;
        }
    }
}