// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SocketHelper.cs" company="Sam Novak">
//   CIS499 - 2013 - IM Server
// </copyright>
// <summary>
//   The socket helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CIS499_IM_Server
{
    using System.Net.Sockets;
    using System.Text;

    /// <summary>
    /// The socket helper.
    /// </summary>
    internal class SocketHelper
    {
        /// <summary>
        /// The client.
        /// </summary>
        internal TcpClient MscClient;

        /// <summary>
        /// The message.
        /// </summary>
        internal string MstrMessage;

        /// <summary>
        /// The response.
        /// </summary>
        internal string MstrResponse;

        /// <summary>
        /// The bytes sent.
        /// </summary>
        internal byte[] BytesSent;

        /// <summary>
        /// The process message.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="bytesReceived">
        /// The bytes received.
        /// </param>
        public void ProcessMsg(TcpClient client, NetworkStream stream, byte[] bytesReceived)
        {
            // Handle the message received and  
            // send a response back to the client.
            this.MstrMessage = Encoding.ASCII.GetString(bytesReceived, 0, bytesReceived.Length);
            this.MscClient = client;
            this.MstrMessage = this.MstrMessage.Substring(0, 5);
            if (this.MstrMessage.Equals("Hello"))
            {
                this.MstrResponse = "Goodbye";
            }
            else
            {
                this.MstrResponse = "What?";
            }

            this.BytesSent = Encoding.ASCII.GetBytes(this.MstrResponse);
            stream.Write(this.BytesSent, 0, this.BytesSent.Length);
        }
    }
}
