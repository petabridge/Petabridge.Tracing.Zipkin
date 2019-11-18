// -----------------------------------------------------------------------
// <copyright file="ISpanSerializer.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2019 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;

namespace Petabridge.Tracing.Zipkin.Reporting
{
    /// <summary>
    ///     Used to serialize a <see cref="Span" /> into a wire-friendly format.
    /// </summary>
    public interface ISpanSerializer
    {
        /// <summary>
        ///     Serializes the provided span into the <see cref="Stream" />.
        /// </summary>
        /// <param name="stream">A writable stream that we'll be adding the contents of the span to.</param>
        /// <param name="span">The span to be serialized.</param>
        void Serialize(Stream stream, Span span);

        /// <summary>
        ///     Serializes a collection of spans into the <see cref="Stream" />.
        /// </summary>
        /// <param name="stream">A writable stream that we'll be adding the contents of the span to.</param>
        /// <param name="spans">The spans to be serialized.</param>
        /// <remarks>
        ///     Designed to offer support for batching et al.
        /// </remarks>
        void Serialize(Stream stream, IEnumerable<Span> spans);
    }
}