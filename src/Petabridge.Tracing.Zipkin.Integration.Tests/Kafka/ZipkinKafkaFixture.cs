using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Akka.Util;
using Docker.DotNet;
using Docker.DotNet.Models;
using Xunit;

namespace Petabridge.Tracing.Zipkin.Integration.Tests.Kafka
{
    public class ZipkinKafkaFixture : ZipkinFixture
    {
        protected const string KafkaImageName = "openzipkin/zipkin-kafka";
        protected readonly string KafkaContainerName = $"zookeeper-kafka-{Guid.NewGuid():N}";

        public string KafkaUri { get; private set; }

        public int KafkaInternalPort { get; private set; }

        public int KafkaExternalPort { get; private set; }

        public int KafkaZooKeeperPort { get; private set; }

        public override async Task InitializeAsync()
        {
            KafkaUri = await StartKafka();

            /*
             * Use the internal Docker port for the binding here, not the host-visible port mapping.
             */
            ZipkinUrl = await StartZipkinContainer(new[]{ KafkaContainerName }, new[]{$"KAFKA_BOOTSTRAP_SERVERS={KafkaContainerName}:9092" });
            await Task.Delay(TimeSpan.FromSeconds(20));
        }

        private async Task<string> StartKafka()
        {
            var images = await _client.Images.ListImagesAsync(new ImagesListParameters {MatchName = KafkaImageName});
            if (images.Count == 0)
                await _client.Images.CreateImageAsync(
                    new ImagesCreateParameters {FromImage = KafkaImageName, Tag = "latest"}, null,
                    new Progress<JSONMessage>(message =>
                    {
                        Console.WriteLine(!string.IsNullOrEmpty(message.ErrorMessage)
                            ? message.ErrorMessage
                            : $"{message.ID} {message.Status} {message.ProgressMessage}");
                    }));

            KafkaInternalPort = ThreadLocalRandom.Current.Next(7000, 9000);
            KafkaExternalPort = 19092; //ThreadLocalRandom.Current.Next(19000, 20000);
            KafkaZooKeeperPort = ThreadLocalRandom.Current.Next(2000, 2999);

            // create the container
            await _client.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Image = KafkaImageName,
                Name = KafkaContainerName,
                Tty = true,
                HostConfig = new HostConfig
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        {
                            "19092/tcp",
                            new List<PortBinding>
                            {
                                new PortBinding
                                {
                                    HostPort = $"{KafkaExternalPort}"
                                }
                            }
                        },
                        {
                            "9092/tcp",
                            new List<PortBinding>
                            {
                                new PortBinding
                                {
                                    HostPort = $"{KafkaInternalPort}"
                                }
                            }
                        },
                        {
                            "2181/tcp",
                            new List<PortBinding>
                            {
                                new PortBinding
                                {
                                    HostPort = $"{KafkaZooKeeperPort}"
                                }
                            }
                        }
                    }
                }
            });

            // start the container
            await _client.Containers.StartContainerAsync(KafkaContainerName, new ContainerStartParameters());
            return $"localhost:{KafkaExternalPort}";
        }

        public override async Task DisposeAsync()
        {
            if (_client != null)
            {
                var stop1 =  _client.Containers.StopContainerAsync(_zipkinContainerName, new ContainerStopParameters());
                var stop2 = _client.Containers.StopContainerAsync(KafkaContainerName, new ContainerStopParameters());
                await Task.WhenAll(stop1, stop2);

                var remove1 = _client.Containers.RemoveContainerAsync(_zipkinContainerName,
                    new ContainerRemoveParameters { Force = true });
                var remove2 = _client.Containers.RemoveContainerAsync(KafkaContainerName,
                    new ContainerRemoveParameters { Force = true });
                await Task.WhenAll(remove1, remove2);

                _client.Dispose();
            }
        }
    }
}
