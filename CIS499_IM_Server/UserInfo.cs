// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserInfo.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the UserInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace CIS499_IM_Server
{
    [Serializable]
    public class UserInfo
    {
        public string UserName;
        public string Password;
        [NonSerialized] public bool LoggedIn;      // Is logged in and connected?
        [NonSerialized] public Client Connection;  // Connection info
        
        public UserInfo(string user, string pass)
        {
            this.UserName = user;
            this.Password = pass;
            this.LoggedIn = false;
        }
        public UserInfo(string user, string pass, Client conn)
        {
            this.UserName = user;
            this.Password = pass;
            this.LoggedIn = true;
            this.Connection = conn;
        }
    }
}
