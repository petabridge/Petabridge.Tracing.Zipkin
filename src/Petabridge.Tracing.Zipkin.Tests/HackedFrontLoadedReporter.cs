#if !NETCOREAPP2_0
using System;
using System.IO;
using System.Reflection;
using System.Text;
using ApprovalTests;
using ApprovalTests.Core;
using ApprovalTests.Reporters;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Petabridge.Tracing.Zipkin.Reporting;
using Petabridge.Tracing.Zipkin.Tracers;
using Xunit;

/// <summary>
/// This class is a hack to allow approvaltests handle xunit2 correctly on Teamcity. Once 
/// this https://github.com/approvals/ApprovalTests.Net/pull/171 is in nuget, 
/// this class and its usages can be removed
/// </summary>
public class HackedDefaultFrontLoaderReporter : FirstWorkingReporter
{
    public HackedDefaultFrontLoaderReporter()
        : base(HackedTeamCityReporter.INSTANCE, NCrunchReporter.INSTANCE)
    {
    }

    public class HackedTeamCityReporter : IEnvironmentAwareReporter
    {
        public static readonly HackedTeamCityReporter INSTANCE = new HackedTeamCityReporter();

        public void Report(string approved, string received)
        {
            var reporter = HackedFrameworkAssertReporter.INSTANCE;
            if (reporter.IsWorkingInThisEnvironment(received))
            {
                reporter.Report(approved, received);
            }
        }

        public bool IsWorkingInThisEnvironment(string forFile)
        {
            return Environment.GetEnvironmentVariable("TEAMCITY_PROJECT_NAME") != null;
        }
    }

    public class HackedFrameworkAssertReporter : FirstWorkingReporter
    {
        public static readonly HackedFrameworkAssertReporter INSTANCE = new HackedFrameworkAssertReporter();

        public HackedFrameworkAssertReporter()
            : base(MsTestReporter.INSTANCE,
                NUnitReporter.INSTANCE,
                XUnit2Reporter.INSTANCE,
                XUnitReporter.INSTANCE
            )
        {
        }
    }
}
#endif