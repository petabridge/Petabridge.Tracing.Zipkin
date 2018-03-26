// -----------------------------------------------------------------------
// <copyright file="ZipkinHttpApiTransmitter.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IO;

namespace Petabridge.Tracing.Zipkin.Reporting.Http
{
    /// <summary>
    ///     INTERNAL API. Used for transmitting data to the Zipkin HTTP API.
    /// </summary>
    public sealed class ZipkinHttpApiTransmitter
    {
        /// <summary>
        ///     Using JSON.
        /// </summary>
        public const string MediaType = "application/json";

        /// <summary>
        ///     Public Uri on any Zipkin instance, which can be used for posting spans.
        /// </summary>
        public const string SpanPostUriPath = "api/v2/spans";

        /// <summary>
        ///     Only need one of these globally, and it's thread-safe. The streams it accesses internally are inherently not safe.
        /// </summary>
        private static readonly RecyclableMemoryStreamManager StreamManager = new RecyclableMemoryStreamManager();

        private readonly HttpClient _client;

        private readonly ISpanSerializer _serializer = new JsonSpanSerializer();

        public ZipkinHttpApiTransmitter(HttpClient client, Uri uri)
        {
            _client = client;
            Uri = uri;
        }

        public Uri Uri { get; }

        public async Task<HttpResponseMessage> TransmitSpans(IEnumerable<Span> spans, TimeSpan timeout)
        {
            using (var stream = StreamManager.GetStream("Petabridge.Tracing.Zipkin.HttpTransmitter"))
            {
                _serializer.Serialize(stream, spans);
                var cts = new CancellationTokenSource(timeout);
                stream.Position = 0;
                var content = new StreamContent(stream);
                content.Headers.Add("Content-Type", MediaType);
                content.Headers.Add("Content-Length", stream.Length.ToString());
                return await _client.PostAsync(Uri, content, cts.Token);
            }
        }

        /// <summary>
        ///     Constructs the full Zipkin HTTP Uri
        /// </summary>
        /// <param name="fullHostname">The full Zipkin HTTP host.</param>
        /// <returns>A valid route to the Zipkin HTTP Uri for posting <see cref="Span" /> instances.</returns>
        public static Uri GetFullZipkinUri(string fullHostname)
        {
            return new Uri(new Uri(fullHostname, UriKind.Absolute), new Uri(SpanPostUriPath, UriKind.Relative));
        }
    }
}