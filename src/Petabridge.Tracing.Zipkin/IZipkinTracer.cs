﻿// -----------------------------------------------------------------------
// <copyright file="IZipkinTracer.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using OpenTracing;

namespace Petabridge.Tracing.Zipkin
{
    /// <summary>
    ///     Interface for instrumenting
    /// </summary>
    public interface IZipkinTracer : ITracer
    {
        /// <summary>
        ///     The clock used by this <see cref="IZipkinTracer" />.
        /// </summary>
        ITimeProvider TimeProvider { get; }

        /// <summary>
        ///     Pipes the completed span to a <see cref="ITraceReporter" /> for delivery.
        /// </summary>
        /// <param name="span">A completed span.</param>
        void Report(Span span);
    }
}