namespace Petabridge.Tracing.Zipkin.Sampling
{
    /// <inheritdoc />
    /// <summary>
    /// Used when we have no sampling enabled.
    /// </summary>
    public sealed class NoSampler : ITraceSampler
    {
        /// <summary>
        /// Singleton instance of <see cref="NoSampler"/>
        /// </summary>
        public static readonly NoSampler Instance = new NoSampler();
        
        private NoSampler() { }

        public bool IncludeInSample(string operationName)
        {
            return true;
        }
    }
}