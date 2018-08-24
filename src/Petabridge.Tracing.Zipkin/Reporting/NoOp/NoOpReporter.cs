// -----------------------------------------------------------------------
// <copyright file="NoOpReporter.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

namespace Petabridge.Tracing.Zipkin.Reporting.NoOp
{
    /// <summary>
    ///     INTERNAL API.
    ///     Reporter that doesn't actually report spans. Used for unit testing.
    /// </summary>
    public sealed class NoOpReporter : ISpanReporter
    {
        public void Dispose()
        {
        }

        public void Report(Span span)
        {
            // no-op
        }
    }
}