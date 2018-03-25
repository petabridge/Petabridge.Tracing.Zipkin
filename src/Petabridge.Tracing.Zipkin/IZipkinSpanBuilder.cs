// -----------------------------------------------------------------------
// <copyright file="IZipkinSpanBuilder.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using OpenTracing;

namespace Petabridge.Tracing.Zipkin
{
    /// <summary>
    ///     Zipkin-specific <see cref="ISpanBuilder" /> interface.
    /// </summary>
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
}