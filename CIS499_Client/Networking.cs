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
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// The networking.
    /// </summary>
    internal class Networking
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
        /// The failed connect.
        /// </summary>
        private SocketException failedConnect = new SocketException(543);

        /// <summary>
        /// Wrong password exception
        /// </summary>
        private Exception wrongPasswordException = new Exception("Password incorrect");

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
        private Boolean loggedIn;

        /// <summary>
        /// Initializes a new instance of the <see cref="Networking"/> class. 
        /// Default constructor
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        public Networking(UserClass user)
        {
            // Creates the connection
            this.tcpClient = new TcpClient(Settings.Default.Server, Settings.Default.Port);

            if (!this.tcpClient.Connected)
            {
                throw this.failedConnect;
            }

            this.loggedIn = true;
            // this.Ping(tcpClient);

            NetworkStream stream = this.tcpClient.GetStream();
            this.ssl = new SslStream(stream, false, ValidateCert);
            this.ssl.AuthenticateAsClient(Settings.Default.Cert_Owner);
            this.writer = new BinaryWriter(this.ssl, Encoding.UTF8);
            this.reader = new BinaryReader(this.ssl, Encoding.UTF8);
            var login = this.Login(user);
            switch (login)
            {
                    // Proceed as normal
                case ImStatuses.IM_OK:
                    break;

                    // Wrong password
                case ImStatuses.IM_WrongPass:
                    this.wrongPasswordException.Source = "User login";
                    throw this.wrongPasswordException;

            }

            var threadStart = new ParameterizedThreadStart(o => this.listen(user));
            var listenMeth = new Thread(threadStart);
            listenMeth.Start();


        }

        /// <summary>
        /// The login.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/>.
        /// </returns>
        private byte Login(UserClass user)
        {
            this.writer.Write(ImStatuses.IM_Login);
            var formatter = new BinaryFormatter(); 
            formatter.Serialize(this.ssl, user);

            // writer.Write(user.PasswordHash);
            return this.reader.ReadByte();

        }

        // private void Ping(TcpClient client)
        // {
        //    NetworkStream stream = client.GetStream();
        //    BinaryWriter writeTemp = new BinaryWriter(stream, Encoding.UTF8);
        //    byte[] buffer = new byte[8];
        //    writeTemp.Write(ImStatuses.Im_Hello);
        //    writeTemp.Flush();
        // }


        private void listen(UserClass user)
        {
            while (tcpClient.Connected)
            {
                // Read the incoming status
                byte type = this.reader.ReadByte();

                if (type == ImStatuses.IM_IsAvailable)
                {
                    string who = reader.ReadString();

                    writer.Write(ImStatuses.IM_IsAvailable);
                    writer.Write(user.UserId);
                }
                    

                //TcpClient tcpClient = this.listener.AcceptTcpClient();  // Accept incoming connection.
                //listener.Start();
                //listener.BeginAcceptTcpClient(ar => listener.EndAcceptSocket(ar), listener);
                //AsyncCallback aCallback = new AsyncCallback(ar => listener.BeginAcceptTcpClient(ar, listener));
                //listener.
            }  
        }

        private void recieve()
        {
            //while (this.tcpClient.Connected)
            //{
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
            //}
        }

        /// <summary>
        /// The send message.
        /// </summary>
        /// <param name="packet">
        /// The packet.
        /// </param>
        internal void SendMessage(Packet packet)
        {
            this.writer.Write(ImStatuses.IM_Send);
            this.writer.Write(packet.SenderId);
            this.writer.Write(packet.RecipientId);
            this.writer.Write(packet.Message);
            this.writer.Flush();
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
}
}
