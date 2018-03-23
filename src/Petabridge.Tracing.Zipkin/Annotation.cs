// -----------------------------------------------------------------------
// <copyright file="Annotation.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Petabridge.Tracing.Zipkin
{
    /// <summary>
    ///     Metadata that will be subsequently included in the Zipkin readout for a given <see cref="Span" />.
    /// </summary>
    public struct Annotation
    {
        public Annotation(string value, DateTimeOffset timeStamp)
        {
            Value = value;
            TimeStamp = timeStamp;
        }

        public string Value { get; }

        public DateTimeOffset TimeStamp { get; }
    }
}