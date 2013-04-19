// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Exceptions.cs" company="Sam Novak">
//   C# Instant Messenger XAML client
// </copyright>
// <summary>
//   The exceptions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CIS499_Client
{
    using System;
    using System.Net.Sockets;

    /// <summary>
    /// The exceptions.
    /// </summary>
    internal class Exceptions
    {
        /// <summary>
        /// The wrong password exception.
        /// </summary>
        [Serializable]
        public class WrongPasswordException : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="WrongPasswordException"/> class.
            /// </summary>
            /// <param name="message">
            /// The message.
            /// </param>
            public WrongPasswordException(string message)
                : base(message)
            {
            }
        }

        /// <summary>
        /// The no user exception.
        /// </summary>
        [Serializable]
        public class NoUserException : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="NoUserException"/> class.
            /// </summary>
            /// <param name="message">
            /// The message.
            /// </param>
            public NoUserException(string message)
                : base(message)
            {
            }
        }

        /// <summary>
        /// The failed connection exception.
        /// </summary>
        [Serializable]
        public class FailedConnectionException : SocketException
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="FailedConnectionException"/> class.
            /// </summary>
            /// <param name="message">
            /// The message.
            /// </param>
            public FailedConnectionException(string message)
            {
                Message = message;
            }

            /// <summary>
            /// Gets or sets the message.
            /// </summary>
            public new string Message { get; set; }
        }
    }
}
