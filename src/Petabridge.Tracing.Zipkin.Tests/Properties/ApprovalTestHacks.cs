#if !NETCOREAPP2_0
using ApprovalTests.Reporters;

[assembly: FrontLoadedReporter(typeof(HackedDefaultFrontLoaderReporter))]
#endif