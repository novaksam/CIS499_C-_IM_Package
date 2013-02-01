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
        ///     Target IP address
        /// </summary>
        private string IPDst;

        /// <summary>
        ///     Sender IP address
        /// </summary>
        private string IPSrc;

        /// <summary>
        ///     String containing the message being sent.
        ///     this might be encrypted later, but for now clear text.
        /// </summary>
        private string Message;

        /// <summary>
        ///     UserID of the target
        /// </summary>
        private uint RecepientID;

        /// <summary>
        ///     UserID of the sender
        /// </summary>
        private uint SenderID;

        #endregion

        #region 

        #endregion
    }
}