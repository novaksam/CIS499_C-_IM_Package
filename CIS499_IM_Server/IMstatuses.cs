// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMstatuses.cs" company="Sam Novak">
//   CIS499 - 2013 - IM Server
// </copyright>
// <summary>
//   The im statuses.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CIS499_IM_Server
{
    using System.Diagnostics.CodeAnalysis;

    // ReSharper disable InconsistentNaming
    /// <summary>
    /// The instant message statuses.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Reviewed. Suppression is OK here.")]
    internal static class ImStatuses
    {
        /// <summary>
        /// The i m_ hello.
        /// </summary>
        public const int Im_Hello = 1337;      // Hello
        
        /// <summary>
        /// The i m_ ok.
        /// </summary>
        public const byte IM_OK = 0;           // OK

        /// <summary>
        /// The i m_ login.
        /// </summary>
        public const byte IM_Login = 1;        // Login

        /// <summary>
        /// The i m_ register.
        /// </summary>
        public const byte IM_Register = 2;     // Register

        /// <summary>
        /// The i m_ too username.
        /// </summary>
        public const byte IM_TooUsername = 3;  // Too long username

        /// <summary>
        /// The i m_ too password.
        /// </summary>
        public const byte IM_TooPassword = 4;  // Too long password

        /// <summary>
        /// The i m_ exists.
        /// </summary>
        public const byte IM_Exists = 5;       // Already exists

        /// <summary>
        /// The i m_ no exists.
        /// </summary>
        public const byte IM_NoExists = 6;     // Doesn't exist

        /// <summary>
        /// The i m_ wrong pass.
        /// </summary>
        public const byte IM_WrongPass = 7;    // Wrong password

        /// <summary>
        /// The i m_ is available.
        /// </summary>
        public const byte IM_IsAvailable = 8;  // Is user available?

        /// <summary>
        /// The i m_ send.
        /// </summary>
        public const byte IM_Send = 9;         // Send message

        /// <summary>
        /// The i m_ received.
        /// </summary>
        public const byte IM_Received = 10;    // Message received
    }
    // ReSharper restore InconsistentNaming
}