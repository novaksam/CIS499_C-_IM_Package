using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Threading;

namespace CIS499_IM_Server
{
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;

    public class Client
    {
        public Client(Program p, TcpClient c)
        {
            prog = p;
            client = c;

            // Handle client in another thread.
            (new Thread(new ThreadStart(SetupConn))).Start();
        }

        Program prog;
        public TcpClient client;
        public NetworkStream netStream;  // Raw-data stream of connection.
        public SslStream ssl;            // Encrypts connection using SSL.
        public BinaryReader Reader;
        public BinaryWriter Writer;

        UserInfo userInfo;  // Information about current user.
        
        void SetupConn()  // Setup connection and login or register.
        {
            try
            {
                Console.WriteLine("[{0}] New connection!", DateTime.Now);
                netStream = client.GetStream();
                ssl = new SslStream(netStream, false);
                ssl.AuthenticateAsServer(prog.Cert, false, SslProtocols.Tls, true);
                Console.WriteLine("[{0}] Connection authenticated!", DateTime.Now);
                // Now we have encrypted connection.

                this.Reader = new BinaryReader(ssl, Encoding.UTF8);
                this.Writer = new BinaryWriter(ssl, Encoding.UTF8);

                // Say "hello".
                Writer.Write(ImStatuses.IM_Hello);
                Writer.Flush();
                int hello = this.Reader.ReadInt32();
                if (hello == ImStatuses.IM_Hello)
                {
                    //Writer.Write(ImStatuses.IM_Login);
                    //Writer.Flush();
                    
                    var taco = this.Reader.ReadByte();
                    // Hello packet is OK. Time to wait for login or register.
                    if (taco == ImStatuses.IM_Login)
                    {
                        var length = this.Reader.ReadInt32();
                        var use = this.Reader.ReadBytes(length);
                        var user = UserClass.Deserialize(use);
                        Writer.Write(ImStatuses.IM_IsAvailable);
                    }






                    byte logMode = this.Reader.ReadByte();
                    string userName = this.Reader.ReadString();
                    string password = this.Reader.ReadString();
                    if (userName.Length < 10) // Isn't username too long?
                    {
                        if (password.Length < 20)  // Isn't password too long?
                        {
                            if (logMode == ImStatuses.IM_Register)  // Register mode
                            {
                                if (!prog.Users.ContainsKey(userName))  // User already exists?
                                {
                                    userInfo = new UserInfo(userName, password, this);
                                    prog.Users.Add(userName, userInfo);  // Add new user
                                    Writer.Write(ImStatuses.IM_OK);
                                    Writer.Flush();
                                    Console.WriteLine("[{0}] ({1}) Registered new user", DateTime.Now, userName);
                                    prog.SaveUsers();
                                    Receiver();  // Listen to client in loop.
                                }
                                else
                                    Writer.Write(ImStatuses.IM_Exists);
                            }
                            else if (logMode == ImStatuses.IM_Login)  // Login mode
                            {
                                if (prog.Users.TryGetValue(userName, out userInfo))  // User exists?
                                {
                                    if (password == userInfo.Password)  // Is password OK?
                                    {
                                        // If user is logged in yet, disconnect him.
                                        if (userInfo.LoggedIn)
                                            userInfo.Connection.CloseConn();

                                        userInfo.Connection = this;
                                        Writer.Write(ImStatuses.IM_OK);
                                        Writer.Flush();
                                        Receiver();  // Listen to client in loop.
                                    }
                                    else
                                        Writer.Write(ImStatuses.IM_WrongPass);
                                }
                                else
                                    Writer.Write(ImStatuses.IM_NoExists);
                            }
                        }
                        else
                            Writer.Write(ImStatuses.IM_TooPassword);
                    }
                    else
                        Writer.Write(ImStatuses.IM_TooUsername);
                }
                CloseConn();
            }
            catch { CloseConn(); }
        }
        void CloseConn() // Close connection.
        {
            try
            {
                userInfo.LoggedIn = false;
                this.Reader.Close();
                Writer.Close();
                ssl.Close();
                netStream.Close();
                client.Close();
                Console.WriteLine("[{0}] End of connection!", DateTime.Now);
            }
            catch { }
        }
        void Receiver()  // Receive all incoming packets.
        {
            Console.WriteLine("[{0}] ({1}) User logged in", DateTime.Now, userInfo.UserName);
            userInfo.LoggedIn = true;

            try
            {
                while (client.Client.Connected)  // While we are connected.
                {
                    byte type = this.Reader.ReadByte();  // Get incoming packet type.

                    if (type == ImStatuses.IM_IsAvailable)
                    {
                        string who = this.Reader.ReadString();

                        Writer.Write(ImStatuses.IM_IsAvailable);
                        Writer.Write(who);

                        UserInfo info;
                        if (prog.Users.TryGetValue(who, out info))
                        {
                            if (info.LoggedIn)
                                Writer.Write(true);   // Available
                            else
                                Writer.Write(false);  // Unavailable
                        }
                        else
                            Writer.Write(false);      // Unavailable
                        Writer.Flush();
                    }
                    else if (type == ImStatuses.IM_Send)
                    {
                        string to = this.Reader.ReadString();
                        string msg = this.Reader.ReadString();

                        UserInfo recipient;
                        if (prog.Users.TryGetValue(to, out recipient))
                        {
                            // Is recipient logged in?
                            if (recipient.LoggedIn)
                            {
                                // Write received packet to recipient
                                recipient.Connection.Writer.Write(ImStatuses.IM_Received);
                                recipient.Connection.Writer.Write(userInfo.UserName);  // From
                                recipient.Connection.Writer.Write(msg);
                                recipient.Connection.Writer.Flush();
                                Console.WriteLine("[{0}] ({1} -> {2}) Message sent!", DateTime.Now, userInfo.UserName, recipient.UserName);
                            }
                        }
                    }
                }
            }
            catch (IOException) { }

            userInfo.LoggedIn = false;
            Console.WriteLine("[{0}] ({1}) User logged out", DateTime.Now, userInfo.UserName);
        }

        //public const int IM_Hello = 2012;      // Hello
        //public const byte IM_OK = 0;           // OK
        //public const byte IM_Login = 1;        // Login
        //public const byte IM_Register = 2;     // Register
        //public const byte IM_TooUsername = 3;  // Too long username
        //public const byte IM_TooPassword = 4;  // Too long password
        //public const byte IM_Exists = 5;       // Already exists
        //public const byte IM_NoExists = 6;     // Doesn't exists
        //public const byte IM_WrongPass = 7;    // Wrong password
        //public const byte IM_IsAvailable = 8;  // Is user available?
        //public const byte IM_Send = 9;         // Send message
        //public const byte IM_Received = 10;    // Message received
    }
}
