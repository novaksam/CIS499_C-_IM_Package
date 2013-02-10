using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace CIS499_Client
{
    using System.IO;
    using System.Net.Security;

    class Networking
    {
        private TcpClient tcpClient;

        private SslStream sslStream;
        private BinaryReader reader;
        private BinaryWriter Writer;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Networking()
        {
            tcpClient = new TcpClient(Settings.Default.Server, Settings.Default.Port);
            this.Ping(tcpClient);
        }

        private void Ping(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            
            BinaryWriter writeTemp = new BinaryWriter(stream, Encoding.UTF8);
            
            byte[] buffer = new byte[8];

            writeTemp.Write(31337);

            writeTemp.Flush();
        }

    }
}
