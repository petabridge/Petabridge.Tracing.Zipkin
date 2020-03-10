// -----------------------------------------------------------------------
// <copyright file="ZipkinKafkaSpanReporter.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2019 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Configuration;
using Akka.Util.Internal;

namespace Petabridge.Tracing.Zipkin.Reporting.Kafka
{
    /// <summary>
    ///     Kafka <see cref="Producer" /> span reporter implementation.
    /// </summary>
    public sealed class ZipkinKafkaSpanReporter : ISpanReporter
    {
        private static readonly AtomicCounter NameCounter = new AtomicCounter(0);

        private static readonly Config NormalHocon = ConfigurationFactory.Empty;
        private static readonly Config DebugHocon = ConfigurationFactory.ParseString(@"akka.loglevel = DEBUG");
        private readonly ActorSystem _ownedActorSystem; // if we own the ActorSystem, we have to kill it.
        private readonly IActorRef _reporterActorRef;

        internal ZipkinKafkaSpanReporter(IActorRef reporterActorRef, ActorSystem ownedActorSystem = null)
        {
            _reporterActorRef = reporterActorRef;
            _ownedActorSystem = ownedActorSystem;
        }

        public void Dispose()
        {
            // give it a chance to cleanup
            _reporterActorRef.GracefulStop(TimeSpan.FromSeconds(5)).Wait();

            // then optionally terminate ActorSystem
            _ownedActorSystem?.Terminate().Wait();
        }

        public void Report(Span span)
        {
            _reporterActorRef.Tell(span);
        }

        /// <summary>
        ///     Performs all of the setup and initialization needed to get the Zipkin Kafka reporting engine up and running.
        /// </summary>
        /// <param name="kafkaBrokerEndpoint">A single kafka broker endpoint.</param>
        /// <param name="actorSystem">
        ///     Optional. If using Akka.NET, you can hook your own <see cref="ActorSystem" /> into our
        ///     reporting engine.
        /// </param>
        /// <returns></returns>
        public static ZipkinKafkaSpanReporter Create(string kafkaBrokerEndpoint,
            ActorSystem actorSystem = null)
        {
            return Create(new[] {kafkaBrokerEndpoint}, actorSystem);
        }

        /// <summary>
        ///     Performs all of the setup and initialization needed to get the Zipkin Kafka reporting engine up and running.
        /// </summary>
        /// <param name="kafkaBrokerEndpoints">A list of kafka broker endpoints.</param>
        /// <param name="actorSystem">
        ///     Optional. If using Akka.NET, you can hook your own <see cref="ActorSystem" /> into our
        ///     reporting engine.
        /// </param>
        /// <returns></returns>
        public static ZipkinKafkaSpanReporter Create(IReadOnlyList<string> kafkaBrokerEndpoints,
            ActorSystem actorSystem = null)
        {
            return Create(new ZipkinKafkaReportingOptions(kafkaBrokerEndpoints), actorSystem);
        }

        /// <summary>
        ///     Performs all of the setup and initialization needed to get the Zipkin Kafka reporting engine up and running.
        /// </summary>
        /// <param name="options">The set of options for configuring timeouts and batch sizes.</param>
        /// <param name="actorSystem">
        ///     Optional. If using Akka.NET, you can hook your own <see cref="ActorSystem" /> into our
        ///     reporting engine.
        /// </param>
        /// <returns></returns>
        public static ZipkinKafkaSpanReporter Create(ZipkinKafkaReportingOptions options,
            ActorSystem actorSystem = null)
        {
            // force this component to explode if the end-user screwed up the URI somehow.
            var weOwnActorSystem = false;

            if (actorSystem == null) // create our own ActorSystem if it doesn't already exist.
            {
                weOwnActorSystem = true;
                actorSystem = ActorSystem.Create("pbzipkin",
                    options.DebugLogging ? DebugHocon : NormalHocon);
            }

            // spawn as a System actor, so in the event of being in a non-owned system our traces get shut down
            // only after all of the user-defined actors have terminated.
            var zipkinActor = actorSystem.AsInstanceOf<ExtendedActorSystem>().SystemActorOf(
                Props.Create(() => new KafkaReportingActor(options)),
                $"zipkin-tracing-kafka-{NameCounter.GetAndIncrement()}");

            return new ZipkinKafkaSpanReporter(zipkinActor, weOwnActorSystem ? actorSystem : null);
        }
    }
}