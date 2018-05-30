// -----------------------------------------------------------------------
// <copyright file="ZipkinDeserializer.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Petabridge.Tracing.Zipkin.Integration.Tests.Serialization
{
    /// <summary>
    ///     Deserializes JSON responses from Zipkin's HTTP API
    /// </summary>
    public static class ZipkinDeserializer
    {
        public static IReadOnlyList<Span> Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<List<Span>>(json);
        }
    }
}