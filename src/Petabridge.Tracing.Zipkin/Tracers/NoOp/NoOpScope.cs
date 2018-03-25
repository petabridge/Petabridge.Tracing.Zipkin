// -----------------------------------------------------------------------
// <copyright file="NoOpScope.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using OpenTracing;
using Phobos.Tracing.Zipkin;

namespace Petabridge.Tracing.Zipkin.Tracers.NoOp
{
    /// <inheritdoc />
    /// <summary>
    ///     INTERNAL API.
    ///     Represents no-op scope.
    /// </summary>
    public sealed class NoOpScope : IScope
    {
        public static readonly NoOpScope Instance = new NoOpScope();

        private NoOpScope()
        {
        }

        public void Dispose()
        {
        }

        public ISpan Span => NoOpSpan.Instance;
    }
}