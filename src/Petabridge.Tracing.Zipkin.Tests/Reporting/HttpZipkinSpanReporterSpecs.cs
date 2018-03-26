// -----------------------------------------------------------------------
// <copyright file="HttpZipkinSpanReporterSpecs.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using Akka.TestKit.Xunit2;
using FluentAssertions;
using Petabridge.Tracing.Zipkin.Reporting.Http;
using Xunit;

namespace Petabridge.Tracing.Zipkin.Tests.Reporting
{
    public class HttpZipkinSpanReporterSpecs : TestKit
    {
        [Fact(DisplayName = "ZipkinHttpSpanReporter should not shut down an ActorSystem it does not own upon Dispose")]
        public void ShouldNotShutDownNonOwnedActorSystem()
        {
            var zipkinSpanReporter =
                ZipkinHttpSpanReporter.Create(new ZipkinHttpReportingOptions("localhost:9311"), Sys);
            zipkinSpanReporter.Dispose();
            Sys.WhenTerminated.IsCompleted.Should().BeFalse();
        }
    }
}