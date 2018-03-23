// -----------------------------------------------------------------------
// <copyright file="Span.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using OpenTracing;

namespace Petabridge.Tracing.Zipkin
{
    /// <summary>
    ///     Describes a single unit of work that is being traced by Zipkin.
    ///     A span should essentially cover one logical operation - if that operation spawns
    ///     or causes other subsequent operations elsewhere inside the distributed system, these
    ///     spans can be connected together through a parent-child relationship.
    /// </summary>
    public class Span : ISpan
    {
        public static readonly IReadOnlyDictionary<string, string> EmptyTags = new Dictionary<string, string>();
        public static readonly IReadOnlyList<Annotation> EmptyAnnotations = new Annotation[0];

        /// <summary>
        ///     The <see cref="IZipkinTracer" /> that we will use to complete this span.
        /// </summary>
        private readonly IZipkinTracer _tracer;

        private List<Annotation> _annotations;
        private Dictionary<string, string> _tags;

        public Span(IZipkinTracer tracer, string operationName, SpanContext context)
        {
            _tracer = tracer;
            OperationName = operationName;
            TypedContext = context;
        }

        /// <summary>
        ///     The name of the operation for this <see cref="Span" />
        /// </summary>
        public string OperationName { get; }

        /// <summary>
        ///     The start time of this operation.
        /// </summary>
        public DateTimeOffset Started { get; }

        /// <summary>
        ///     The completion time of this operation.
        /// </summary>
        public DateTimeOffset? Finished { get; private set; }

        public IReadOnlyList<Annotation> Annotations => _annotations ?? EmptyAnnotations;

        public IReadOnlyDictionary<string, string> Tags => _tags ?? EmptyTags;

        /// <summary>
        ///     For people who aren't fond of boxing.
        /// </summary>
        public SpanContext TypedContext { get; }

        public ISpan SetTag(string key, string value)
        {
            throw new NotImplementedException();
        }

        public ISpan SetTag(string key, bool value)
        {
            throw new NotImplementedException();
        }

        public ISpan SetTag(string key, int value)
        {
            throw new NotImplementedException();
        }

        public ISpan SetTag(string key, double value)
        {
            throw new NotImplementedException();
        }

        public ISpan Log(IDictionary<string, object> fields)
        {
            throw new NotImplementedException();
        }

        public ISpan Log(DateTimeOffset timestamp, IDictionary<string, object> fields)
        {
            throw new NotImplementedException();
        }

        public ISpan Log(string @event)
        {
            throw new NotImplementedException();
        }

        public ISpan Log(DateTimeOffset timestamp, string @event)
        {
            throw new NotImplementedException();
        }

        public ISpan SetBaggageItem(string key, string value)
        {
            throw new NotImplementedException();
        }

        public string GetBaggageItem(string key)
        {
            throw new NotImplementedException();
        }

        public ISpan SetOperationName(string operationName)
        {
            throw new NotImplementedException();
        }

        public void Finish()
        {
            throw new NotImplementedException();
        }

        public void Finish(DateTimeOffset finishTimestamp)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        /// <summary>
        ///     For OpenTracing compatibility.
        /// </summary>
        public ISpanContext Context => TypedContext;
    }
}