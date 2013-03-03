// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Sam Novak" file="UsersDB.cs">
//   CIS499 - 2013 - IM Server
// </copyright>
// <summary>
//   Represents the Users_DB table
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CIS499_IM_Server.DatabaseClasses
{
    using System;
    using System.Collections.Generic;

    using UserClass;

    /// <summary>
    ///     Represents the Users_DB table
    /// </summary>
    public class UsersDB : UserClass
    {
        #region Constants

        /// <summary>
        ///     The Maximum Length the PassHash field allows
        /// </summary>
        public const int PassHashMaxLength = 128;

        /// <summary>
        ///     The Maximum Length the UserName field allows
        /// </summary>
        public const int UserNameMaxLength = 128;

        #endregion

        #region Fields

        /// <summary>
        ///     The pass hash.
        /// </summary>
        private string passHash;

        /// <summary>
        ///     The user name.
        /// </summary>
        private string userName;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the value of Friends
        /// </summary>
        public new List<UserClass> Friends { get; set; }

        /// <summary>
        ///     Gets or sets the value of PassHash
        /// </summary>
        public string PassHash
        {
            get
            {
                return this.passHash;
            }

            set
            {
                this.passHash = value;
                if (this.passHash != null && this.PassHash.Length > PassHashMaxLength)
                {
                    throw new ArgumentException("Max length for PassHash is 128");
                }
            }
        }

        /// <summary>
        ///     Gets or sets the value of UserID
        /// </summary>
        public new int? UserId { get; set; }

        /// <summary>
        ///     Gets or sets the value of UserName
        /// </summary>
        public new string UserName
        {
            get
            {
                return this.userName;
            }

            set
            {
                this.userName = value;
                if (this.userName != null && this.UserName.Length > UserNameMaxLength)
                {
                    throw new ArgumentException("Max length for UserName is 128");
                }
            }
        }

        #endregion
    }
}