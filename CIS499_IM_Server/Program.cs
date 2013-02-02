using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Net;
using System.Diagnostics;

namespace CIS499_IM_Server
{
    using System.IO;
    using System.Net.Sockets;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Security.Cryptography.X509Certificates;

    public class Program
    {
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

        static void Main(string[] args)
        {
            Program p = new Program();
            Console.WriteLine();
            Console.WriteLine("Press enter to close program.");
            Console.ReadLine();
        }

        // Self-signed certificate for SSL encryption.
        // You can generate one using my generate_cert script in tools directory (OpenSSL is required).
        public X509Certificate2 cert = new X509Certificate2("server.pfx", "instant");

        // IP of this computer. If you are running all clients at the same computer you can use 127.0.0.1 (localhost). 
        public IPAddress ip = IPAddress.Parse("127.0.0.1");
        public int port = 2000;
        public bool running = true;
        public TcpListener server;

        public Dictionary<string, UserInfo> users = new Dictionary<string, UserInfo>();  // Information about users + connections info.

        public Program()
        {
            Console.Title = "InstantMessenger Server";
            Console.WriteLine("----- InstantMessenger Server -----");
            LoadUsers();
            Console.WriteLine("[{0}] Starting server...", DateTime.Now);

            server = new TcpListener(ip, port);
            server.Start();
            Console.WriteLine("[{0}] Server is running properly!", DateTime.Now);

            Listen();
        }

        void Listen()  // Listen to incoming connections.
        {
            while (running)
            {
                TcpClient tcpClient = server.AcceptTcpClient();  // Accept incoming connection.
                Client client = new Client(this, tcpClient);     // Handle in another thread.
            }
        }

        string usersFileName = Environment.CurrentDirectory + "\\users.dat";
        public void SaveUsers()  // Save users data
        {
            try
            {
                Console.WriteLine("[{0}] Saving users...", DateTime.Now);
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = new FileStream(usersFileName, FileMode.Create, FileAccess.Write);
                bf.Serialize(file, users.Values.ToArray());  // Serialize UserInfo array
                file.Close();
                Console.WriteLine("[{0}] Users saved!", DateTime.Now);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void LoadUsers()  // Load users data
        {
            try
            {
                Console.WriteLine("[{0}] Loading users...", DateTime.Now);
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = new FileStream(usersFileName, FileMode.Open, FileAccess.Read);
                UserInfo[] infos = (UserInfo[])bf.Deserialize(file);      // Deserialize UserInfo array
                file.Close();
                users = infos.ToDictionary((u) => u.UserName, (u) => u);  // Convert UserInfo array to Dictionary
                Console.WriteLine("[{0}] Users loaded! ({1})", DateTime.Now, users.Count);
            }
            catch { }
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
