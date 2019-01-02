// -----------------------------------------------------------------------
// <copyright file="ZipkinException.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2019 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Petabridge.Tracing.Zipkin.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    ///     An exception thrown by the Zipkin driver.
    /// </summary>
    public class ZipkinException : Exception
    {
        public ZipkinException()
        {
        }

        public ZipkinException(string message) : base(message)
        {
        }

        public ZipkinException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}