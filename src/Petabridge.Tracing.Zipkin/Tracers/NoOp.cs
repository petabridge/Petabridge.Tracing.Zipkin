// -----------------------------------------------------------------------
// <copyright file="NoOp.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using OpenTracing;
using OpenTracing.Noop;

namespace Petabridge.Tracing.Zipkin.Tracers
{
    /// <summary>
    ///     Taps into the OpenTracing.NoOpTracer primitives,
    ///     which aren't publicly accessible right now as of OpenTracing v0.11
    /// </summary>
    public static class NoOp
    {
        public static readonly ISpan Span;
        public static readonly IScope Scope;
        public static readonly IScopeManager ScopeManager;

        static NoOp()
        {
            var noOpTracer = NoopTracerFactory.Create();
            Span = noOpTracer.BuildSpan("foo").Start();
            Scope = noOpTracer.ScopeManager.Active;
            ScopeManager = noOpTracer.ScopeManager;
        }
    }
}