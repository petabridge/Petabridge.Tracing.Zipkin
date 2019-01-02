using System;
using System.Text;
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
            var length = WriteB3SingleFormatHeader(context, buffer);
            return new string(buffer, 0, length);
        }

        public static byte[] WriteB3SingleFormatAsBytes(SpanContext context)
        {
            var buffer = CHAR_ARRAY.Value;
            var length = WriteB3SingleFormatHeader(context, buffer);
            return Encoding.UTF8.GetBytes(buffer, 0, length);
        }

        private static int WriteB3SingleFormatHeader(SpanContext context, char[] result)
        {
            var pos = 0;
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

        public static SpanContext ParseB3SingleFormat(string b3)
        {
            // TODO: avoid allocation here
            return ParseB3SingleFormat(b3.ToCharArray(), 0, b3.Length);
        }

        private static SpanContext ParseB3SingleFormat(char[] b3, int begin, int count)
        {
            if (count == 0)
            {
                return null;
            }

            var pos = begin;
            if (pos + 1 == count) // sampling flags only
            {
                // TODO: add support for sampling flags only
                return null;
            }

            // At this point we expect at least a traceid-spanid pair
            if (count < 16 + 1 + 16)
            {
                throw new ArgumentOutOfRangeException(nameof(b3), $"Invalid input: truncated {new string(b3)}");
            }
            if (count > FORMAT_MAX_LENGTH)
            {
                throw new ArgumentOutOfRangeException(nameof(b3), $"Invalid input: too long {new string(b3)}");
            }

            string traceId = null;
            if (b3[pos + 32] == '-')
            {
                traceId = new string(b3, pos, 32);
                pos += 32;
            }
            else
            {
                traceId = new string(b3, 0, 16);
                pos += 16;
            }

            TraceId trace;
            if (!TraceId.TryParse(traceId, out trace))
            {
                throw new ArgumentOutOfRangeException("traceId", $"Invalid input: expected a 16 or 32 lower hex trace ID at offset 0 [{traceId}]");
            }

            if (!CheckHyphen(b3, pos++)) return null;

            if (pos + 16 > count)
            {
                throw new ArgumentOutOfRangeException("spanId", $"Invalid input: expected a 16 span id at offset {pos}");
            }

            var spanId = new string(b3, pos, 16);
            pos += 16; // spanid

            var sampled = false;
            var debug = false;
            string parentId = null;
            if (count > pos) // sampling flags or debug
            {
                if (count == pos + 1) // sampling flag didn't get included
                {
                    throw new ArgumentOutOfRangeException(nameof(b3), "Invalid input: truncated");
                }

                if (!CheckHyphen(b3, pos++)) return null;

                var flagFound = false;
                if (count > pos)
                {
                    var sampledField = b3[pos];
                    switch (sampledField)
                    {
                        case 'd':
                            debug = true;
                            flagFound = true;
                            break;
                        case '1':
                            sampled = true;
                            flagFound = true;
                            break;
                        case '0':
                            sampled = false;
                            flagFound = true;
                            break;
                        default:
                            break;
                    }

                    if (flagFound)
                    {
                        pos++; // need to account for the flag
                        if (!CheckHyphen(b3, pos++))
                        {
                            return new SpanContext(trace, spanId, null, debug, sampled);
                        }
                    }
                }


                if (count > pos)
                {
                    //If we've made it here, there should be a parentId
                    if (pos + 16 > count)
                    {
                        throw new ArgumentOutOfRangeException("parentId", $"Invalid input: expected a 16 parent id at offset {pos}");
                    }
                    parentId = new string(b3, pos, 16);
                }
            }

            return new SpanContext(trace, spanId, parentId, debug, sampled);
        }

        static bool CheckHyphen(char[] b3, int pos)
        {
            if (b3.Length > pos && b3[pos] == '-') return true;
            return false;
        }
    }
}