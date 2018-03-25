// -----------------------------------------------------------------------
// <copyright file="MockZipkinTracer.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Concurrent;
using OpenTracing;
using OpenTracing.Propagation;
using Petabridge.Tracing.Zipkin.Exceptions;
using Petabridge.Tracing.Zipkin.Propagation;
using Petabridge.Tracing.Zipkin.Tracers.NoOp;
using Petabridge.Tracing.Zipkin.Util;

namespace Petabridge.Tracing.Zipkin.Tracers
{
    /// <summary>
    ///     Used for unit-testing Petabridge.Tracing.Zipkin.
    /// </summary>
    public sealed class MockZipkinTracer : IZipkinTracer
    {
        private readonly IPropagator<ITextMap> _propagator;

        public MockZipkinTracer(Endpoint localEndpoint = null, ITimeProvider timeProvider = null,
            ISpanIdProvider idProvider = null,
            IScopeManager scopeManager = null, IPropagator<ITextMap> propagtor = null)
        {
            LocalEndpoint = localEndpoint ?? Endpoint.Testing;
            TimeProvider = timeProvider ?? new DateTimeOffsetTimeProvider();
            IdProvider = idProvider ?? ThreadLocalRngSpanIdProvider.TraceId128BitProvider;
            ScopeManager = scopeManager ?? NoOpScopeManager.Instance;
            _propagator = propagtor ?? new B3Propagator();
            CollectedSpans = new ConcurrentQueue<Span>();
        }

        public ConcurrentQueue<Span> CollectedSpans { get; }

        public Endpoint LocalEndpoint { get; }
        public ITimeProvider TimeProvider { get; }
        public ISpanIdProvider IdProvider { get; }

        public void Report(Span span)
        {
            CollectedSpans.Enqueue(span);
        }

        public IZipkinSpanBuilder BuildSpan(string operationName)
        {
            return new SpanBuilder(this, operationName);
        }

        ISpanBuilder ITracer.BuildSpan(string operationName)
        {
            return BuildSpan(operationName);
        }

        public void Inject<TCarrier>(ISpanContext spanContext, IFormat<TCarrier> format, TCarrier carrier)
        {
            if ((format == BuiltinFormats.TextMap || format == BuiltinFormats.TextMap) && carrier is ITextMap textMap)
                _propagator.Inject((SpanContext) spanContext, textMap);

            throw new ZipkinFormatException(
                $"Unrecognized carrier format [{format}]. Only ITextMap is supported by this driver.");
        }

        public ISpanContext Extract<TCarrier>(IFormat<TCarrier> format, TCarrier carrier)
        {
            if ((format == BuiltinFormats.TextMap || format == BuiltinFormats.TextMap) && carrier is ITextMap textMap)
                return _propagator.Extract(textMap);

            throw new ZipkinFormatException(
                $"Unrecognized carrier format [{format}]. Only ITextMap is supported by this driver.");
        }

        public IScopeManager ScopeManager { get; }
        public ISpan ActiveSpan => ScopeManager.Active.Span;
    }
}