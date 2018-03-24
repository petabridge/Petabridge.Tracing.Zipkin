using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Petabridge.Tracing.Zipkin.Reporting
{
    /*
     * TODO: rewrite this without using Json.NET
     */
    /// <summary>
    /// Serializes <see cref="Span"/> objects into JSON format for delivery
    /// over one of the registered <see cref="ITraceReporter"/> implementations.
    /// </summary>
    public sealed class SpanJsonSerializer : ISpanSerializer
    {
        private const string TraceId = "traceId";
        private const string SpanId = "id";
        private const string ParentId = "parentId";
        private const string OperationName = "name";

        public void Serialize(Stream stream, Span span)
        {
            using (var writer = new JsonTextWriter(new StreamWriter(stream)))
            {
                
            }
        }

        public void Serialize(Stream stream, IEnumerable<Span> spans)
        {
            
        }

        private void SpanToJson(JsonTextWriter writer, Span span)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(TraceId);
            writer.WriteValue(span.TypedContext.TraceId.ToString());
            writer.WritePropertyName(SpanId);
            writer.WriteValue(span.TypedContext.SpanId.ToString("x16"));
            if (span.TypedContext.ParentId.HasValue)
            {
                writer.WritePropertyName(ParentId);
                writer.WriteValue(span.TypedContext.ParentId.ToString("x16"));
            }
            writer.WritePropertyName(OperationName);
            writer.WriteValue(span.OperationName);
        }
    }
}
