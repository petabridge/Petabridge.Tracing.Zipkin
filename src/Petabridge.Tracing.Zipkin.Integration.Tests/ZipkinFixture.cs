using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Akka.Util;
using Docker.DotNet;
using Docker.DotNet.Models;
using Xunit;

namespace Petabridge.Tracing.Zipkin.Integration.Tests
{
    public class ZipkinFixture : IAsyncLifetime
    {
        private DockerClient _client;
        private readonly string _zipkinContainerName = $"zipkin-{Guid.NewGuid():N}";
        private const string ZipkinImageName = "openzipkin/zipkin";

        public string ZipkinUrl { get; private set; }

        public async Task InitializeAsync()
        {
            DockerClientConfiguration config;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                config = new DockerClientConfiguration(new Uri("unix:///var/run/docker.sock"));
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                config = new DockerClientConfiguration(new Uri("npipe://./pipe/docker_engine"));
            }
            else
            {
                throw new NotSupportedException($"Unsupported OS [{RuntimeInformation.OSDescription}]");
            }

            _client = config.CreateClient();

            var images = await _client.Images.ListImagesAsync(new ImagesListParameters { MatchName = ZipkinImageName });
            if (images.Count == 0)
            {
                // No image found. Pulling latest ..
                await _client.Images.CreateImageAsync(new ImagesCreateParameters { FromImage = ZipkinImageName, Tag = "latest" }, null, new Progress<JSONMessage>());
            }

            var zipkinHttpPort = ThreadLocalRandom.Current.Next(9000, 10000);

            // create the container
            await _client.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Image = ZipkinImageName,
                Name = _zipkinContainerName,
                Tty = true,
                HostConfig = new HostConfig
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        {
                            "9411/tcp",
                            new List<PortBinding>
                            {
                                new PortBinding
                                {
                                    HostPort = $"{zipkinHttpPort}"
                                }
                            }
                        }
                    }
                }
            });

            // start the container
            await _client.Containers.StartContainerAsync(_zipkinContainerName, new ContainerStartParameters() { });
            ZipkinUrl = $"localhost:{zipkinHttpPort}";
            await Task.Delay(TimeSpan.FromSeconds(5));
        }

        public async Task DisposeAsync()
        {
            if (_client != null)
            {
                await _client.Containers.StopContainerAsync(_zipkinContainerName, new ContainerStopParameters() { });
                await _client.Containers.RemoveContainerAsync(_zipkinContainerName,
                    new ContainerRemoveParameters() {Force = true});
                _client.Dispose();
            }
        }
    }
}
