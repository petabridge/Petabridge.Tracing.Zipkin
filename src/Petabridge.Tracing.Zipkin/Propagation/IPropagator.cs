// -----------------------------------------------------------------------
// <copyright file="IPropagator.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

namespace Petabridge.Tracing.Zipkin.Propagation
{
    public interface IPropagator<TCarrier>
    {
        void Inject(SpanContext context, TCarrier carrier);
    }
}