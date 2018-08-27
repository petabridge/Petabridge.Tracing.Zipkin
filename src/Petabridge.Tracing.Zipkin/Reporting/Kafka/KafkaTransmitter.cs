// -----------------------------------------------------------------------
// <copyright file="KafkaTransmitter.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Petabridge.Tracing.Zipkin.Reporting.Kafka
{
    /// <summary>
    ///     INTERNAL API. Used to carry out serialization and transmission to Kafka.
    /// </summary>
    public sealed class KafkaTransmitter : IDisposable
    {
        private readonly Producer<Null, byte[]> _producer;
        private readonly ISpanSerializer _serializer;
        private readonly string _topicName;

        public KafkaTransmitter(string topicName, Producer<Null, byte[]> producer, ISpanSerializer serializer)
        {
            _producer = producer;
            _serializer = serializer;
            _topicName = topicName;
        }

        public void Dispose()
        {
            _producer?.Dispose();
        }

        public async Task<Message<Null, byte[]>> TransmitSpans(IEnumerable<Span> spans)
        {
            using (var stream =
                SerializationStreamManager.StreamManager.GetStream("Petabridge.Tracing.Zipkin.KafkaTransmitter"))
            {
                _serializer.Serialize(stream, spans);
                var outboundBytes = stream.ToArray();
                return await _producer.ProduceAsync(_topicName, null, outboundBytes).ConfigureAwait(false);
            }
        }
    }
}