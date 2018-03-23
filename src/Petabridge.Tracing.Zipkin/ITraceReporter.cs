// -----------------------------------------------------------------------
// <copyright file="ITraceReporter.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

namespace Petabridge.Tracing.Zipkin
{
    /// <summary>
    ///     Component used for reporting trace results to Zipkin via any of its supported transports.
    /// </summary>
    public interface ITraceReporter
    {
        void Report(Span span);
    }
}