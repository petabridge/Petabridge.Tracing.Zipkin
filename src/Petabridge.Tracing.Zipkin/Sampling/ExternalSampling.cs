// -----------------------------------------------------------------------
// <copyright file="ExternalSampling.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

namespace Petabridge.Tracing.Zipkin.Sampling
{
    /// <inheritdoc />
    /// <summary>
    ///     Used when sampling is running, but it's driven by an external engine
    ///     rather than anything internal to this Zipkin driver itself.
    /// </summary>
    public sealed class ExternalSampling : ITraceSampler
    {
        public static readonly ExternalSampling Instance = new ExternalSampling();

        private ExternalSampling()
        {
        }

        public bool Sampling { get; } = true;

        public bool IncludeInSample(string operationName)
        {
            /*
             * We return "true" here because if the code reached this stage,
             * the sampling already occurred at a level above the Zipkin driver.
             *
             * So we include any traces that make it here in the sample.
             */
            return true;
        }
    }
}