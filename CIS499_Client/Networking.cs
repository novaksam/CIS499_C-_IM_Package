// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Networking.cs" company="Sam Novak">
//   C# Instant Messenger XAML client
// </copyright>
// <summary>
//   The networking.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CIS499_Client
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;

    using Imstatuses;

    using Packet;

    using UserClass;

    /// <summary>
    /// The networking.
    /// </summary>
    public sealed class Networking : IDisposable
    {
        /// <summary>
        /// The TCP client.
        /// </summary>
        private TcpClient tcpClient;

        /// <summary>
        /// The reader.
        /// </summary>
        private BinaryReader reader;

        /// <summary>
        /// The writer.
        /// </summary>
        private BinaryWriter writer;

        /// <summary>
        /// The security stream.
        /// </summary>
        private SslStream ssl;

        /// <summary>
        /// Boolean for if the program is running
        /// </summary>
        private bool running;

        /// <summary>
        /// The listener.
        /// </summary>
        private TcpListener listener;

        /// <summary>
        /// Boolean for the connection state
        /// </summary>
        private bool loggedIn;

        /// <summary>
        /// The network stream.
        /// </summary>
        private NetworkStream stream;

        /// <summary>
        /// The user
        /// </summary>
        internal UserClass TheUser { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Networking"/> class. 
        /// Default constructor
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        public Networking(UserClass user)
        {

            this.TheUser = user.Clone() as UserClass;
            // Creates the connection
            // var time = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoaming);
            this.tcpClient = new TcpClient(Settings.Default.Server, Settings.Default.Port);

            if (!this.tcpClient.Connected)
            {
                //throw this.failedConnect;
            }

            this.loggedIn = true;

            // this.Ping(tcpClient);
            this.stream = this.tcpClient.GetStream();
            this.ssl = new SslStream(this.stream, false, ValidateCert);
            this.ssl.AuthenticateAsClient(Settings.Default.Cert_Owner);
            this.writer = new BinaryWriter(this.ssl, Encoding.UTF8);
            this.reader = new BinaryReader(this.ssl, Encoding.UTF8);
            
            // Get the hello from the server
            var hello = this.reader.ReadInt32();
            if (hello == ImStatuses.ImHello)
            {
                // Send a hello to the server
                this.writer.Write(ImStatuses.ImHello);

                // var threadStart = new ParameterizedThreadStart(o => this.Listen(user));
                // var listenMeth = new Thread(threadStart);
                // listenMeth.Start();
            }
        }

        /// <summary>
        /// The login.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="ImStatuses"/>.
        /// </returns>
        internal bool Login(UserClass user)
        {
            this.writer.Write(ImStatuses.ImLogin);
            var serial = UserClass.Serialize(user);
            this.writer.Write(serial.Length);
            this.writer.Write(serial);
            this.writer.Flush();
            
            // writer.Write(user.PasswordHash);

            var temp = this.reader.ReadByte();
            if (temp == ImStatuses.ImOk)
            {
                // Get the length of the incoming byte array
                int length = this.reader.ReadInt32();

                // Read said byte array
                byte[] use = this.reader.ReadBytes(length);

                // Convert that array into a user.
                this.TheUser = UserClass.Deserialize(use).Clone() as UserClass;
                

                return true;
            }
            
            if (temp == ImStatuses.ImWrongPass)
            {
                //this.wrongPasswordException.Source = "User login";
                //throw this.wrongPasswordException;
            }

            if (temp == ImStatuses.ImNoExists)
            {
                //this.noUserException.Source = "User login";
                //throw this.noUserException;
            }
            
            return false;
        }

        /// <summary>
        /// Register user
        /// </summary>
        /// <param name="user">
        /// The user to remember
        /// </param>
        /// <returns>
        /// Returns the result of the registration
        /// </returns>
        internal bool Register(UserClass user)
        {
            this.writer.Write(ImStatuses.ImRegister);
            var serial = UserClass.Serialize(user);
            this.writer.Write(serial.Length);
            this.writer.Write(serial);
            this.writer.Flush();

            // TODO add code to determine cause of failure.
            // perhaps change the return type.
            // Actually I could probably use the IM statuses for this.
            var temp = this.reader.ReadByte();
            if (temp == ImStatuses.ImOk)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// The search.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="UserClass"/>.
        /// </returns>
        internal List<UserClass> Search(string name)
        {
            this.writer.Write(ImStatuses.ImSearch);
            var usr = UserClass.Serialize(this.TheUser);
            this.writer.Write(usr.Length);
            this.writer.Write(usr);
            this.writer.Write(name);
            this.writer.Flush();
            var list = new List<UserClass>();

            var temp = this.reader.ReadByte();
            if (temp != ImStatuses.ImSearch)
            {
                return list;
            }

            // Get the length of the incoming byte array
            int length = this.reader.ReadInt32();

            // Read said byte array
            byte[] use = this.reader.ReadBytes(length);

            // Convert that array into a user.
            list = UserClass.RestoreFriends(use);

            try
            {
                foreach (var b in this.TheUser.Friends)
                {
                    // TODO figure out why this won't work
                    var worker2 = list.Contains(b);
                    var worker = list.Remove(b);
                }
                
            }
            catch (Exception ex)
            {
                throw;
            }
            // Return the list

            return list;
        }

        /// <summary>
        /// The search delegate.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The User class list
        /// </returns>
        internal delegate List<UserClass> SearchDelegate(string name);

        /// <summary>
        /// The add friend.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="users">
        /// The users.
        /// </param>
        internal void AddFriend(UserClass user, List<UserClass> users)
        {
            this.writer.Write(ImStatuses.ImAddFriend);
            var usr = UserClass.Serialize(user);
            this.writer.Write(usr.Length);
            this.writer.Write(usr);
            var addees = UserClass.StoreFriends(users);
            this.writer.Write(addees.Length);
            this.writer.Write(addees);
            this.writer.Flush();

            var temp = this.reader.ReadByte();
            if (temp != ImStatuses.ImAddFriend)
            {
                return;
            }

            // Get the length of the incoming byte array
            int length = this.reader.ReadInt32();

            // Read said byte array
            byte[] use = this.reader.ReadBytes(length);

            // Convert that array into a user.
            this.TheUser.Friends = UserClass.RestoreFriends(use);
        }

        // private void Ping(TcpClient client)
        // {
        //    NetworkStream stream = client.GetStream();
        //    BinaryWriter writeTemp = new BinaryWriter(stream, Encoding.UTF8);
        //    byte[] buffer = new byte[8];
        //    writeTemp.Write(ImStatuses.Im_Hello);
        //    writeTemp.Flush();
        // }

        /// <summary>
        /// The listen.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        private void Listen(UserClass user)
        {
            while (this.tcpClient.Connected)
            {
                // Read the incoming status
                var type = this.reader.ReadByte();

                switch (type)
                {
                    case ImStatuses.ImIsAvailable:
                        {
                            var who = this.reader.ReadString();

                            this.writer.Write(ImStatuses.ImIsAvailable);
                            this.writer.Write(user.UserId);
                            this.writer.Flush();
                        }
                        break;
                    case ImStatuses.ImSend:
                        {
                            
                        }
                        break;
                }

                // TcpClient tcpClient = this.listener.AcceptTcpClient();  // Accept incoming connection.
                // listener.Start();
                // listener.BeginAcceptTcpClient(ar => listener.EndAcceptSocket(ar), listener);
                // AsyncCallback aCallback = new AsyncCallback(ar => listener.BeginAcceptTcpClient(ar, listener));
                // listener.
            }
        }

        /// <summary>
        /// The receiver.
        /// </summary>
        private void Receive()
        {
            // while (this.tcpClient.Connected)
            // {
            //    byte type = reader.ReadByte();  // Get incoming packet type.
            //    if (type == ImStatuses.IM_IsAvailable)
            //    {
            //        string who = reader.ReadString();
            //        writer.Write(ImStatuses.IM_IsAvailable);
            //        writer.Write(who);
            //        if (prog.Users.TryGetValue(who, out info))
            //        {
            //            if (info.LoggedIn)
            //                writer.Write(true);   // Available
            //            else
            //                writer.Write(false);  // Unavailable
            //        }
            //        else
            //            writer.Write(false);      // Unavailable
            //        writer.Flush();
            //    }
            //    else if (type == ImStatuses.IM_Send)
            //    {
            //        string to = reader.ReadString();
            //        string msg = reader.ReadString();
            //        UserClass recipient;
            //        if (prog.Users.TryGetValue(to, out recipient))
            //        {
            //            // Is recipient logged in?
            //            if (recipient.LoggedIn)
            //            {
            //                // Write received packet to recipient
            //                recipient.Connection.bw.Write(ImStatuses.IM_Received);
            //                recipient.Connection.bw.Write(userInfo.UserName);  // From
            //                recipient.Connection.bw.Write(msg);
            //                recipient.Connection.bw.Flush();
            //                Console.WriteLine("[{0}] ({1} -> {2}) Message sent!", DateTime.Now, userInfo.UserName, recipient.UserName);
            //            }
            //        }
            //    }
            // }
        }

        /// <summary>
        /// The send message.
        /// </summary>
        /// <param name="packet">
        /// The packet.
        /// </param>
        internal void SendMessage(Packet packet)
        {
            this.writer.Write(ImStatuses.ImSend);
            this.writer.Write(packet.Message);
            this.writer.Flush();
        }

        /// <summary>
        /// Close all network related objects
        /// </summary>
        internal void CloseNetwork()
        {
            this.Dispose();
        }

        /// <summary>
        /// The validate cert.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="certificate">
        /// The certificate.
        /// </param>
        /// <param name="chain">
        /// The chain.
        /// </param>
        /// <param name="sslPolicyErrors">
        /// The SSL policy errors.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool ValidateCert(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // Uncomment this lines to disallow untrusted certificates.
            // if (sslPolicyErrors == SslPolicyErrors.None)
            //    return true;
            // else
            //    return false;
            return true; // Allow untrusted certificates.
        }

        /// <summary>
        /// Dispose members and objects.
        /// </summary>
        public void Dispose()
        {
            // this.listener.Stop();
            this.writer.Close();
            this.writer.Dispose();
            this.reader.Close();
            this.reader.Dispose();
            this.ssl.Close();
            this.ssl.Dispose();
            this.stream.Close();
            this.stream.Dispose();
            this.TheUser.Dispose();
            this.tcpClient.Close();
            GC.SuppressFinalize(this);
        }
    }
}
