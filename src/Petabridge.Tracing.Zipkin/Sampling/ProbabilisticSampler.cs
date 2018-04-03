using System;
using Akka.Util;

namespace Petabridge.Tracing.Zipkin.Sampling
{
    /// <inheritdoc />
    /// <summary>
    /// Samples <see cref="T:OpenTracing.ISpan" /> instances based on a probabilistic distribution using random numbers.
    /// </summary>
    public sealed class ProbabilisticSampler : ITraceSampler
    {
        public ProbabilisticSampler(double sampleRate)
        {
            if (sampleRate < 0.0d || sampleRate > 1.0d)
                throw new ArgumentOutOfRangeException(nameof(sampleRate),
                    $"Sample rate must be in [0.0, 1.0]. Was [{sampleRate}]");
            SampleRate = sampleRate;
        }

        /// <summary>
        /// A value between 0.0 (exclude everything) and 1.0 (include everything)
        /// </summary>
        public double SampleRate { get; }

        public bool IncludeInSample(string operationName)
        {
            return ThreadLocalRandom.Current.NextDouble() <= SampleRate;
        }
    }
}