// -----------------------------------------------------------------------
// <copyright file="MockZipkinTracer.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using OpenTracing;
using OpenTracing.Propagation;
using Petabridge.Tracing.Zipkin.Tracers.NoOp;
using Petabridge.Tracing.Zipkin.Util;

namespace Petabridge.Tracing.Zipkin.Tracers
{
    /// <summary>
    ///     Used for unit-testing Petabridge.Tracing.Zipkin.
    /// </summary>
    public sealed class MockZipkinTracer : IZipkinTracer
    {
        public MockZipkinTracer() : this(Endpoint.Testing, new DateTimeOffsetTimeProvider(),
            ThreadLocalRngSpanIdProvider.TraceId128BitProvider, NoOpScopeManager.Instance)
        {
        }

        public MockZipkinTracer(Endpoint localEndpoint, ITimeProvider timeProvider, ISpanIdProvider idProvider,
            IScopeManager scopeManager)
        {
            LocalEndpoint = localEndpoint;
            TimeProvider = timeProvider;
            IdProvider = idProvider;
            ScopeManager = scopeManager;
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

        public ISpanBuilder BuildSpan(string operationName)
        {
            throw new NotImplementedException();
        }

        public void Inject<TCarrier>(ISpanContext spanContext, IFormat<TCarrier> format, TCarrier carrier)
        {
            throw new NotImplementedException();
        }

        public ISpanContext Extract<TCarrier>(IFormat<TCarrier> format, TCarrier carrier)
        {
            throw new NotImplementedException();
        }

        public IScopeManager ScopeManager { get; }
        public ISpan ActiveSpan => ScopeManager.Active.Span;
    }
}