// -----------------------------------------------------------------------
// <copyright file="NoOpScopeManager.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using OpenTracing;

namespace Petabridge.Tracing.Zipkin.Tracers.NoOp
{
    /// <inheritdoc />
    /// <summary>
    ///     INTERNAL API.
    /// </summary>
    public sealed class NoOpScopeManager : IScopeManager
    {
        public static readonly NoOpScopeManager Instance = new NoOpScopeManager();

        private NoOpScopeManager()
        {
        }

        public IScope Activate(ISpan span, bool finishSpanOnDispose)
        {
            return NoOpScope.Instance;
        }

        public IScope Active => NoOpScope.Instance;
    }
}