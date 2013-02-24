// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserClass.cs" company="Sam Novak">
//   CIS499 - 2013 - IM Server
// </copyright>
// <summary>
//   Defines the UserClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// The user class.
/// </summary>
[Serializable]
// ReSharper disable CheckNamespace
public class UserClass : IDisposable // ReSharper restore CheckNamespace
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
    internal List<UserClass> Friends { get; set; }

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

    /// <summary>
    /// The dispose.
    /// </summary>
    public void Dispose()
    {
        this.UserName = null;
        this.PasswordHash = null;
        this.Friends = null;
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
    internal static byte[] Serialize(UserClass obj)
    {
        if (obj == null)
        {
            return null;
        }

        var bf = new BinaryFormatter() { AssemblyFormat = FormatterAssemblyStyle.Simple };
        var ms = new MemoryStream();
        bf.Serialize(ms, obj);
        return ms.ToArray();
    }

    /// <summary>
    /// De serialize
    /// </summary>
    /// <param name="arrBytes">the array of bytes to convert back to the class</param>
    /// <returns>The user</returns>
    internal static UserClass Deserialize(byte[] arrBytes)
    {
        var memStream = new MemoryStream();
        var binForm = new BinaryFormatter { AssemblyFormat = FormatterAssemblyStyle.Simple };
        memStream.Write(arrBytes, 0, arrBytes.Length);
        memStream.Seek(0, SeekOrigin.Begin);
        var obj = (UserClass)binForm.Deserialize(memStream);
        return obj;
    }
}