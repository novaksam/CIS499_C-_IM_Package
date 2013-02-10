// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserClass.cs" company="Sam Novak">
//   CIS499 - 2013 - IM Server
// </copyright>
// <summary>
//   Defines the UserClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CIS499_Client
{
    using System.Collections.Generic;

    /// <summary>
    /// The user class.
    /// </summary>
    internal class UserClass
    {
        /// <summary>
        /// Gets the user name.
        /// </summary>
        internal string UserName { get; private set; }

        /// <summary>
        /// Gets the user id.
        /// </summary>
        internal uint UserId { get; private set; }

        /// <summary>
        /// Gets the password hash.
        /// </summary>
        internal string PasswordHash { get; private set; }

        /// <summary>
        /// Gets a value indicating whether logged in.
        /// </summary>
        internal bool LoggedIn { get; private set; }

        /// <summary>
        /// Gets the friends.
        /// </summary>
        internal List<UserClass> Friends { get; private set; }

    }
}
