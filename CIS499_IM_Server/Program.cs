// --------------------------------------------------------------------------------------------------------------------
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
    using System.Configuration;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Sockets;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;

    using CIS499_IM_Server.DatabaseClasses;

    using UserClass;

    /// <summary>
    ///     The program.
    /// </summary>
    public class Program
    {
        #region Static Fields

        /// <summary>
        ///     List of clients.
        ///     Clients are added as they connect.
        ///     When they initially hit the port the server will send back something
        ///     and if the client returns something particular they get added to the list.
        /// </summary>
        private static readonly List<IPAddress> clients = new List<IPAddress>();

        #endregion

        // Self-signed certificate for SSL encryption.
        // You can generate one using my generate_
        // cert script in tools directory (OpenSSL is required).

        // IP of this computer. If you are running all clients at the same computer you can use 127.0.0.1 (localhost). 
        // public IPAddress ip = IPAddress.Parse("127.0.0.1");
        // public int port = 2000;
        #region Fields

        /// <summary>
        ///     State of the server
        /// </summary>
        public bool Running = true;

        /// <summary>
        ///     TCP Listener for the server
        /// </summary>
        public TcpListener Server;

        /// <summary>
        ///     Users dictionary.
        ///     String of the IP address and the userID
        /// </summary>
        public Dictionary<int, TcpClient> UserConnections = new Dictionary<int, TcpClient>();

        /// <summary>
        ///     Certificate for encryption
        /// </summary>
        internal X509Certificate2 Cert;

        /// <summary>
        ///     The users file name.
        /// </summary>
        internal string UsersFileName = Environment.CurrentDirectory + "\\users.dat";

        /// <summary>
        ///     The users.
        /// </summary>
        internal List<UserClass> UserList;

        /// <summary>
        ///     The database repository.
        /// </summary>
        internal UsersDBRepository dbRepository = new UsersDBRepository();

        /// <summary>
        ///     The lock token.
        /// </summary>
        private bool lockToken;

        /// <summary>
        ///     Port is read when the application starts
        /// </summary>
        private int port;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Program"/> class.
        ///     The program!
        /// </summary>
        /// <param name="clients">
        /// The clients to be used
        /// </param>
        public Program(List<IPAddress> clients)
        {
            // this.clients = clients;
            this.LoadSettings();

            this.LoadUsers();
            EventLogging.WriteEvent("Starting server..." + DateTime.Now, EventLogEntryType.Information);

            this.Server = new TcpListener(IPAddress.Parse("127.0.0.1"), this.port);
            this.Server.Start();
            EventLogging.WriteEvent("Server is running properly!" + DateTime.Now, EventLogEntryType.Information);

            this.Listen();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The arguments.
        /// </param>
        public static void Main(string[] args)
        {
            // Adds local host
            clients.Add(IPAddress.Parse("127.0.0.1"));

            // Adds the local outward facing IP address
            clients.AddRange(Dns.GetHostEntry(Dns.GetHostName()).AddressList);

            // Starts the program
            var r = new Program(clients);

            // Console.WriteLine();
            // Console.WriteLine("Press enter to close program.");
            // Console.ReadLine();
        }

        /// <summary>
        ///     Loads users
        /// </summary>
        public void LoadUsers()
        {
            // Load users data
            //Monitor.TryEnter(this.dbRepository, -1, ref this.lockToken);
            try
            {
                lock (this.dbRepository)
                {
                    lock (this.UserList)
                    {
                        this.UserList = new List<UserClass>(this.dbRepository.ToList());
                    }
                }
            }
            catch (Exception exception)
            {
                EventLogging.WriteError(exception);
            }
            finally
            {
                //Monitor.Exit(this.dbRepository);
                //Monitor.Pulse(this.dbRepository);
            }

            // try
            // {
            // Console.WriteLine("[{0}] Loading users...", DateTime.Now);
            // var bf = new BinaryFormatter();
            // var file = new FileStream(this.UsersFileName, FileMode.Open, FileAccess.Read);
            // var infos = (UserInfo[])bf.Deserialize(file); // Deserialize UserInfo array
            // file.Close();
            // this.Users = infos.ToDictionary(u => u.UserName, u => u); // Convert UserInfo array to Dictionary
            // Console.WriteLine("[{0}] Users loaded! ({1})", DateTime.Now, this.Users.Count);
            // }
            // catch (Exception ex)
            // {
            // EventLogging.WriteError(ex);
            // }
        }

        /// <summary>
        ///     Saves Users
        /// </summary>
        [Obsolete]
        public void SaveUsers()
        {
            // Save users data
            // try
            // {
            // Console.WriteLine("[{0}] Saving users...", DateTime.Now);
            // var bf = new BinaryFormatter();
            // var file = new FileStream(this.UsersFileName, FileMode.Create, FileAccess.Write);
            // bf.Serialize(file, this.Users.Values.ToArray());  // Serialize UserInfo array
            // file.Close();
            // Console.WriteLine("[{0}] Users saved!", DateTime.Now);
            // }
            // catch (Exception e)
            // {
            // EventLogging.WriteError(e);
            // }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The listen.
        /// </summary>
        internal void Listen()
        {
            // Listen to incoming connections.
            while (this.Running)
            {
                TcpClient tcpClient = this.Server.AcceptTcpClient(); // Accept incoming connection.

                // TODO Setup the client verification part
                var client = new Client(this, tcpClient, ref this.dbRepository);
                client.Start();
            }
        }

        /// <summary>
        ///     Load settings at startup
        /// </summary>
        private void LoadSettings()
        {
            // clients.Add(IPAddress.Parse("127.0.0.1"));
#pragma warning disable 612,618
            this.port = int.Parse(ConfigurationSettings.AppSettings["port"]);
            this.Cert = new X509Certificate2(
                ConfigurationSettings.AppSettings["certName"], ConfigurationSettings.AppSettings["certPass"]);
#pragma warning restore 612,618
        }

        #endregion
    }

    #region extra code

    // using System.Net.Sockets;
    // using System.Threading;

    // static class Program
    // {
    // /// <summary>
    // /// http://msdn.microsoft.com/en-us/library/bb397809%28v=vs.90%29.aspx
    // /// The main entry point for the application.
    // /// </summary>
    // static void Main()
    // {
    // ServiceBase[] ServicesToRun;
    // ServicesToRun = new ServiceBase[] 
    // { 
    // new Main_Class() 
    // };
    // ServiceBase.Run(ServicesToRun);
    // }
    // static void Listen()
    // {
    // string output;
    // // Create an instance of the TcpListener class.
    // TcpListener tcpListener = null;
    // IPAddress ipAddress = Dns.GetHostEntry("localhost").AddressList[0];
    // try
    // {
    // eve
    // // Set the listener on the local IP address 
    // // and specify the port.
    // tcpListener = new TcpListener(ipAddress, 13);
    // tcpListener.Start();
    // output = "Waiting for a connection...";
    // }
    // catch (Exception e)
    // {
    // output = "Error: " + e.ToString();
    // }
    // while (true)
    // {
    // // Always use a Sleep call in a while(true) loop 
    // // to avoid locking up your CPU.
    // Thread.Sleep(10);
    // // Create a TCP socket. 
    // // If you ran this server on the desktop, you could use 
    // // Socket socket = tcpListener.AcceptSocket() 
    // // for greater flexibility.
    // TcpClient tcpClient = tcpListener.AcceptTcpClient();
    // // Read the data stream from the client. 
    // byte[] bytes = new byte[256];
    // NetworkStream stream = tcpClient.GetStream();
    // stream.Read(bytes, 0, bytes.Length);
    // SocketHelper helper = new SocketHelper();
    // helper.processMsg(tcpClient, stream, bytes);
    // }
    // }
    // }
    #endregion
}