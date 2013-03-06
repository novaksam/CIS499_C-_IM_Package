// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserInfo.cs" company="Sam Novak">
//   CIS499 - 2013 - IM Server
// </copyright>
// <summary>
//   Defines the UserInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CIS499_IM_Server
{
    using System;

    /// <summary>
    /// The user info.
    /// </summary>
    [Serializable]
    public class UserInfo : DatabaseClasses.UsersDB
    {
        /// <summary>
        /// The user name.
        /// </summary>
        public new string UserName;

        /// <summary>
        /// The password.
        /// </summary>
        public new string PassHash;

        /// <summary>
        /// The logged in.
        /// </summary>
        [NonSerialized] public new bool LoggedIn;      // Is logged in and connected?

        /// <summary>
        /// The connection.
        /// </summary>
        [NonSerialized] public Client Connection;  // Connection info

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInfo"/> class.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="pass">
        /// The pass.
        /// </param>
        public UserInfo(string user, string pass)
        {
            this.UserName = user;
            this.PassHash = pass;
            this.LoggedIn = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInfo"/> class.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="pass">
        /// The pass.
        /// </param>
        /// <param name="conn">
        /// The conn.
        /// </param>
        public UserInfo(string user, string pass, Client conn)
        {
            this.UserName = user;
            this.PassHash = pass;
            this.LoggedIn = true;
            this.Connection = conn;
        }
    }
}
