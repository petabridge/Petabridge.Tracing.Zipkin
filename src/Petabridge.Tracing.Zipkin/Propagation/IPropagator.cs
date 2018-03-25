using System;
using System.Collections.Generic;
using System.Text;

namespace Petabridge.Tracing.Zipkin.Propagation
{
    public interface IPropagator<TCarrier>
    {
        void Inject(SpanContext context, TCarrier carrier);
    }
}
