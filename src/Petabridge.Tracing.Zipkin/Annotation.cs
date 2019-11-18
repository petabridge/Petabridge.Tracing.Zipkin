// -----------------------------------------------------------------------
// <copyright file="Annotation.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2019 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Petabridge.Tracing.Zipkin
{
    /// <inheritdoc />
    /// <summary>
    ///     A standard string-based annotation.
    /// </summary>
    public struct Annotation
    {
        public Annotation(DateTimeOffset timestamp, string value)
        {
            Timestamp = timestamp;
            Value = value;
        }

        public DateTimeOffset Timestamp { get; }

        public string Value { get; }
    }
}