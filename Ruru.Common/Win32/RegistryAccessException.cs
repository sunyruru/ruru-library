namespace Ruru.Common.Win32
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents an error that occurs while accessing the registry.
    /// </summary>
    [Serializable]
    public class RegistryAccessException : Exception, ISerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryAccessException"/> class.
        /// </summary>
        public RegistryAccessException()
            : base()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryAccessException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public RegistryAccessException(String message)
            : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryAccessException"/> class with a specified error message 
        /// and a reference to the inner exception that is the cause of this exception. 
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The inner exception reference.</param>
        public RegistryAccessException(String message, Exception inner)
            : base(message, inner)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryAccessException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected RegistryAccessException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
