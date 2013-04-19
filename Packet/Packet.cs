// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Packet.cs" company="Sam Novak">
//   IM Packet class - CNMT 499
// </copyright>
// <summary>
//   This class serves to provide a packet structure for all message related interactions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Packet
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.Serialization.Formatters.Binary;

    /// <summary>
    ///     This class serves to provide a packet structure for all message related interactions.
    /// </summary>
    [Serializable]
    public class Packet
    {
        #region Constructors and Destructors

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
        public Packet(int sender, int recep, string mess)
        {
            this.SenderId = sender;
            this.RecipientId = recep;
            this.Message = mess;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     String containing the message being sent.
        ///     this might be encrypted later, but for now clear text.
        /// </summary>
        public string Message { get; private set; }

        #endregion

        #region Properties

        /// <summary>
        ///     UserID of the target
        /// </summary>
        public int RecipientId { get; private set; }

        /// <summary>
        ///     UserID of the sender
        /// </summary>
        public int SenderId { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// De serialize
        /// </summary>
        /// <param name="bytes">
        /// the array of bytes to convert back to the class
        /// </param>
        /// <returns>
        /// The user
        /// </returns>
        public static Packet Deserialize(byte[] bytes)
        {
            var memStream = new MemoryStream();
            var binForm = new BinaryFormatter { AssemblyFormat = FormatterAssemblyStyle.Simple };
            memStream.Write(bytes, 0, bytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            var obj = (Packet)binForm.Deserialize(memStream);
            return obj;
        }

        /// <summary>
        /// The serialize.
        /// </summary>
        /// <param name="obj">
        /// The user
        /// </param>
        /// <returns>
        /// The byte array of the user
        /// </returns>
        public static byte[] Serialize(Packet obj)
        {
            if (obj == null)
            {
                return null;
            }

            var bf = new BinaryFormatter { AssemblyFormat = FormatterAssemblyStyle.Simple };
            var ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        #endregion
    }
}