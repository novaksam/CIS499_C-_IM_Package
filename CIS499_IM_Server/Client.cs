// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Client.cs" company="Sam Novak">
//   CIS499 - 2013 - IM Server
// </copyright>
// <summary>
//   Defines the Client type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CIS499_IM_Server
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Security.Authentication;
    using System.Text;
    using System.Threading;
    using UserClass;

    /// <summary>
    /// The client.
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <param name="c">
        /// The c.
        /// </param>
        public Client(Program p, TcpClient c)
        {
            this.Prog = p;
            this.client = c;

            // Handle client in another thread.
            (new Thread(this.SetupConn)).Start();
        }

        /// <summary>
        /// The program.
        /// </summary>
        internal Program Prog;

        /// <summary>
        /// The client.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:AccessibleFieldsMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
        // ReSharper disable InconsistentNaming
        private TcpClient client;
        // ReSharper restore InconsistentNaming

        /// <summary>
        /// The net stream.
        /// </summary>
        private NetworkStream netStream;  // Raw-data stream of connection.

        /// <summary>
        /// The secure connection
        /// </summary>
        private SslStream ssl;            // Encrypts connection using SSL.

        /// <summary>
        /// The reader.
        /// </summary>
        private BinaryReader reader;

        /// <summary>
        /// The writer.
        /// </summary>
        private BinaryWriter writer;

        /// <summary>
        /// The user info.
        /// </summary>
        private UserInfo userInfo;  // Information about current user.

        /// <summary>
        /// The setup connection.
        /// </summary>
        internal void SetupConn()  // Setup connection and login or register.
        {
            try
            {
                Console.WriteLine("[{0}] New connection!", DateTime.Now);
                this.netStream = this.client.GetStream();
                this.ssl = new SslStream(this.netStream, false);
                this.ssl.AuthenticateAsServer(this.Prog.Cert, false, SslProtocols.Tls, true);
                Console.WriteLine("[{0}] Connection authenticated!", DateTime.Now);

                // Now we have encrypted connection.
                this.reader = new BinaryReader(this.ssl, Encoding.UTF8);
                this.writer = new BinaryWriter(this.ssl, Encoding.UTF8);

                // Say "hello".
                this.writer.Write(ImStatuses.IM_Hello);
                this.writer.Flush();
                var hello = this.reader.ReadInt32();
                if (hello == ImStatuses.IM_Hello)
                {
                    // Writer.Write(ImStatuses.IM_Login);
                    // Writer.Flush();
                    var taco = this.reader.ReadByte();

                    // Hello packet is OK. Time to wait for login or register.
                    if (taco == ImStatuses.IM_Login)
                    {
                        var length = this.reader.ReadInt32();
                        var use = this.reader.ReadBytes(length);
                        var user = UserClass.Deserialize(use);
                        this.writer.Write(ImStatuses.IM_IsAvailable);

                        // TODO call database to verify user
                        // TODO call database to verify password
                        // TODO return statement to user
                    }

                    var logMode = this.reader.ReadByte();
                    var userName = this.reader.ReadString();
                    var password = this.reader.ReadString();

                    // Isn't username too long?
                    if (userName.Length < 10)
                    {
                        // Isn't password too long?
                        if (password.Length < 20)
                        {
                            // Register mode
                            if (logMode == ImStatuses.IM_Register)
                            {
                                // User already exists?
                                if (!this.Prog.Users.ContainsKey(userName))
                                {
                                    this.userInfo = new UserInfo(userName, password, this);
                                    this.Prog.Users.Add(userName, this.userInfo); // Add new user
                                    this.writer.Write(ImStatuses.IM_OK);
                                    this.writer.Flush();
                                    Console.WriteLine("[{0}] ({1}) Registered new user", DateTime.Now, userName);
                                    this.Prog.SaveUsers();
                                    this.Receiver(); // Listen to client in loop.
                                }
                                else
                                {
                                    this.writer.Write(ImStatuses.IM_Exists);
                                }
                            }
                            else if (logMode == ImStatuses.IM_Login)
                            {
                                // Login mode
                                // User exists?
                                if (this.Prog.Users.TryGetValue(userName, out this.userInfo))
                                {
                                    // Is password OK?
                                    if (password == this.userInfo.Password)
                                    {
                                        // If user is logged in yet, disconnect him.
                                        if (this.userInfo.LoggedIn)
                                        {
                                            this.userInfo.Connection.CloseConn();
                                        }

                                        this.userInfo.Connection = this;
                                        this.writer.Write(ImStatuses.IM_OK);
                                        this.writer.Flush();
                                        this.Receiver(); // Listen to client in loop.
                                    }
                                    else
                                    {
                                        this.writer.Write(ImStatuses.IM_WrongPass);
                                    }
                                }
                                else
                                {
                                    this.writer.Write(ImStatuses.IM_NoExists);
                                }
                            }
                        }
                        else
                        {
                            this.writer.Write(ImStatuses.IM_TooPassword);
                        }
                    }
                    else
                    {
                        this.writer.Write(ImStatuses.IM_TooUsername);
                    }
                }

                this.CloseConn();
            }
            catch
            {
                this.CloseConn();
            }
        }

        /// <summary>
        /// The close connection
        /// </summary>
        internal void CloseConn()
        {
            try
            {
                this.userInfo.LoggedIn = false;
                this.reader.Close();
                this.writer.Close();
                this.ssl.Close();
                this.netStream.Close();
                this.client.Close();
                Console.WriteLine("[{0}] End of connection!", DateTime.Now);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// The receiver.
        /// </summary>
        internal void Receiver()  // Receive all incoming packets.
        {
            Console.WriteLine("[{0}] ({1}) User logged in", DateTime.Now, this.userInfo.UserName);
            this.userInfo.LoggedIn = true;

            try
            {
                // While we are connected.
                while (this.client.Client.Connected)  
                {
                    var type = this.reader.ReadByte();  // Get incoming packet type.

                    if (type == ImStatuses.IM_IsAvailable)
                    {
                        string who = this.reader.ReadString();

                        this.writer.Write(ImStatuses.IM_IsAvailable);
                        this.writer.Write(who);

                        UserInfo info;
                        if (this.Prog.Users.TryGetValue(who, out info))
                        {
                            if (info.LoggedIn) 
                            {
                                this.writer.Write(true); // Available
                            }
                            else 
                            {
                                this.writer.Write(false);  // Unavailable
                            }
                        }
                        else
                        {
                            this.writer.Write(false);      // Unavailable
                        }

                        this.writer.Flush();
                    }
                    else if (type == ImStatuses.IM_Send)
                    {
                        var to = this.reader.ReadString();
                        var msg = this.reader.ReadString();

                        UserInfo recipient;
                        if (this.Prog.Users.TryGetValue(to, out recipient))
                        {
                            // Is recipient logged in?
                            if (recipient.LoggedIn)
                            {
                                // Write received packet to recipient
                                recipient.Connection.writer.Write(ImStatuses.IM_Received);
                                recipient.Connection.writer.Write(this.userInfo.UserName);  // From
                                recipient.Connection.writer.Write(msg);
                                recipient.Connection.writer.Flush();
                                Console.WriteLine("[{0}] ({1} -> {2}) Message sent!", DateTime.Now, this.userInfo.UserName, recipient.UserName);
                            }
                        }
                    }
                }
            }
            catch (IOException)
            {
            }

            this.userInfo.LoggedIn = false;
            Console.WriteLine("[{0}] ({1}) User logged out", DateTime.Now, this.userInfo.UserName);
        }

        // public const int IM_Hello = 2012;      // Hello
        // public const byte IM_OK = 0;           // OK
        // public const byte IM_Login = 1;        // Login
        // public const byte IM_Register = 2;     // Register
        // public const byte IM_TooUsername = 3;  // Too long username
        // public const byte IM_TooPassword = 4;  // Too long password
        // public const byte IM_Exists = 5;       // Already exists
        // public const byte IM_NoExists = 6;     // Doesn't exists
        // public const byte IM_WrongPass = 7;    // Wrong password
        // public const byte IM_IsAvailable = 8;  // Is user available?
        // public const byte IM_Send = 9;         // Send message
        // public const byte IM_Received = 10;    // Message received
    }
}
