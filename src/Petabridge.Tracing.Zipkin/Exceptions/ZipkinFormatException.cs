// -----------------------------------------------------------------------
// <copyright file="ZipkinFormatException.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2018 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Petabridge.Tracing.Zipkin.Exceptions
{
    /// <summary>
    ///     Thrown when a header or any other argument is defined in an unknown or unparseable format.
    /// </summary>
    public class ZipkinFormatException : ZipkinException
    {
        public ZipkinFormatException()
        {
        }

        public ZipkinFormatException(string message) : base(message)
        {
        }

        public ZipkinFormatException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}