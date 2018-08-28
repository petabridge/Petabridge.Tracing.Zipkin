// -----------------------------------------------------------------------
// <copyright file="ISpanReporter.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Petabridge.Tracing.Zipkin
{
    /// <summary>
    ///     Component used for reporting trace results to Zipkin via any of its supported transports.
    /// </summary>
    public interface ISpanReporter : IDisposable
    {
        void Report(Span span);
    }
}