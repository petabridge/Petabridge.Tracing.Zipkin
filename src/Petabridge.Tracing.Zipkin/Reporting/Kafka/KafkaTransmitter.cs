using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace Petabridge.Tracing.Zipkin.Reporting.Kafka
{
    /// <summary>
    /// INTERNAL API. Used to carry out serialization and transmission to Kafka.
    /// </summary>
    public sealed class KafkaTransmitter : IDisposable
    {
        private readonly string _topicName;
        private readonly Producer<Null, byte[]> _producer;
        private readonly ISpanSerializer _serializer;

        public KafkaTransmitter(string topicName, Producer<Null, byte[]> producer, ISpanSerializer serializer)
        {
            _producer = producer;
            _serializer = serializer;
            _topicName = topicName;
        }

        public async Task<Message<Null, byte[]>> TransmitSpans(IEnumerable<Span> spans)
        {
            using (var stream = SerializationStreamManager.StreamManager.GetStream("Petabridge.Tracing.Zipkin.KafkaTransmitter"))
            {
                _serializer.Serialize(stream, spans);
                var outboundBytes = stream.ToArray();
                return await _producer.ProduceAsync(_topicName, null, outboundBytes).ConfigureAwait(false);
            }
        }

        public void Dispose()
        {
            _producer?.Dispose();
        }
    }
}
