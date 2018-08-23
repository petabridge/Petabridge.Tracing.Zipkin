using System;
using System.Collections.Generic;
using System.Text;

namespace Petabridge.Tracing.Zipkin.Reporting.NoOp
{
    /// <summary>
    /// INTERNAL API.
    ///
    /// Reporter that doesn't actually report spans. Used for unit testing.
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
