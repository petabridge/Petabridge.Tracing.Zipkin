// -----------------------------------------------------------------------
// <copyright file="IZipkinSpanContext.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2019 Petabridge, LLC <https://petabridge.com>
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
        TraceId ZipkinTraceId { get; }

        /// <summary>
        ///     Optional. Identify of the parent span if there is one.
        /// </summary>
        string ParentId { get; }

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