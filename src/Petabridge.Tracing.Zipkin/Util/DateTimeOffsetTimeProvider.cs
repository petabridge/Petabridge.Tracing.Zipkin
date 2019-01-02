// -----------------------------------------------------------------------
// <copyright file="DateTimeOffsetTimeProvider.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2019 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Petabridge.Tracing.Zipkin.Util
{
    /// <inheritdoc />
    /// <summary>
    ///     Creates new timestamps via simply new-ing up a <see cref="T:System.DateTimeOffset" />.
    /// </summary>
    public sealed class DateTimeOffsetTimeProvider : ITimeProvider
    {
        public DateTimeOffset Now => DateTimeOffset.UtcNow;
    }
}