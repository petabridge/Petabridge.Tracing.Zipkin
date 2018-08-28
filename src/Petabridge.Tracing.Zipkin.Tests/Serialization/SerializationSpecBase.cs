// -----------------------------------------------------------------------
// <copyright file="SerializationSpecBase.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Petabridge.Tracing.Zipkin.Reporting;

namespace Petabridge.Tracing.Zipkin.Tests.Serialization
{
    /// <summary>
    ///     Used to verify the contents of a serialization spec.
    /// </summary>
    /// <param name="actual">What the serializer actually produced.</param>
    /// <param name="expected">What we expected to be produced.</param>
    public delegate void SerializationVerifier(byte[] actual, byte[] expected);

    /// <summary>
    ///     Base class for serialization specs and test cases.
    /// </summary>
    public class SerializationSpecBase
    {
        public static void DefaultVerifier(byte[] actual, byte[] expected)
        {
            expected.Should().BeEquivalentTo(actual);
        }

        public void VerifySerialization(ISpanSerializer serializer, byte[] expectedOutput, Span inputSpan)
        {
            VerifySerialization(serializer, expectedOutput, inputSpan, DefaultVerifier);
        }

        public void VerifySerialization(ISpanSerializer serializer, byte[] expectedOutput, Span inputSpan,
            SerializationVerifier verifier)
        {
            VerifySerialization(serializer, expectedOutput, new[] {inputSpan}, verifier);
        }

        public void VerifySerialization(ISpanSerializer serializer, byte[] expectedOutput, IEnumerable<Span> inputSpans)
        {
            VerifySerialization(serializer, expectedOutput, inputSpans, DefaultVerifier);
        }

        public void VerifySerialization(ISpanSerializer serializer, byte[] expectedOutput, IEnumerable<Span> inputSpans,
            SerializationVerifier verifier)
        {
            var stream = new MemoryStream();
            serializer.Serialize(stream, inputSpans);
            var actualOutput = stream.ToArray();
            stream.Flush();
            stream.Dispose();
            verifier(actualOutput, expectedOutput);
        }
    }
}