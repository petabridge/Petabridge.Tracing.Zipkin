// -----------------------------------------------------------------------
// <copyright file="ITraceSampler.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using OpenTracing;

namespace Petabridge.Tracing.Zipkin
{
    /// <summary>
    ///     Strategy pattern for deciding which <see cref="ISpan" /> instances we want
    ///     to include inside the current sample or not.
    /// </summary>
    public interface ITraceSampler
    {
        /// <summary>
        ///     If <c>true</c>, indicates that we actually are excluding some samples and running a real sampling process.
        /// </summary>
        bool Sampling { get; }

        /// <summary>
        ///     Determines whether or not to include the next <see cref="Span" /> (which hasn't yet been created)
        ///     in the sample or not.
        /// </summary>
        /// <param name="operationName">
        ///     May or may not be used by the underlying sampling system to determine
        ///     if some operations should be sampled but not others.
        /// </param>
        /// <returns><c>true</c> if the next <see cref="Span" /> should be included in the sample. <c>false</c> otherwise.</returns>
        bool IncludeInSample(string operationName);
    }
}