// -----------------------------------------------------------------------
// <copyright file="Span.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace Petabridge.Tracing.Zipkin.Integration.Tests.Serialization
{
    public class LocalEndpoint
    {
        public string serviceName { get; set; }
        public string ipv4 { get; set; }
        public string ipv6 { get; set; }
        public int port { get; set; }
    }

    public class RemoteEndpoint
    {
        public string serviceName { get; set; }
        public string ipv4 { get; set; }
        public string ipv6 { get; set; }
        public int port { get; set; }
    }

    public class Annotation
    {
        public long timestamp { get; set; }
        public string value { get; set; }
    }

    public class Tags
    {
        public string additionalProp1 { get; set; }
        public string additionalProp2 { get; set; }
        public string additionalProp3 { get; set; }
    }

    public class Span
    {
        public string traceId { get; set; }
        public string name { get; set; }
        public string parentId { get; set; }
        public string id { get; set; }
        public string kind { get; set; }
        public long timestamp { get; set; }
        public long duration { get; set; }
        public bool debug { get; set; }
        public bool shared { get; set; }
        public LocalEndpoint localEndpoint { get; set; }
        public RemoteEndpoint remoteEndpoint { get; set; }
        public List<Annotation> annotations { get; set; }
        public Tags tags { get; set; }
    }
}