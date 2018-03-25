// -----------------------------------------------------------------------
// <copyright file="Endpoint.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using Petabridge.Tracing.Zipkin.Tracers;

namespace Petabridge.Tracing.Zipkin
{
    /// <inheritdoc />
    /// <summary>
    ///     Describes the endpoint of the service that is producing traces.
    /// </summary>
    public sealed class Endpoint : IEquatable<Endpoint>
    {
        /// <summary>
        ///     Singleton used inside the <see cref="MockZipkinTracer" /> for testing purposes.
        /// </summary>
        public static readonly Endpoint Testing = new Endpoint("testing", "testing", 0);

        public Endpoint(string serviceName, string host, int port)
        {
            ServiceName = serviceName;
            Host = host;
            Port = port;
        }

        /// <summary>
        ///     The friendly name of this service.
        /// </summary>
        public string ServiceName { get; }

        /// <summary>
        ///     Hostname or IP address.
        /// </summary>
        public string Host { get; }

        /// <summary>
        ///     The port number.
        /// </summary>
        public int Port { get; }

        public bool Equals(Endpoint other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(ServiceName, other.ServiceName) && string.Equals(Host, other.Host) &&
                   Port == other.Port;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Endpoint && Equals((Endpoint) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ServiceName.GetHashCode();
                hashCode = (hashCode * 397) ^ Host.GetHashCode();
                hashCode = (hashCode * 397) ^ Port;
                return hashCode;
            }
        }

        public static bool operator ==(Endpoint left, Endpoint right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Endpoint left, Endpoint right)
        {
            return !Equals(left, right);
        }
    }
}