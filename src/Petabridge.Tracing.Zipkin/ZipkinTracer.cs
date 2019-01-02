// -----------------------------------------------------------------------
// <copyright file="ZipkinTracer.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2019 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;
using OpenTracing;
using OpenTracing.Propagation;
using Petabridge.Tracing.Zipkin.Exceptions;
using Petabridge.Tracing.Zipkin.Propagation;
using Petabridge.Tracing.Zipkin.Sampling;
using Petabridge.Tracing.Zipkin.Tracers;
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
            _propagator = options.Propagator ?? new B3Propagator();
            _reporter = options.Reporter;
            LocalEndpoint = options.LocalEndpoint;
            TimeProvider = options.TimeProvider ?? new DateTimeOffsetTimeProvider();
            ScopeManager = options.ScopeManager ?? NoOp.ScopeManager;
            IdProvider = options.IdProvider ?? ThreadLocalRngSpanIdProvider.TraceId128BitProvider;
            Sampler = options.Sampler ?? NoSampler.Instance;
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
        public ITraceSampler Sampler { get; }

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
                sb = sb.WithSpanKind(Options.DefaultSpanKind.Value);

            if (Options.DebugMode)
                sb = sb.SetDebugMode(true);

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
                if (spanContext is SpanContext zipkinContext)
                    _propagator.Inject(zipkinContext, textMap);
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

        /*
         * Need a null check here since some ScopeManager implementations
         * may have the ActiveSpan initially set to null.
         */
        public ISpan ActiveSpan => ScopeManager.Active?.Span;
    }
}