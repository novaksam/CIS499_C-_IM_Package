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
    using System.Diagnostics.CodeAnalysis;

    using UserClass;

    /// <summary>
    ///     Represents the Users_DB table
    /// </summary>
    [Serializable]
    public class UsersDB : UserClass
    {
        #region Constants

        public UsersDB()
        {
        }

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
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
// ReSharper disable InconsistentNaming
        //private new string PasswordHash;
// ReSharper restore InconsistentNaming

        /// <summary>
        ///     The user name.
        /// </summary>
// ReSharper disable InconsistentNaming
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
        //private new string UserName;
// ReSharper restore InconsistentNaming
        #endregion

        #region Public Properties

        //public UsersDB(string userName, string passwordHash, bool loggedIn)
        //    : base(userName, passwordHash, loggedIn)
        //{

        //}

        //public UsersDB(string userName, uint id, string pass, bool logged)
        //    : base(userName, id, pass, logged)
        //{
        //}

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
                return this.PasswordHash;
            }

            set
            {
                this.PasswordHash = value;
                if (this.PasswordHash != null && this.PassHash.Length > PassHashMaxLength)
                {
                    throw new ArgumentException("Max length for PassHash is 128");
                }
            }
        }

        /// <summary>
        ///     Gets or sets the value of UserID
        /// </summary>
        //public new int? UserId { get; set; }

        /// <summary>
        ///     Gets or sets the value of UserName
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
// ReSharper disable InconsistentNaming
        public string userName
// ReSharper restore InconsistentNaming
        {
            get
            {
                return this.UserName;
            }

            set
            {
                this.UserName = value;
                if (this.UserName != null && this.UserName.Length > UserNameMaxLength)
                {
                    throw new ArgumentException("Max length for UserName is 128");
                }
            }
        }

        #endregion
    }
}