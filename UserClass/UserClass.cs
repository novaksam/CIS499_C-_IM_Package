// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserClass.cs" company="Sam Novak">
//   CIS499 - 2013 - UserClass
// </copyright>
// <summary>
//   Defines the UserClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace UserClass
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.Serialization.Formatters.Binary;

    /// <summary>
    ///     The user class.
    /// </summary>
    [Serializable]
    public class UserClass : IDisposable, ICloneable, IEquatable<UserClass>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserClass" /> class.
        /// </summary>
        public UserClass()
        {
            this.Friends = new List<UserClass>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserClass"/> class.
        /// </summary>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <param name="passwordHash">
        /// The password hash.
        /// </param>
        /// <param name="loggedIn">
        /// The logged in.
        /// </param>
        public UserClass(string userName, string passwordHash, bool loggedIn)
        {
            this.UserName = userName;
            this.PasswordHash = passwordHash;
            this.LoggedIn = loggedIn;
            this.Friends = new List<UserClass>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserClass"/> class.
        /// </summary>
        /// <param name="userName">
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
        public UserClass(string userName, int id, string pass, bool logged)
        {
            this.UserName = userName;
            this.UserId = id;
            this.PasswordHash = pass;
            this.LoggedIn = logged;
            this.Friends = new List<UserClass>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the friends.
        /// </summary>
        public List<UserClass> Friends { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether logged in.
        /// </summary>
        public bool LoggedIn { get; set; }

        /// <summary>
        ///     Gets or sets the password hash.
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        ///     Gets or sets the user id.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        ///     Gets or sets the user name.
        /// </summary>
        public string UserName { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// De serialize
        /// </summary>
        /// <param name="bytes">
        /// the array of bytes to convert back to the class
        /// </param>
        /// <returns>
        /// The user
        /// </returns>
        public static UserClass Deserialize(byte[] bytes)
        {
            // IFormatter formatter = new BinaryFormatter();
            var memStream = new MemoryStream();
            var binForm = new BinaryFormatter(); // { AssemblyFormat = FormatterAssemblyStyle.Full };
            memStream.Write(bytes, 0, bytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            var obj = binForm.Deserialize(memStream) as UserClass;
            memStream.Dispose();
            return obj;
        }

        /// <summary>
        /// Convert stored list of friends into a list of users
        /// </summary>
        /// <param name="bytes">
        /// The stored byte array
        /// </param>
        /// <returns>
        /// Friends list
        /// </returns>
        public static List<UserClass> RestoreFriends(byte[] bytes)
        {
            var memStream = new MemoryStream();
            var binForm = new BinaryFormatter(); // { AssemblyFormat = FormatterAssemblyStyle.Simple };
            memStream.Write(bytes, 0, bytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            var obj = binForm.Deserialize(memStream) as List<UserClass>;
            memStream.Dispose();
            return obj;
        }

        /// <summary>
        /// The serialize.
        /// </summary>
        /// <param name="obj">
        /// The user
        /// </param>
        /// <returns>
        /// The byte array of the user
        /// </returns>
        public static byte[] Serialize(UserClass obj)
        {
            if (obj == null)
            {
                return null;
            }

            var bf = new BinaryFormatter(); // { AssemblyFormat = FormatterAssemblyStyle.Full };
            var ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        /// <summary>
        /// Convert list of friends into storable object
        /// </summary>
        /// <param name="obj">
        /// The list of store
        /// </param>
        /// <returns>
        /// The byte array
        /// </returns>
        public static byte[] StoreFriends(List<UserClass> obj)
        {
            if (obj == null)
            {
                return null;
            }

            var bf = new BinaryFormatter { AssemblyFormat = FormatterAssemblyStyle.Simple };
            var ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        /// <summary>
        ///     The clone.
        /// </summary>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        ///     The dispose.
        /// </summary>
        public void Dispose()
        {
            this.UserName = null;
            this.PasswordHash = null;
            this.Friends = null;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Equals(UserClass other)
        {
            return this.UserId == other.UserId && 
                this.UserName == other.UserName;
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="obj">
        /// The object.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as UserClass);
        }

        /// <summary>
        ///     The to string.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public override string ToString()
        {
            return this.UserName;
        }

        /// <summary>
        /// The get hash code.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.UserName.GetHashCode();
        }

        #endregion
    }
}