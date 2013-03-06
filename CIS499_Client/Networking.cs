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
    using System.IO;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading;
    using UserClass;
    using Packet;

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
        /// No user exception
        /// </summary>
        private Exception noUserException = new Exception("No user with that Username exists.");

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
        /// The network stream.
        /// </summary>
        private NetworkStream stream;

        /// <summary>
        /// The user
        /// </summary>
        private UserClass theUser;

        /// <summary>
        /// Initializes a new instance of the <see cref="Networking"/> class. 
        /// Default constructor
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        public Networking(UserClass user)
        {
            this.theUser = user.Clone() as UserClass;
            // Creates the connection
            this.tcpClient = new TcpClient(Settings.Default.Server, Settings.Default.Port);

            if (!this.tcpClient.Connected)
            {
                throw this.failedConnect;
            }

            this.loggedIn = true;

            // this.Ping(tcpClient);
            this.stream = this.tcpClient.GetStream();
            this.ssl = new SslStream(this.stream, false, ValidateCert);
            this.ssl.AuthenticateAsClient(Settings.Default.Cert_Owner);
            this.writer = new BinaryWriter(this.ssl, Encoding.UTF8);
            this.reader = new BinaryReader(this.ssl, Encoding.UTF8);

            // Get the hello from the server
            var hello = reader.ReadInt32();
            if (hello == ImStatuses.IM_Hello)
            {
                // Send a hello to the server
                this.writer.Write(ImStatuses.IM_Hello);

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
            this.writer.Write(ImStatuses.IM_Login);
            var serial = UserClass.Serialize(user);
            this.writer.Write(serial.Length);
            this.writer.Write(serial);
            this.writer.Flush();
            
            // writer.Write(user.PasswordHash);

            var temp = this.reader.ReadByte();
            if (temp == ImStatuses.IM_OK)
            {
                // Get the length of the incoming byte array
                int length = this.reader.ReadInt32();

                // Read said byte array
                byte[] use = this.reader.ReadBytes(length);

                // Convert that array into a user.
                this.theUser = UserClass.Deserialize(use).Clone() as UserClass;

                return true;
            }
            
            if (temp == ImStatuses.IM_WrongPass)
            {
                this.wrongPasswordException.Source = "User login";
                throw this.wrongPasswordException;
            }

            if (temp == ImStatuses.IM_NoExists)
            {
                this.noUserException.Source = "User login";
                throw this.noUserException;
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
            this.writer.Write(ImStatuses.IM_Register);
            var serial = UserClass.Serialize(user);
            this.writer.Write(serial.Length);
            this.writer.Write(serial);
            this.writer.Flush();

            var temp = this.reader.ReadByte();
            if (temp == ImStatuses.IM_OK)
            {
                return true;
            }
            return false;
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

                if (type == ImStatuses.IM_IsAvailable)
                {
                    var who = this.reader.ReadString();

                    this.writer.Write(ImStatuses.IM_IsAvailable);
                    this.writer.Write(user.UserId);
                    this.writer.Flush();
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
            this.writer.Write(ImStatuses.IM_Send);
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
