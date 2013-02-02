﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Sam Novak">
//   CIS499 - 2013 - IM Server
// </copyright>
// <summary>
//   Defines the Program type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CIS499_IM_Server
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Security.Cryptography.X509Certificates;
    using System.ServiceProcess;
    using System.Text;

    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        #region Properties and Fields
        /// <summary>
        /// The name of the process logging events
        /// In this case "C# Instant message server"
        /// </summary>
        private const string EventSource = "C# Instant message server";

        /// <summary>
        /// Target event log for event logging
        /// System, Application, etc
        /// </summary>
        private const string EventLogName = "Application";

        #region Runtime configuration

        /// <summary>
        /// Port is read when the application starts
        /// </summary>
        private int port;

        /// <summary>
        /// List of clients.
        /// Clients are added as they connect.
        /// When they initially hit the port the server will send back something
        /// and if the client returns something particular they get added to the list.
        /// </summary>
        private List<IPAddress> clients;

        // Self-signed certificate for SSL encryption.
        // You can generate one using my generate_
        // cert script in tools directory (OpenSSL is required).

        /// <summary>
        /// Certificate for encryption
        /// </summary>
        internal X509Certificate2 Cert;

        // IP of this computer. If you are running all clients at the same computer you can use 127.0.0.1 (localhost). 
        // public IPAddress ip = IPAddress.Parse("127.0.0.1");
        // public int port = 2000;

        /// <summary>
        /// State of the server
        /// </summary>
        public bool Running = true;

        /// <summary>
        /// TCP Listener for the server
        /// </summary>
        public TcpListener Server;

        #endregion
        #endregion

        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The arguments.
        /// </param>
        public static void Main(string[] args)
        {

            var p = new Program();
            Console.WriteLine();
            Console.WriteLine("Press enter to close program.");
            Console.ReadLine();
        }


        /// <summary>
        /// Users dictionary
        /// </summary>
        public Dictionary<string, UserInfo> Users = new Dictionary<string, UserInfo>();  // Information about users + connections info.

        /// <summary>
        /// The program! 
        /// </summary>
        /// <param name="clients">
        /// The clients to be used
        /// </param>
        public Program(List<IPAddress> clients)
        {
            this.clients = clients;
            this.LoadSettings();
            this.WriteEvent("IM server starting");
            Console.Title = "InstantMessenger Server";
            Console.WriteLine("----- InstantMessenger Server -----");
            LoadUsers();
            Console.WriteLine("[{0}] Starting server...", DateTime.Now);

            this.Server = new TcpListener(IPAddress.Parse("127.0.0.1"), this.port);
            this.Server.Start();
            Console.WriteLine("[{0}] Server is running properly!", DateTime.Now);

            Listen();
        }

        void Listen()  // Listen to incoming connections.
        {
            while (Running)
            {
                TcpClient tcpClient = this.Server.AcceptTcpClient();  // Accept incoming connection.
                // TODO Setup the client verification part
                Client client = new Client(this, tcpClient);     // Handle in another thread.
            }
        }

        string usersFileName = Environment.CurrentDirectory + "\\users.dat";

        private Program()
        {
        }

        /// <summary>
        /// Saves Users
        /// </summary>
        public void SaveUsers()  // Save users data
        {
            try
            {
                Console.WriteLine("[{0}] Saving users...", DateTime.Now);
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = new FileStream(usersFileName, FileMode.Create, FileAccess.Write);
                bf.Serialize(file, this.Users.Values.ToArray());  // Serialize UserInfo array
                file.Close();
                Console.WriteLine("[{0}] Users saved!", DateTime.Now);
            }
            catch (Exception e)
            {
                this.WriteError(e);
            }
        }

        /// <summary>
        /// Loads users
        /// </summary>
        public void LoadUsers()  // Load users data
        {
            try
            {
                Console.WriteLine("[{0}] Loading users...", DateTime.Now);
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = new FileStream(usersFileName, FileMode.Open, FileAccess.Read);
                UserInfo[] infos = (UserInfo[])bf.Deserialize(file); // Deserialize UserInfo array
                file.Close();
                this.Users = infos.ToDictionary((u) => u.UserName, (u) => u); // Convert UserInfo array to Dictionary
                Console.WriteLine("[{0}] Users loaded! ({1})", DateTime.Now, this.Users.Count);
            }
            catch (Exception ex)
            {
                this.WriteError(ex);
            }
        }

        /// <summary>
        /// Load settings at startup
        /// </summary>
        private void LoadSettings()
        {
            this.clients.Add(IPAddress.Parse("127.0.0.1"));
            this.port = int.Parse(System.Configuration.ConfigurationSettings.AppSettings["port"]);
            this.Cert = new X509Certificate2(
                System.Configuration.ConfigurationSettings.AppSettings["certName"],
                System.Configuration.ConfigurationSettings.AppSettings["certPass"]);

        }

        #region Event Log
        /// <summary>
        /// The write event.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void WriteEvent(string message)
        {
            EventLog.WriteEntry(EventSource, message);
        }

        /// <summary>
        /// For logging exceptions during run time
        /// </summary>
        /// <param name="ex">
        /// The exception to be logged
        /// </param>
        internal void WriteError(Exception ex)
        {
            EventLog.WriteEntry(EventSource, ex.Message, EventLogEntryType.Error, 1144);
        }

        /// <summary>
        /// Creates the event log source for the service.
        /// Only ran during startup of the service.
        /// </summary>
        private void CreateEventSource()
        {
            if (!EventLog.SourceExists(EventSource))
            {
                EventLog.CreateEventSource(EventSource, EventLogName);
            }
        }

        #endregion
    }

    #region extra code
    //using System.Net.Sockets;
    //using System.Threading;

    //static class Program
    //{
    //    /// <summary>
    //    /// http://msdn.microsoft.com/en-us/library/bb397809%28v=vs.90%29.aspx
    //    /// The main entry point for the application.
    //    /// </summary>
    //    static void Main()
    //    {
    //        ServiceBase[] ServicesToRun;
    //        ServicesToRun = new ServiceBase[] 
    //        { 
    //            new Main_Class() 
    //        };
    //        ServiceBase.Run(ServicesToRun);
    //    }


    //    static void Listen()
    //    {
    //        string output;
    //        // Create an instance of the TcpListener class.
    //        TcpListener tcpListener = null;
    //        IPAddress ipAddress = Dns.GetHostEntry("localhost").AddressList[0];
    //        try
    //        {
    //            eve
    //                // Set the listener on the local IP address 
    //                // and specify the port.
    //            tcpListener = new TcpListener(ipAddress, 13);
    //            tcpListener.Start();
    //            output = "Waiting for a connection...";
    //        }
    //        catch (Exception e)
    //        {
    //            output = "Error: " + e.ToString();
    //        }
    //        while (true)
    //        {
    //            // Always use a Sleep call in a while(true) loop 
    //            // to avoid locking up your CPU.
    //            Thread.Sleep(10);
    //            // Create a TCP socket. 
    //            // If you ran this server on the desktop, you could use 
    //            // Socket socket = tcpListener.AcceptSocket() 
    //            // for greater flexibility.
    //            TcpClient tcpClient = tcpListener.AcceptTcpClient();
    //            // Read the data stream from the client. 
    //            byte[] bytes = new byte[256];
    //            NetworkStream stream = tcpClient.GetStream();
    //            stream.Read(bytes, 0, bytes.Length);
    //            SocketHelper helper = new SocketHelper();
    //            helper.processMsg(tcpClient, stream, bytes);
    //        }
    //    }
    //}
    #endregion
}
