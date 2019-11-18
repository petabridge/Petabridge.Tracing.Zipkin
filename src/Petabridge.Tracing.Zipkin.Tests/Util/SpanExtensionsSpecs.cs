// -----------------------------------------------------------------------
// <copyright file="SpanExtensionsSpecs.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Petabridge.Tracing.Zipkin.Tracers;
using Petabridge.Tracing.Zipkin.Util;
using Xunit;

namespace Petabridge.Tracing.Zipkin.Tests.Util
{
    public class SpanExtensionsSpecs
    {
        [Fact(DisplayName = "NoOp.Span.IsEmpty() should always be true")]
        public void NoOpSpanContextShouldBeEmpty()
        {
            NoOp.Span.Context.IsEmpty().Should().BeTrue();
        }

        [Fact(DisplayName = "NoOpZipkinSpanContext.IsEmpty() should always be true")]
        public void NoOpZipkinSpanContextShouldBeEmpty()
        {
            NoOpZipkinSpanContext.Instance.IsEmpty().Should().BeTrue();
        }

        [Fact(DisplayName = "null.IsEmpty() shoudl always be true")]
        public void NullSpanContextShouldBeEmpty()
        {
            IZipkinSpanContext none = null;
            none.IsEmpty().Should().BeTrue();
        }
    }
}