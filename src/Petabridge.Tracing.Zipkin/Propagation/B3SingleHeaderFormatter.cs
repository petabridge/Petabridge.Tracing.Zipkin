using System.Threading;

namespace Petabridge.Tracing.Zipkin.Propagation
{
    /// <summary>
    /// Used for parsing the B3 "single header" format
    /// </summary>
    /// <remarks>
    /// See https://github.com/openzipkin/b3-propagation/issues/21 for rationale.
    /// </remarks>
    public static class B3SingleHeaderFormatter
    {
        /// <summary>
        /// The maximum length of a fully specified B3 single length header
        /// </summary>
        public const int FORMAT_MAX_LENGTH = 32 + 1 + 16 + 3 + 16; // traceid128-spanid-1-parentid

        private static readonly ThreadLocal<char[]> CHAR_ARRAY = new ThreadLocal<char[]>(() => new char[FORMAT_MAX_LENGTH]);

        public static string WriteB3SingleFormat(SpanContext context)
        {
            var buffer = CHAR_ARRAY.Value;
            int length = WriteB3SingleFormatHeader(context, buffer);
            return new string(buffer, 0, length);
        }

        private static int WriteB3SingleFormatHeader(SpanContext context, char[] result)
        {
            int pos = 0;
            context.TraceId.CopyTo(0, result, pos, context.TraceId.Length);
            if (context.ZipkinTraceId.Is128Bit)
            {
                pos += 16;
            }
            pos += 16;
            result[pos++] = '-';
            context.SpanId.CopyTo(0, result, pos, context.SpanId.Length);
            pos += 16;

            if (context.Sampled || context.Debug)
            {
                result[pos++] = '-';
                result[pos++] = context.Debug ? 'd' : context.Sampled ? '1' : '0';
            }

            if (context.ParentId != null)
            {
                result[pos++] = '-';
                context.ParentId.CopyTo(0, result, pos, context.ParentId.Length);
                pos += 16;
            }

            return pos;
        }
    }
}