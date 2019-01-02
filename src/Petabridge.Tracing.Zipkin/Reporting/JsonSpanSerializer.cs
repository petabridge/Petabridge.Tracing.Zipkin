﻿// -----------------------------------------------------------------------
// <copyright file="JsonSpanSerializer.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2019 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Petabridge.Tracing.Zipkin.Util;

namespace Petabridge.Tracing.Zipkin.Reporting
{
    /*
     * TODO: rewrite this without using Json.NET (nice to have)
     */
    /// <summary>
    ///     Serializes <see cref="Span" /> objects into JSON format for delivery
    ///     over one of the registered <see cref="ISpanReporter" /> implementations.
    /// </summary>
    /// <remarks>
    ///     Based on the Zipkin V2 API described here: https://zipkin.io/zipkin-api/#/default/post_spans
    /// </remarks>
    public sealed class JsonSpanSerializer : ISpanSerializer
    {
        internal const string TraceId = "traceId";
        internal const string SpanId = "id";
        internal const string ParentId = "parentId";
        internal const string OperationName = "name";
        internal const string Timestamp = "timestamp";
        internal const string Debug = "debug";
        internal const string Duration = "duration";
        internal const string LocalEndpoint = "localEndpoint";
        internal const string RemoteEndpoint = "remoteEndpoint";
        internal const string ServiceName = "serviceName";
        internal const string Ipv4 = "ipv4";
        internal const string Ipv6 = "ipv6";
        internal const string Port = "port";
        internal const string Kind = "kind";
        internal const string Tags = "tags";
        internal const string Annotations = "annotations";
        internal const string Value = "value";

        public void Serialize(Stream stream, Span span)
        {
            using (var writer = new JsonTextWriter(new StreamWriter(stream, Encoding.Default, 2048, true)))
            {
                writer.WriteStartArray();
                SpanToJson(writer, span);
                writer.WriteEndArray();
            }
        }

        public void Serialize(Stream stream, IEnumerable<Span> spans)
        {
            using (var writer = new JsonTextWriter(new StreamWriter(stream, Encoding.Default, 2048, true)))
            {
                writer.WriteStartArray();
                foreach (var span in spans)
                    SpanToJson(writer, span);
                writer.WriteEndArray();
            }
        }

        /// <summary>
        ///     Serializes the Span to JSON.
        /// </summary>
        /// <param name="writer">The JSON Writer.</param>
        /// <param name="span">The Span.</param>
        /// <remarks>
        ///     Implementation note, for performance and correctness:
        ///     1. Don't emit null fields at all if the value isn't populated.
        ///     No need for that space in the content on the wire.
        ///     2. Design this method to be repeatable for multiple spans in case
        ///     we want to batch.
        /// </remarks>
        private static void SpanToJson(JsonTextWriter writer, Span span)
        {
            writer.WriteStartObject();

            // meta-data
            writer.WritePropertyName(TraceId);
            writer.WriteValue(span.TypedContext.TraceId);
            writer.WritePropertyName(SpanId);
            var spanId = span.TypedContext.SpanId;
            writer.WriteValue(spanId);
            if (!string.IsNullOrEmpty(span.TypedContext.ParentId))
            {
                writer.WritePropertyName(ParentId);
                writer.WriteValue(span.TypedContext.ParentId);
            }

            writer.WritePropertyName(OperationName);
            writer.WriteValue(span.OperationName);

            // timing
            writer.WritePropertyName(Timestamp);
            writer.WriteValue(span.Started.ToUnixMicros());

            if (span.Duration.HasValue)
            {
                writer.WritePropertyName(Duration);
                writer.WriteValue(span.Duration.Value.ToMicros());
            }

            // special flags
            writer.WritePropertyName(Debug);
            writer.WriteValue(span.Debug);

            if (span.SpanKind.HasValue)
            {
                writer.WritePropertyName(Kind);
                switch (span.SpanKind)
                {
                    case SpanKind.CONSUMER:
                        writer.WriteValue("CONSUMER");
                        break;
                    case SpanKind.PRODUCER:
                        writer.WriteValue("PRODUCER");
                        break;
                    case SpanKind.CLIENT:
                        writer.WriteValue("CLIENT");
                        break;
                    case SpanKind.SERVER:
                        writer.WriteValue("SERVER");
                        break;
                    default:
                        throw new NotSupportedException($"Unknown span kind: [{span.SpanKind}]");
                }
            }

            // endpoints
            WriteEndpoint(writer, true, span.LocalEndpoint);

            if (span.RemoteEndpoint != null)
                WriteEndpoint(writer, false, span.RemoteEndpoint);

            // tags
            if (span.Tags.Any())
                WriteTags(writer, span.Tags);

            // annotations
            if (span.Annotations.Any())
                WriteAnnotations(writer, span.Annotations);

            writer.WriteEndObject();
        }

        private static void WriteAnnotations(JsonTextWriter writer, IReadOnlyList<Annotation> annotations)
        {
            writer.WritePropertyName(Annotations);
            writer.WriteStartArray();
            foreach (var a in annotations)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(Timestamp);
                writer.WriteValue(a.Timestamp.ToUnixMicros());
                writer.WritePropertyName(Value);
                writer.WriteValue(a.Value);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        private static void WriteTags(JsonTextWriter writer, IReadOnlyDictionary<string, string> tags)
        {
            writer.WritePropertyName(Tags);
            writer.WriteStartObject();
            foreach (var tag in tags)
            {
                writer.WritePropertyName(tag.Key);
                writer.WriteValue(tag.Value);
            }

            writer.WriteEndObject();
        }

        private static void WriteEndpoint(JsonTextWriter writer, bool local, Endpoint ep)
        {
            writer.WritePropertyName(local ? LocalEndpoint : RemoteEndpoint);
            writer.WriteStartObject();
            writer.WritePropertyName(ServiceName);
            writer.WriteValue(ep.ServiceName);
            if (!string.IsNullOrEmpty(ep.Host))
            {
                writer.WritePropertyName(Ipv4); // N.B. - don't really care what the IP family is.
                writer.WriteValue(ep.Host);
                writer.WritePropertyName(Ipv6);
                writer.WriteValue(ep.Host);
            }

            if (ep.Port != null)
            {
                writer.WritePropertyName(Port);
                writer.WriteValue(ep.Port);
            }

            writer.WriteEndObject();
        }
    }
}