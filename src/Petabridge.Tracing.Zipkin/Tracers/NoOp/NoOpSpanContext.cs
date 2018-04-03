// -----------------------------------------------------------------------
// <copyright file="NoOpSpanContext.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using OpenTracing;

namespace Phobos.Tracing.Zipkin
{
    /// <summary>
    ///     Used when there's no active span context.
    /// </summary>
    public sealed class NoOpSpanContext : ISpanContext
    {
        public static readonly NoOpSpanContext Instance = new NoOpSpanContext();

        public static readonly IEnumerable<KeyValuePair<string, string>> Empty = new Dictionary<string, string>();

        private NoOpSpanContext()
        {
        }

        public IEnumerable<KeyValuePair<string, string>> GetBaggageItems()
        {
            return Empty;
        }
    }
}