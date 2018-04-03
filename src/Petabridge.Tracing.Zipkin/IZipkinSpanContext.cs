// -----------------------------------------------------------------------
// <copyright file="IZipkinSpanContext.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using OpenTracing;

namespace Petabridge.Tracing.Zipkin
{
    /// <inheritdoc cref="ISpanContext" />
    /// <summary>
    ///     Defines a Zipkin implementation of <see cref="T:OpenTracing.ISpanContext" />.
    /// </summary>
    public interface IZipkinSpanContext : ISpanContext, IEquatable<IZipkinSpanContext>
    {
        /// <summary>
        ///     The trace ID. Intended to be shared across spans for
        ///     a single logical trace.
        /// </summary>
        TraceId TraceId { get; }

        /// <summary>
        ///     The span ID. Used to identify a single atomic operation that is being
        ///     tracked as part of an ongoing trace.
        /// </summary>
        long SpanId { get; }

        /// <summary>
        ///     Optional. Identify of the parent span if there is one.
        /// </summary>
        long? ParentId { get; }

        /// <summary>
        ///     Indicates if this is a Debug trace or not.
        /// </summary>
        bool Debug { get; }

        /// <summary>
        ///     Indicates if this is a sampled trace or not.
        /// </summary>
        bool Sampled { get; }

        /// <summary>
        ///     Indicates if this is a shared trace or not.
        /// </summary>
        bool Shared { get; }
    }
}