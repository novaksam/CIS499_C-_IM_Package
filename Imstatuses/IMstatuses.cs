// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMstatuses.cs" company="Sam Novak">
//   C# Instant messenger status library
// </copyright>
// <summary>
//   The im statuses.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Imstatuses
{
    /// <summary>
    /// The instant message statuses.
    /// </summary>
    public static class ImStatuses
    {
        /// <summary>
        /// The i m_ hello.
        /// </summary>
        public const int ImHello = 1337;      // Hello
        
        /// <summary>
        /// The i m_ ok.
        /// </summary>
        public const byte ImOk = 0;           // OK

        /// <summary>
        /// The i m_ login.
        /// </summary>
        public const byte ImLogin = 1;        // Login

        /// <summary>
        /// The i m_ register.
        /// </summary>
        public const byte ImRegister = 2;     // Register

        /// <summary>
        /// The i m_ too username.
        /// </summary>
        public const byte ImTooLongUsername = 3;  // Too long username

        /// <summary>
        /// The i m_ too password.
        /// </summary>
        public const byte ImTooLongPassword = 4;  // Too long password

        /// <summary>
        /// The user exists.
        /// </summary>
        public const byte ImExists = 5;       // Already exists

        /// <summary>
        /// The user doesn't exist
        /// </summary>
        public const byte ImNoExists = 6;     // Doesn't exist

        /// <summary>
        /// The password is wrong
        /// </summary>
        public const byte ImWrongPass = 7;    // Wrong password

        /// <summary>
        /// The user is available.
        /// </summary>
        public const byte ImIsAvailable = 8;  // Is user available?

        /// <summary>
        /// Send message
        /// </summary>
        public const byte ImSend = 9;         // Send message

        /// <summary>
        /// Message received
        /// </summary>
        public const byte ImReceived = 10;    // Message received

        /// <summary>
        /// Logout byte.
        /// </summary>
        public const byte ImLogout = 11;

        /// <summary>
        /// Search byte
        /// </summary>
        public const byte ImSearch = 12;

        /// <summary>
        /// Add friend byte.
        /// </summary>
        public const byte ImAddFriend = 13;

        /// <summary>
        /// Error byte
        /// </summary>
        public const byte ImError = 14;
    }
}