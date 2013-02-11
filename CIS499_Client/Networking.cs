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
    using System.Security.Cryptography.X509Certificates;
    using System.Text;

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
        /// The security stream.
        /// </summary>
        private SslStream ssl;

        /// <summary>
        /// Initializes a new instance of the <see cref="Networking"/> class. 
        /// Default constructor
        /// </summary>
        public Networking()
        {
            // Creates the connection
            this.tcpClient = new TcpClient(Settings.Default.Server, Settings.Default.Port);

            if (!this.tcpClient.Connected)
            {
                throw this.failedConnect;
            }
            
            // this.Ping(tcpClient);

            NetworkStream stream = this.tcpClient.GetStream();
            this.ssl = new SslStream(stream, false, ValidateCert);
            this.ssl.AuthenticateAsClient(Settings.Default.Cert_Owner);
            this.writer = new BinaryWriter(this.ssl, Encoding.UTF8);
            this.reader = new BinaryReader(this.ssl, Encoding.UTF8);

        }

        // private void Ping(TcpClient client)
        // {
        //    NetworkStream stream = client.GetStream();
        //    BinaryWriter writeTemp = new BinaryWriter(stream, Encoding.UTF8);
        //    byte[] buffer = new byte[8];
        //    writeTemp.Write(ImStatuses.Im_Hello);
        //    writeTemp.Flush();
        // }

        internal void SendMessage(TcpClient client, Packet packet)
        {
           
        }

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
