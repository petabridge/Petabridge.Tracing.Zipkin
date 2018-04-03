// -----------------------------------------------------------------------
// <copyright file="NoOpSpan.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using OpenTracing;

namespace Phobos.Tracing.Zipkin
{
    /// <inheritdoc />
    /// <summary>
    ///     INTERNAL API.
    ///     Used when there's no scope available.
    /// </summary>
    public sealed class NoOpSpan : ISpan
    {
        public static readonly NoOpSpan Instance = new NoOpSpan();

        private NoOpSpan()
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

        public ISpanContext Context => NoOpSpanContext.Instance;
    }
}