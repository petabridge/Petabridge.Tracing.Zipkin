// -----------------------------------------------------------------------
// <copyright file="ThreadLocalRngSpanIdProvider.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;

namespace Petabridge.Tracing.Zipkin.Util
{
    /// <inheritdoc />
    /// <summary>
    ///     Default implementation of <see cref="T:Petabridge.Tracing.Zipkin.ISpanIdProvider" />.
    ///     Uses a <see cref="T:System.Threading.ThreadLocal`1" />  to seed values for 128bit and 64bit IDs.
    /// </summary>
    public sealed class ThreadLocalRngSpanIdProvider : ISpanIdProvider
    {
        public static readonly ISpanIdProvider TraceId128BitProvider = new ThreadLocalRngSpanIdProvider(true);
        public static readonly ISpanIdProvider TraceId64BitProvider = new ThreadLocalRngSpanIdProvider(false);

        private static readonly ThreadLocal<Random> Rng = new ThreadLocal<Random>(() => new Random(), false);
        private static readonly ThreadLocal<byte[]> Buffers = new ThreadLocal<byte[]>(() => new byte[8], false);

        private ThreadLocalRngSpanIdProvider(bool use128Bit)
        {
            Use128Bit = use128Bit;
        }

        public bool Use128Bit { get; }

        public TraceId NextTraceId()
        {
            if (Use128Bit)
                return new TraceId(NextSpanIdLong(), NextSpanIdLong());
            return new TraceId(NextSpanIdLong());
        }

        public string NextSpanId()
        {
            return NextSpanIdLong().ToString("x16");
        }

        public long NextSpanIdLong()
        {
            Rng.Value.NextBytes(Buffers.Value);
            return BitConverter.ToInt64(Buffers.Value, 0);
        }
    }
}