// -----------------------------------------------------------------------
// <copyright file="IZipkinSpan.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using OpenTracing;

namespace Petabridge.Tracing.Zipkin
{
    /// <summary>
    ///     A Zipkin-specific <see cref="ISpan" /> implementation.
    /// </summary>
    public interface IZipkinSpan : ISpan
    {
        IZipkinSpanContext TypedContext { get; }

        /// <summary>
        ///     Indicates if the <see cref="Span" /> is being used to during debugging.
        /// </summary>
        bool Debug { get; }

        /// <summary>
        ///     Indicates if the current <see cref="Span" /> is shared among many other traces.
        /// </summary>
        bool Shared { get; }

        /// <summary>
        ///     Indicates if the current <see cref="Span" /> is part of sampling.
        /// </summary>
        bool Sampled { get; }

        /// <summary>
        ///     The type of span.
        /// </summary>
        SpanKind? SpanKind { get; }
    }
}