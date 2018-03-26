using System;
using System.Collections.Generic;
using System.Text;
using OpenTracing;
using OpenTracing.Propagation;
using Petabridge.Tracing.Zipkin.Exceptions;
using Petabridge.Tracing.Zipkin.Propagation;
using Petabridge.Tracing.Zipkin.Tracers.NoOp;
using Petabridge.Tracing.Zipkin.Util;

namespace Petabridge.Tracing.Zipkin
{
    /// <summary>
    /// Standard <see cref="ITracer"/> implementation for working with Zipkin.
    /// </summary>
    public sealed class ZipkinTracer : IZipkinTracer, IDisposable
    {
        private readonly IPropagator<ITextMap> _propagator;
        private readonly ISpanReporter _reporter;

        public ZipkinTracer(ZipkinTracerOptions options)
        {
            _propagator = new B3Propagator();
            _reporter = options.Reporter;
            LocalEndpoint = options.LocalEndpoint;
            TimeProvider = options.TimeProvider ?? new DateTimeOffsetTimeProvider();
            ScopeManager = options.ScopeManager ?? NoOpScopeManager.Instance;
            IdProvider = options.IdProvider ?? ThreadLocalRngSpanIdProvider.TraceId128BitProvider;
            Options = options;
        }

        public ZipkinTracerOptions Options { get; }
        
        public Endpoint LocalEndpoint { get; }
        public ITimeProvider TimeProvider { get; }
        public ISpanIdProvider IdProvider { get; }

        public void Report(Span span)
        {
            _reporter.Report(span);
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
            if ((format == BuiltinFormats.TextMap || format == BuiltinFormats.HttpHeaders) &&
                carrier is ITextMap textMap)
            {
                _propagator.Inject((SpanContext)spanContext, textMap);
                return;
            }

            throw new ZipkinFormatException(
                $"Unrecognized carrier format [{format}]. Only ITextMap is supported by this driver.");
        }

        public ISpanContext Extract<TCarrier>(IFormat<TCarrier> format, TCarrier carrier)
        {
            if ((format == BuiltinFormats.TextMap || format == BuiltinFormats.HttpHeaders) &&
                carrier is ITextMap textMap)
                return _propagator.Extract(textMap);

            throw new ZipkinFormatException(
                $"Unrecognized carrier format [{format}]. Only ITextMap is supported by this driver.");
        }

        public IScopeManager ScopeManager { get; }
        public ISpan ActiveSpan => ScopeManager.Active.Span;

        public void Dispose()
        {
            _reporter?.Dispose();
        }
    }
}
