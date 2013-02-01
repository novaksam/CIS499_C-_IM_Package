// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Packet.cs" company="Sam Novak">
//   CIS499 - 2013 - IM Server
// </copyright>
// <summary>
//   This class serves to provide a packet structure for all message related interactions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CIS499_IM_Server
{
    /// <summary>
    ///     This class serves to provide a packet structure for all message related interactions.
    /// </summary>
    internal class Packet
    {
        #region Fields

        /// <summary>
        ///     UserID of the target
        /// </summary>
        internal uint RecipientId { get; private set; }

        /// <summary>
        ///     UserID of the sender
        /// </summary>
        internal uint SenderId { get; private set; }

        /// <summary>
        ///     Target IP address
        /// </summary>
        internal string IpDst { get; private set; }

        /// <summary>
        ///     Sender IP address
        /// </summary>
        internal string IpSrc { get; private set; }

        /// <summary>
        ///     String containing the message being sent.
        ///     this might be encrypted later, but for now clear text.
        /// </summary>
        internal string Message { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Packet"/> class.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="recep">
        /// The Recipient.
        /// </param>
        /// <param name="mess">
        /// The mess.
        /// </param>
        public Packet(uint sender, uint recep, string mess)
        {
            this.SenderId = sender;
            this.RecipientId = recep;
            this.Message = mess;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Packet"/> class.
        /// </summary>
        /// <param name="ipDst">
        /// The destination IP
        /// </param>
        /// <param name="ipSrc">
        /// The source IP
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="recipientId">
        /// The Recipient.
        /// </param>
        /// <param name="senderId">
        /// The sender.
        /// </param>
        public Packet(string ipDst, string ipSrc, string message, uint recipientId, uint senderId)
        {
            this.IpDst = ipDst;
            this.IpSrc = ipSrc;
            this.Message = message;
            this.RecipientId = recipientId;
            this.SenderId = senderId;
        }

        #endregion
    }
}