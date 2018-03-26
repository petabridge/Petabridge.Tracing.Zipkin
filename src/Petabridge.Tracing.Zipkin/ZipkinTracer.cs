// -----------------------------------------------------------------------
// <copyright file="ZipkinTracer.cs" company="Petabridge, LLC">
//      Copyright (C) 2018 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using OpenTracing;
using OpenTracing.Propagation;
using Petabridge.Tracing.Zipkin.Exceptions;
using Petabridge.Tracing.Zipkin.Propagation;
using Petabridge.Tracing.Zipkin.Tracers.NoOp;
using Petabridge.Tracing.Zipkin.Util;

namespace Petabridge.Tracing.Zipkin
{
    /// <summary>
    ///     Standard <see cref="ITracer" /> implementation for working with Zipkin.
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

        public void Dispose()
        {
            _reporter?.Dispose();
        }

        public Endpoint LocalEndpoint { get; }
        public ITimeProvider TimeProvider { get; }
        public ISpanIdProvider IdProvider { get; }

        public void Report(Span span)
        {
            _reporter.Report(span);
        }

        public IZipkinSpanBuilder BuildSpan(string operationName)
        {
            IZipkinSpanBuilder sb = new SpanBuilder(this, operationName);

            /*
             * Turn on auto-properties when explicitly configured by user.
             * 
             * Can be changed or overwritten via the IZipkinSpanBuilder elsewhere.
             */

            if (Options.DefaultSpanKind.HasValue)
            {
                sb = sb.WithSpanKind(Options.DefaultSpanKind.Value);
            }

            if (Options.DebugMode)
            {
                sb = sb.SetDebugMode(true);
            }

            return sb;
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
                _propagator.Inject((SpanContext) spanContext, textMap);
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
    }
}