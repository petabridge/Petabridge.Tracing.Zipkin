// -----------------------------------------------------------------------
// <copyright file="ExternalScopeManagerSpecs.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using OpenTracing;
using Petabridge.Tracing.Zipkin.Tracers;
using Xunit;

namespace Petabridge.Tracing.Zipkin.Tests
{
    /// <summary>
    ///     Want to validate that the <see cref="ZipkinTracer" /> will play nicely
    ///     with anything other than the default <see cref="IScopeManager" />
    /// </summary>
    public class ExternalScopeManagerSpecs
    {
        public ExternalScopeManagerSpecs()
        {
            Tracer = new MockZipkinTracer(scopeManager: new MockScopeManager());
        }

        protected readonly MockZipkinTracer Tracer;

        [Fact(DisplayName =
            "Should use the ScopeManager.Active.Span as parent span, even overriding other explicit spans")]
        public void ShouldPreferActiveSpanAsParent()
        {
            Tracer.BuildSpan("op1").Start().Finish();
            Tracer.CollectedSpans.Count.Should().Be(1);
            Tracer.CollectedSpans.TryDequeue(out var parent);

            using (var scope = Tracer.BuildSpan("op1").StartActive(true))
            using (var scope2 = Tracer.BuildSpan("op2").AsChildOf(parent).StartActive(true))
            {
                Tracer.ActiveSpan.Should().Be(scope2.Span);
            }

            // verify that the scope disposal stuff actually works
            Tracer.CollectedSpans.Count.Should().Be(2);
            Tracer.CollectedSpans.TryDequeue(out var span1);
            Tracer.CollectedSpans.TryDequeue(out var span2);
            span1.OperationName.Should().Be("op2"); // inner span gets disposed first
            span2.OperationName.Should().Be("op1");
            span1.TypedContext.ParentId.Should()
                .Be(span2.TypedContext.SpanId); // shouldn't be the specified parent from earlier
        }

        [Fact(DisplayName = "Should be able to create ActiveScopes with ScopeManager")]
        public void ShouldRecordActiveSpan()
        {
            using (var scope = Tracer.BuildSpan("op1").StartActive(true))
            {
                Tracer.ActiveSpan.Should().Be(scope.Span);
            }

            // verify that the scope disposal stuff actually works
            Tracer.CollectedSpans.Count.Should().Be(1);
            Tracer.CollectedSpans.TryDequeue(out var innerSpan);
            innerSpan.OperationName.Should().Be("op1");

            Tracer.ActiveSpan.Should().BeNull(); // ActiveSpan should be unset in this scenario
        }

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

        [Fact(DisplayName = "Should use the ScopeManager.Active.Span as parent span by default")]
        public void ShouldUseActiveSpanAsParent()
        {
            using (var scope = Tracer.BuildSpan("op1").StartActive(true))
            using (var scope2 = Tracer.BuildSpan("op2").StartActive(true))
            {
                Tracer.ActiveSpan.Should().Be(scope2.Span);
            }

            // verify that the scope disposal stuff actually works
            Tracer.CollectedSpans.Count.Should().Be(2);
            Tracer.CollectedSpans.TryDequeue(out var span1);
            Tracer.CollectedSpans.TryDequeue(out var span2);
            span1.OperationName.Should().Be("op2"); // inner span gets disposed first
            span2.OperationName.Should().Be("op1");
            span1.TypedContext.ParentId.Should().Be(span2.TypedContext.SpanId);
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
            private readonly bool _finishOnDispose;
            private readonly MockScopeManager _mockScopeManager;
            private readonly IScope _scopeToRestore;

            public MockScope(MockScopeManager mockScopeManager, ISpan wrappedSpan, bool finishOnDispose)
            {
                _mockScopeManager = mockScopeManager;
                Span = wrappedSpan;
                _finishOnDispose = finishOnDispose;
                _scopeToRestore = mockScopeManager.Active;
                _mockScopeManager.Active = this;
            }

            public void Dispose()
            {
                if (_finishOnDispose)
                    Span.Finish();

                _mockScopeManager.Active = _scopeToRestore;
            }

            public ISpan Span { get; }
        }
    }
}