#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Temp.Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using Temp.Newtonsoft.Json.Utilities;

namespace Temp.Newtonsoft.Json.Schema
{
    /// <summary>
    /// Returns detailed information about the schema exception.
    /// </summary>
#if !(NETFX_CORE || PORTABLE40 || PORTABLE)
    [Serializable]
#endif
    public class JSchemaException : JsonException
    {
        /// <summary>
        /// Gets the line number indicating where the error occurred.
        /// </summary>
        /// <value>The line number indicating where the error occurred.</value>
        public int LineNumber
        {
            get { return (ValidationError != null) ? ValidationError.LineNumber : 0; }
        }

        /// <summary>
        /// Gets the line position indicating where the error occurred.
        /// </summary>
        /// <value>The line position indicating where the error occurred.</value>
        public int LinePosition
        {
            get { return (ValidationError != null) ? ValidationError.LinePosition : 0; }
        }

        /// <summary>
        /// Gets the path to the JSON where the error occurred.
        /// </summary>
        /// <value>The path to the JSON where the error occurred.</value>
        public string Path
        {
            get { return (ValidationError != null) ? ValidationError.Path : null; }
        }

        /// <summary>
        /// Gets the <see cref="JSchemaException"/>'s validation error.
        /// </summary>
        public ValidationError ValidationError { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JSchemaException"/> class.
        /// </summary>
        public JSchemaException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JSchemaException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public JSchemaException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JSchemaException"/> class
        /// with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public JSchemaException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JSchemaException"/> class
        /// with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        /// <param name="validationError">The <see cref="ValidationError"/> for ths exception.</param>
        public JSchemaException(string message, Exception innerException, ValidationError validationError)
            : base(message, innerException)
        {
            ValidationError = validationError;
        }

#if !(NETFX_CORE || PORTABLE40 || PORTABLE)
        /// <summary>
        /// Initializes a new instance of the <see cref="JSchemaException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        public JSchemaException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif

        internal static JSchemaException Create(string message, ValidationError validationError)
        {
            JSchemaException ex = new JSchemaException(validationError.BuildExtendedMessage(), null, validationError);

            return ex;
        }
    }
}
