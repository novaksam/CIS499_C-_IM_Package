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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Net;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Security.Authentication;
    using System.Text;
    using System.Threading;

    using CIS499_IM_Server.DatabaseClasses;

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
        /// <param name="db">
        /// reference to the database
        /// </param>
        /// <param name="dictionary">
        /// Dictionary for users
        /// </param>
        public Client(Program p, TcpClient c, ref UsersDBRepository db)
        {
            this.thread = new Thread(this.SetupConn);
            this.Prog = p;
            this.client = c;
            this.dbRepository = db;
        }

        /// <summary>
        /// Start the client connection thread.
        /// </summary>
        public void Start()
        {
            // Handle client in another thread.
            this.thread.Start();
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public void Stop()
        {
            this.thread.Join();
        }

        /// <summary>
        /// The thread
        /// </summary>
        private Thread thread;

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
        /// The database repository.
        /// </summary>
        private UsersDBRepository dbRepository;

        /// <summary>
        /// The setup connection.
        /// </summary>
        internal void SetupConn()  // Setup connection and login or register.
        {
            try
            {
                EventLogging.WriteEvent("New connection!", EventLogEntryType.Information);
                this.netStream = this.client.GetStream();
                this.ssl = new SslStream(this.netStream, false);
                this.ssl.AuthenticateAsServer(this.Prog.Cert, false, SslProtocols.Tls, true);
                EventLogging.WriteEvent("Connection authenticated!", EventLogEntryType.SuccessAudit);

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
                    var action = this.reader.ReadByte();

                    int length;
                    byte[] use;
                    UserClass user;

                    switch (action)
                    {
                        case ImStatuses.IM_Login:
                            // Get the length of the incoming byte array
                            length = this.reader.ReadInt32();

                            // Read said byte array
                            use = this.reader.ReadBytes(length);

                            // Convert that array into a user.
                            user = UserClass.Deserialize(use);

                            lock (this.dbRepository)
                            {
                                var temp = this.dbRepository.SelectByUserName(user.UserName)[0];

                                if (user.PasswordHash == temp.PasswordHash)
                                {
                                    // User logged in so return their account to them
                                    this.writer.Write(ImStatuses.IM_IsAvailable);
                                    var logg = UserClass.Serialize(temp);
                                    this.writer.Write(logg.Length);
                                    this.writer.Write(logg);
                                    lock (this.Prog.UserConnections)
                                    {
                                        if (temp.UserId != null)
                                        {
                                            this.Prog.UserConnections.Add(
                                                ((IPEndPoint)this.client.Client.RemoteEndPoint).Address, 
                                                (int)temp.UserId);
                                        }
                                    }

                                    this.Receiver();
                                }
                                else
                                {
                                    this.writer.Write(ImStatuses.IM_WrongPass);
                                }
                            }


                            // TODO call database to verify user
                            // TODO call database to verify password
                            // TODO return statement to user

                            break;

                        case ImStatuses.IM_Register:
                            // Get the length of the incoming byte array
                            length = this.reader.ReadInt32();

                            // Read said byte array
                            use = this.reader.ReadBytes(length);

                            // Convert that array into a user.
                            user = UserClass.Deserialize(use);
                            lock (this.dbRepository)
                            {
                                this.dbRepository.Create((UsersDB)user);
                            }
                            break;
                        case ImStatuses.IM_Logout:
                            break;
                    }
                    // Hello packet is OK. Time to wait for login or register.
                    if (action == ImStatuses.IM_Login)
                    {

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
                                //// User already exists?
                                //if (!this.Prog.UserList.Contains(userName))
                                //{
                                //    this.userInfo = new UserInfo(userName, password, this);
                                //    //this.Prog.Users.Add(userName, this.userInfo); // Add new user
                                //    this.writer.Write(ImStatuses.IM_OK);
                                //    this.writer.Flush();
                                //    Console.WriteLine("[{0}] ({1}) Registered new user", DateTime.Now, userName);
                                //    this.Prog.SaveUsers();
                                //    this.Receiver(); // Listen to client in loop.
                                //}
                                //else
                                //{
                                //    this.writer.Write(ImStatuses.IM_Exists);
                                //}
                            }
                            else if (logMode == ImStatuses.IM_Login)
                            {
                                // Login mode
                                // User exists?
                                //if (this.Prog.Users.TryGetValue(userName, out this.userInfo))
                                //{
                                //    // Is password OK?
                                //    if (password == this.userInfo.Password)
                                //    {
                                //        // If user is logged in yet, disconnect him.
                                //        if (this.userInfo.LoggedIn)
                                //        {
                                //            this.userInfo.Connection.CloseConn();
                                //        }

                                //        this.userInfo.Connection = this;
                                //        this.writer.Write(ImStatuses.IM_OK);
                                //        this.writer.Flush();
                                //        this.Receiver(); // Listen to client in loop.
                                //    }
                                //    else
                                //    {
                                //        this.writer.Write(ImStatuses.IM_WrongPass);
                                //    }
                                //}
                                //else
                                //{
                                //    this.writer.Write(ImStatuses.IM_NoExists);
                                //}
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
                        //if (this.Prog.Users.TryGetValue(who, out info))
                        //{
                        //    if (info.LoggedIn) 
                        //    {
                        //        this.writer.Write(true); // Available
                        //    }
                        //    else 
                        //    {
                        //        this.writer.Write(false);  // Unavailable
                        //    }
                        //}
                        //else
                        //{
                        //    this.writer.Write(false);      // Unavailable
                        //}

                        this.writer.Flush();
                    }
                    else if (type == ImStatuses.IM_Send)
                    {
                        var to = this.reader.ReadString();
                        var msg = this.reader.ReadString();

                        UserInfo recipient;
                        //if (this.Prog.Users.TryGetValue(to, out recipient))
                        //{
                        //    // Is recipient logged in?
                        //    if (recipient.LoggedIn)
                        //    {
                        //        // Write received packet to recipient
                        //        recipient.Connection.writer.Write(ImStatuses.IM_Received);
                        //        recipient.Connection.writer.Write(this.userInfo.UserName);  // From
                        //        recipient.Connection.writer.Write(msg);
                        //        recipient.Connection.writer.Flush();
                        //        Console.WriteLine("[{0}] ({1} -> {2}) Message sent!", DateTime.Now, this.userInfo.UserName, recipient.UserName);
                        //    }
                        //}
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
