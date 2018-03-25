using System.Collections.Concurrent;
using OpenTracing;
using OpenTracing.Propagation;
using Petabridge.Tracing.Zipkin.Util;

namespace Petabridge.Tracing.Zipkin.Tracers
{
    /// <summary>
    /// Used for unit-testing Petabridge.Tracing.Zipkin.
    /// </summary>
    public sealed class MockZipkinTracer : IZipkinTracer
    {
        public MockZipkinTracer(ISpanIdProvider idProvider) : this(Endpoint.Testing, new DateTimeOffsetTimeProvider(), idProvider) { }

        public MockZipkinTracer(Endpoint localEndpoint, ITimeProvider timeProvider, ISpanIdProvider idProvider)
        {
            LocalEndpoint = localEndpoint;
            TimeProvider = timeProvider;
            IdProvider = idProvider;
            CollectedSpans = new ConcurrentQueue<Span>();
        }

        public Endpoint LocalEndpoint { get; }
        public ITimeProvider TimeProvider { get; }
        public ISpanIdProvider IdProvider { get; }

        public void Report(Span span)
        {
            CollectedSpans.Enqueue(span);
        }

        public ConcurrentQueue<Span> CollectedSpans { get; private set; }

        public ISpanBuilder BuildSpan(string operationName)
        {
            throw new System.NotImplementedException();
        }

        public void Inject<TCarrier>(ISpanContext spanContext, IFormat<TCarrier> format, TCarrier carrier)
        {
            throw new System.NotImplementedException();
        }

        public ISpanContext Extract<TCarrier>(IFormat<TCarrier> format, TCarrier carrier)
        {
            throw new System.NotImplementedException();
        }

        public IScopeManager ScopeManager { get; }
        public ISpan ActiveSpan => ScopeManager.Active.Span;
    }
}