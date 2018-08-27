using Microsoft.IO;

namespace Petabridge.Tracing.Zipkin.Reporting
{
    /// <summary>
    /// INTERNAL API.
    ///
    /// Used to provide access to the <see cref="RecyclableMemoryStreamManager"/> across different <see cref="ISpanReporter"/>
    /// implementations.
    /// </summary>
    internal static class SerializationStreamManager
    {
        /// <summary>
        ///     Only need one of these globally, and it's thread-safe. The streams it accesses internally are inherently not safe.
        /// </summary>
        public static readonly RecyclableMemoryStreamManager StreamManager = new RecyclableMemoryStreamManager();
    }
}