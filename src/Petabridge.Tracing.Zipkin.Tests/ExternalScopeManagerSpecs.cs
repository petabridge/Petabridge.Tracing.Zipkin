using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using OpenTracing;
using Petabridge.Tracing.Zipkin.Tracers;
using Xunit;

namespace Petabridge.Tracing.Zipkin.Tests
{

    /// <summary>
    /// Want to validate that the <see cref="ZipkinTracer"/> will play nicely
    /// with anything other than the default <see cref="IScopeManager"/>
    /// </summary>
    public class ExternalScopeManagerSpecs
    {
        public ExternalScopeManagerSpecs()
        {
            Tracer = new MockZipkinTracer(scopeManager: new MockScopeManager());
        }

        protected readonly MockZipkinTracer Tracer;

        /*
         * This spec is mostly a sanity check against things going catastrophically wrong
         * with the Zipkin SpanBuilder.
         */
        [Fact(DisplayName = "Should create spans normally if no ScopeManager.Active.Span available")]
        public void ShouldRecordTracesWhenNoActiveSpan()
        {
            Tracer.BuildSpan("op1").Start().Finish();
            Tracer.CollectedSpans.Count.Should().Be(1);
            Tracer.CollectedSpans.TryDequeue(out var innerSpan);
            innerSpan.OperationName.Should().Be("op1");
        }

        [Fact(DisplayName = "Should be able to create ActiveScopes with ScopeManager")]
        public void ShouldRecordActiveSpan()
        {
            using (var scope = Tracer.BuildSpan("op1").StartActive(true))
            {
                Tracer.ActiveSpan.Should().Be(scope.Span);
            }
        }
    }

    public class MockScopeManager : IScopeManager
    {

        public IScope Activate(ISpan span, bool finishSpanOnDispose)
        {
            return new MockScope(this, span, finishSpanOnDispose);
        }

        public IScope Active { get; set; }

        public class MockScope : IScope
        {
            private readonly MockScopeManager _mockScopeManager;
            private readonly ISpan _wrappedSpan;
            private readonly bool _finishOnDispose;
            private readonly IScope _scopeToRestore;

            public MockScope(MockScopeManager mockScopeManager, ISpan wrappedSpan, bool finishOnDispose)
            {
                _mockScopeManager = mockScopeManager;
                _wrappedSpan = wrappedSpan;
                _finishOnDispose = finishOnDispose;
                _scopeToRestore = mockScopeManager.Active;
                _mockScopeManager.Active = this;
            }

            public void Dispose()
            {
                if (_finishOnDispose)
                {
                    _wrappedSpan.Finish();
                }

                _mockScopeManager.Active = _scopeToRestore;
            }

            public ISpan Span => _wrappedSpan;
        }
    }
}
