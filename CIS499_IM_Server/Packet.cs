using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIS499_IM_Server
{
    class Packet
    {

        /// <summary>
        /// String containing the message being sent.
        /// this might be encrypted later, but for now clear text.
        /// </summary>
        private string Message;

        /// <summary>
        /// Sender IP address
        /// </summary>
        private string IPSrc;

        private uint SenderID;

        private uint RecepientID;
    }
}
