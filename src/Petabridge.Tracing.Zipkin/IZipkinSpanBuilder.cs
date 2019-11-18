// -----------------------------------------------------------------------
// <copyright file="IZipkinSpanBuilder.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2019 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using OpenTracing;
using OpenTracing.Tag;

namespace Petabridge.Tracing.Zipkin
{
    /// <summary>
    ///     Zipkin-specific <see cref="ISpanBuilder" /> interface.
    /// </summary>
    public interface IZipkinSpanBuilder : ISpanBuilder
    {
        new IZipkinSpanBuilder AsChildOf(ISpanContext parent);

        new IZipkinSpanBuilder AsChildOf(ISpan parent);

        new IZipkinSpanBuilder IgnoreActiveSpan();

        new IZipkinSpanBuilder WithTag(string key, string value);

        new IZipkinSpanBuilder WithTag(string key, bool value);

        new IZipkinSpanBuilder WithTag(string key, int value);

        new IZipkinSpanBuilder WithTag(string key, double value);
        new IZipkinSpanBuilder WithTag(BooleanTag tag, bool value);

        new IZipkinSpanBuilder WithTag(IntOrStringTag tag, string value);

        new IZipkinSpanBuilder WithTag(IntTag tag, int value);

        new IZipkinSpanBuilder WithTag(StringTag tag, string value);

        new IZipkinSpanBuilder WithStartTimestamp(DateTimeOffset timestamp);

        new IZipkinSpan Start();

        IZipkinSpanBuilder WithSpanKind(SpanKind spanKind);

        /// <summary>
        ///     Sets debug mode to enabled for this particular <see cref="ISpan" />.
        /// </summary>
        /// <param name="debugOn"><c>true</c> if debug mode will be enabled. <c>false</c> otherwise.</param>
        /// <returns>The current span builder instance.</returns>
        IZipkinSpanBuilder SetDebugMode(bool debugOn);

        /// <summary>
        ///     Forces the current <see cref="ISpan" /> to be included in the sample regardless of what the
        ///     <see cref="IZipkinTracer.Sampler" />
        ///     decides.
        /// </summary>
        /// <param name="includeInSample">
        ///     <c>true</c> if the <see cref="ISpan" /> will be included in the sample. <c>false</c>
        ///     otherwise.
        /// </param>
        /// <returns>The current span builder instance.</returns>
        IZipkinSpanBuilder ForceIncludeInSample(bool includeInSample = true);
    }
}