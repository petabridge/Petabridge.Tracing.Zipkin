// -----------------------------------------------------------------------
// <copyright file="ZipkinHttpSpanReporter.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using Akka.Actor;
using Akka.Configuration;
using Akka.Util.Internal;

namespace Petabridge.Tracing.Zipkin.Reporting.Http
{
    /// <summary>
    ///     Reporting engine for working with Zipkin spans and the Zipkin HTTP API.
    ///     Safe to use across multiple <see cref="IZipkinTracer" /> instances. Highly concurrent and memory-efficient.
    /// </summary>
    public sealed class ZipkinHttpSpanReporter : ISpanReporter
    {
        private static readonly AtomicCounter NameCounter = new AtomicCounter(0);

        private static readonly Config NormalHocon = ConfigurationFactory.Empty;
        private static readonly Config DebugHocon = ConfigurationFactory.ParseString(@"akka.loglevel = DEBUG");
        private readonly ActorSystem _ownedActorSystem; // if we own the ActorSystem, we have to kill it.
        private readonly IActorRef _reporterActorRef;

        internal ZipkinHttpSpanReporter(IActorRef reporterRef, ActorSystem ownedActorSystem = null)
        {
            _reporterActorRef = reporterRef;
            _ownedActorSystem = ownedActorSystem;
        }

        public void Report(Span span)
        {
            _reporterActorRef.Tell(span);
        }

        public void Dispose()
        {
            // give it a chance to cleanup
            _reporterActorRef.GracefulStop(TimeSpan.FromSeconds(5)).Wait();

            // then optionally terminate ActorSystem
            _ownedActorSystem?.Terminate().Wait();
        }

        /// <summary>
        ///     Performs all of the setup and initialization needed to get the Zipkin HTTP reporting engine up and running.
        /// </summary>
        /// <param name="options">The set of options for configuring timeouts and batch sizes.</param>
        /// <param name="actorSystem">
        ///     Optional. If using Akka.NET, you can hook your own <see cref="ActorSystem" /> into our
        ///     reporting engine.
        /// </param>
        /// <returns></returns>
        public static ZipkinHttpSpanReporter Create(ZipkinHttpReportingOptions options, ActorSystem actorSystem = null)
        {
            // force this component to explode if the end-user screwed up the URI somehow.
            var uri = ZipkinHttpApiTransmitter.GetFullZipkinUri(options.ZipkinUrl);
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
                Props.Create(() => new HttpReportingActor(options)), $"zipkin-tracing-{NameCounter.GetAndIncrement()}");

            return new ZipkinHttpSpanReporter(zipkinActor, weOwnActorSystem ? actorSystem : null);
        }
    }
}