// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserClass.cs" company="Sam Novak">
//   C# Instant Messenger XAML client
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
    public class UserClass
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
        internal List<UserClass> Friends { get;  set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserClass"/> class.
        /// </summary>
        internal UserClass()
        {
            this.Friends = new List<UserClass>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserClass"/> class.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="pass">
        /// The pass.
        /// </param>
        /// <param name="logged">
        /// The logged.
        /// </param>
        internal UserClass(string username, uint id, string pass, bool logged)
        {
            this.UserName = username;
            this.UserId = id;
            this.PasswordHash = pass;
            this.LoggedIn = logged;
            this.Friends = new List<UserClass>();
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return this.UserName;
        }
    }
}
