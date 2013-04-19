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
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Security.Authentication;
    using System.Text;
    using System.Threading;

    using CIS499_IM_Server.DatabaseClasses;

    using Imstatuses;

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
        ///     The p.
        /// </param>
        /// <param name="c">
        ///     The c.
        /// </param>
        public Client(Program p, TcpClient c)
        {
            this.thread = new Thread(this.SetupConn) { Name = "Client thread" };
            this.thread.SetApartmentState(ApartmentState.MTA);
            this.Prog = p;
            this.client = c;

            // this.dbRepository = db;
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
                this.writer.Write(ImStatuses.ImHello);
                this.writer.Flush();
                var hello = this.reader.ReadInt32();
                if (hello == ImStatuses.ImHello)
                {
                    // Writer.Write(ImStatuses.IM_Login);
                    // Writer.Flush();
                    var action = this.reader.ReadByte();

                    int length;
                    byte[] use;
                    UserClass user;

                    switch (action)
                    {
                        case ImStatuses.ImLogin:
                            // Get the length of the incoming byte array
                            length = this.reader.ReadInt32();

                            // Read said byte array
                            use = this.reader.ReadBytes(length);

                            // Convert that array into a user.
                            user = UserClass.Deserialize(use);

                            // TODO: research mutex
                            // lock (this.Prog.DBRepository)
                            // {
                            var list = this.Prog.DBRepository.SelectByUserName(user.UserName);

                            if (list.Count < 1)
                            {
                                this.writer.Write(ImStatuses.ImNoExists);
                                this.writer.Flush();
                                break;
                            }

                            var tempUse = list[0];
                            if (user.PasswordHash == tempUse.PasswordHash)
                            {
                                // User logged in so return their account to them
                                this.writer.Write(ImStatuses.ImOk);
                                var temp2 = new UserClass(
                                    tempUse.UserName,
                                    tempUse.UserId,
                                    tempUse.PasswordHash,
                                    true)
                                                {
                                                    Friends = tempUse.Friends
                                                };
                                var logg = UserClass.Serialize(temp2);
                                this.writer.Write(logg.Length);
                                this.writer.Write(logg);
                                this.writer.Flush();

                                // Add the connection to the database
                                lock (this.Prog.UserConnections)
                                {
                                    if (this.Prog.UserConnections.ContainsKey(tempUse.UserId))
                                    {
                                        this.Prog.UserConnections.Remove(tempUse.UserId);
                                    }

                                    this.Prog.UserConnections.Add(tempUse.UserId, this.client);
                                }

                                // With the call going to the receiver the
                                // temp should still be in scope
                                // this.userInfo = temp.Clone() as UserInfo;

                                // TODO check which of these actually work
                                this.userInfo = tempUse.Clone() as UserInfo;
                                if (this.userInfo == null)
                                {
                                    this.userInfo = new UserInfo
                                                        {
                                                            Connection = this,
                                                            UserId = tempUse.UserId,
                                                            UserName = tempUse.UserName,
                                                            PasswordHash = tempUse.PasswordHash,
                                                            Friends = tempUse.Friends
                                                        };

                                    // this.userInfo.Connection = this;
                                }

                                this.Receiver();
                            }
                            else
                            {
                                this.writer.Write(ImStatuses.ImWrongPass);
                                this.writer.Flush();

                                // this.CloseConn();
                            }

                            // }
                            break;

                        case ImStatuses.ImRegister:
                            // Get the length of the incoming byte array
                            length = this.reader.ReadInt32();

                            // Read said byte array
                            use = this.reader.ReadBytes(length);

                            // Convert that array into a user.
                            user = UserClass.Deserialize(use);

                            // lock (this.Prog.DBRepository)
                            // {
                            if (this.Prog.DBRepository.SelectByUserName(user.UserName) == null)
                            {
                                var temp = new UsersDB();
                                temp.UserName = user.UserName;
                                temp.PassHash = user.PasswordHash;
                                temp.Friends = user.Friends;
                                this.Prog.DBRepository.Create(temp);
                                this.writer.Write(ImStatuses.ImOk);
                                this.writer.Flush();
                            }
                            else
                            {
                                this.writer.Write(ImStatuses.ImExists);
                                this.writer.Flush();

                                // this.CloseConn();
                            }

                            // }
                            break;
                        default:
                            this.CloseConn();
                            break;
                    }
                }

                this.CloseConn();
            }
            catch (Exception ex)
            {
                EventLogging.WriteError(ex, "An error has occurred trying to setup the connection.");
                this.writer.Write(ImStatuses.ImError);
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
                if (this.userInfo != null)
                {
                    lock (this.Prog.UserConnections)
                    {
                        this.Prog.UserConnections.Remove(this.userInfo.UserId);
                    }

                    this.userInfo.LoggedIn = false;
                }

                this.reader.Close();
                this.writer.Close();
                this.ssl.Close();
                this.netStream.Close();
                this.client.Close();
                Console.WriteLine("[{0}] End of connection!", DateTime.Now);
            }
            catch (Exception ex)
            {
                EventLogging.WriteError(ex, "An exception has occurred at user logout.");
            }
        }

        /// <summary>
        /// The receiver.
        /// </summary>
        internal void Receiver()  // Receive all incoming packets.
        {
            EventLogging.WriteEvent("User " + this.userInfo.UserName + " has logged in.", EventLogEntryType.Information);
            this.userInfo.LoggedIn = true;

            try
            {
                // While we are connected.
                while (this.client.Client.Connected)
                {
                    var type = this.reader.ReadByte();  // Get incoming packet type.

                    switch (type)
                    {
                        case ImStatuses.ImIsAvailable:
                            {
                                string who = this.reader.ReadString();

                                this.writer.Write(ImStatuses.ImIsAvailable);
                                this.writer.Write(who);

                                // if (this.Prog.Users.TryGetValue(who, out info))
                                // {
                                //    if (info.LoggedIn) 
                                //    {
                                //        this.writer.Write(true); // Available
                                //    }
                                //    else 
                                //    {
                                //        this.writer.Write(false);  // Unavailable
                                //    }
                                // }
                                // else
                                // {
                                //    this.writer.Write(false);      // Unavailable
                                // }
                                this.writer.Flush();
                            }

                            break;
                        case ImStatuses.ImSend:
                            {
                                var to = this.reader.ReadString();
                                var msg = this.reader.ReadString();

                                UserInfo recipient;

                                // if (this.Prog.Users.TryGetValue(to, out recipient))
                                // {
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
                                // }
                            }

                            break;
                        case ImStatuses.ImSearch:
                            {
                                // Get the length of the incoming byte array
                                var length = this.reader.ReadInt32();

                                // Read said byte array
                                var use = this.reader.ReadBytes(length);

                                // Convert that array into a user.
                                var user = UserClass.Deserialize(use);

                                var name = this.reader.ReadString();
                                var temp = this.Prog.DBRepository.SearchByUserName(name, user);
                                var users = UserClass.StoreFriends(temp);
                                this.writer.Write(ImStatuses.ImSearch);
                                this.writer.Write(users.Length);
                                this.writer.Flush();
                                this.writer.Write(users);
                            }

                            break;
                        case ImStatuses.ImAddFriend:
                            {
                                // Get the length of the incoming byte array
                                var length = this.reader.ReadInt32();

                                // Read said byte array
                                var use = this.reader.ReadBytes(length);

                                // Convert that array into a user.
                                var user = UserClass.Deserialize(use);

                                // Length of the users to add
                                var usersLength = this.reader.ReadInt32();

                                // Read said byte array
                                var uses = this.reader.ReadBytes(usersLength);

                                // Convert to user list
                                var users = UserClass.RestoreFriends(uses);

                                // Add the selected friends to the range
                                // TODO validation
                                user.Friends.AddRange(users);

                                // Update it in the database
                                this.Prog.DBRepository.UpdateFriends(user);

                                this.writer.Write(ImStatuses.ImAddFriend);
                                var logg = UserClass.StoreFriends(user.Friends);
                                this.writer.Write(logg.Length);
                                this.writer.Write(logg);
                                this.writer.Flush();

                                // this.Prog.DBRepository.SelectByUserName(user.UserName);

                                // var name = this.reader.ReadString();
                                // this.Prog.DBRepository.
                            }

                            break;
                    }
                }
            }
            catch (IOException)
            {
            }

            this.userInfo.LoggedIn = false;
            Console.WriteLine("[{0}] ({1}) User logged out", DateTime.Now, this.userInfo.UserName);
        }
    }
}
