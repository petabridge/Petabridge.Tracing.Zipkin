// -----------------------------------------------------------------------
// <copyright file="ZipkinFixture.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2019 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Akka.Util;
using Docker.DotNet;
using Docker.DotNet.Models;
using Xunit;

namespace Petabridge.Tracing.Zipkin.Integration.Tests
{
    public class ZipkinFixture : IAsyncLifetime
    {
        protected const string ZipkinImageName = "openzipkin/zipkin";
        protected readonly string _zipkinContainerName = $"zipkin-{Guid.NewGuid():N}";
        protected DockerClient _client;

        public ZipkinFixture()
        {
            DockerClientConfiguration config;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                config = new DockerClientConfiguration(new Uri("unix://var/run/docker.sock"));
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                config = new DockerClientConfiguration(new Uri("npipe://./pipe/docker_engine"));
            else
                throw new NotSupportedException($"Unsupported OS [{RuntimeInformation.OSDescription}]");

            _client = config.CreateClient();
        }

        public string ZipkinUrl { get; protected set; }

        public virtual async Task InitializeAsync()
        {
            ZipkinUrl = await StartZipkinContainer(null, null);
            await Task.Delay(TimeSpan.FromSeconds(20));
        }

        public virtual async Task DisposeAsync()
        {
            if (_client != null)
            {
                await _client.Containers.StopContainerAsync(_zipkinContainerName, new ContainerStopParameters());
                await _client.Containers.RemoveContainerAsync(_zipkinContainerName,
                    new ContainerRemoveParameters {Force = true});
                _client.Dispose();
            }
        }

        protected async Task<string> StartZipkinContainer(string[] links, string[] environmentArgs)
        {
            var images = await _client.Images.ListImagesAsync(new ImagesListParameters {MatchName = ZipkinImageName});
            if (images.Count == 0)
                await _client.Images.CreateImageAsync(
                    new ImagesCreateParameters {FromImage = ZipkinImageName, Tag = "latest"}, null,
                    new Progress<JSONMessage>(message =>
                    {
                        Console.WriteLine(!string.IsNullOrEmpty(message.ErrorMessage)
                            ? message.ErrorMessage
                            : $"{message.ID} {message.Status} {message.ProgressMessage}");
                    }));

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
                    },
                    Links = links
                },
                Env = environmentArgs
            });

            // start the container
            await _client.Containers.StartContainerAsync(_zipkinContainerName, new ContainerStartParameters());
            return $"localhost:{zipkinHttpPort}";
        }
    }
}