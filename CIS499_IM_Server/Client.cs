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
        public BinaryReader br;
        public BinaryWriter bw;

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

                br = new BinaryReader(ssl, Encoding.UTF8);
                bw = new BinaryWriter(ssl, Encoding.UTF8);

                // Say "hello".
                bw.Write(ImStatuses.IM_Hello);
                bw.Flush();
                int hello = br.ReadInt32();
                if (hello == ImStatuses.IM_Hello)
                {
                    bw.Write(ImStatuses.IM_Login);
                    bw.Flush();
                    var taco = br.ReadInt32();
                    // Hello packet is OK. Time to wait for login or register.
                    if (taco == ImStatuses.IM_Login)
                    {
                        IFormatter formatter = new BinaryFormatter();
                        UserClass user = (UserClass)formatter.Deserialize(ssl);
                        bw.Write(ImStatuses.IM_IsAvailable);
                    }
                    byte logMode = br.ReadByte();
                    string userName = br.ReadString();
                    string password = br.ReadString();
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
                                    bw.Write(ImStatuses.IM_OK);
                                    bw.Flush();
                                    Console.WriteLine("[{0}] ({1}) Registered new user", DateTime.Now, userName);
                                    prog.SaveUsers();
                                    Receiver();  // Listen to client in loop.
                                }
                                else
                                    bw.Write(ImStatuses.IM_Exists);
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
                                        bw.Write(ImStatuses.IM_OK);
                                        bw.Flush();
                                        Receiver();  // Listen to client in loop.
                                    }
                                    else
                                        bw.Write(ImStatuses.IM_WrongPass);
                                }
                                else
                                    bw.Write(ImStatuses.IM_NoExists);
                            }
                        }
                        else
                            bw.Write(ImStatuses.IM_TooPassword);
                    }
                    else
                        bw.Write(ImStatuses.IM_TooUsername);
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
                br.Close();
                bw.Close();
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
                    byte type = br.ReadByte();  // Get incoming packet type.

                    if (type == ImStatuses.IM_IsAvailable)
                    {
                        string who = br.ReadString();

                        bw.Write(ImStatuses.IM_IsAvailable);
                        bw.Write(who);

                        UserInfo info;
                        if (prog.Users.TryGetValue(who, out info))
                        {
                            if (info.LoggedIn)
                                bw.Write(true);   // Available
                            else
                                bw.Write(false);  // Unavailable
                        }
                        else
                            bw.Write(false);      // Unavailable
                        bw.Flush();
                    }
                    else if (type == ImStatuses.IM_Send)
                    {
                        string to = br.ReadString();
                        string msg = br.ReadString();

                        UserInfo recipient;
                        if (prog.Users.TryGetValue(to, out recipient))
                        {
                            // Is recipient logged in?
                            if (recipient.LoggedIn)
                            {
                                // Write received packet to recipient
                                recipient.Connection.bw.Write(ImStatuses.IM_Received);
                                recipient.Connection.bw.Write(userInfo.UserName);  // From
                                recipient.Connection.bw.Write(msg);
                                recipient.Connection.bw.Flush();
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
