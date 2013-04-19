// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChatWindow.xaml.cs" company="Sam Novak">
//   C# Instant Messenger XAML client
// </copyright>
// <summary>
//   Interaction logic for ChatWindow
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CIS499_Client
{
    using System.Text;
    using System.Windows.Input;

    using Packet;

    using UserClass;

    /// <summary>
    /// Interaction logic for ChatWindow
    /// </summary>
    public partial class ChatWindow
    {
        /// <summary>
        /// The net.
        /// </summary>
        private Networking net;

        /// <summary>
        /// The friend.
        /// </summary>
        private UserClass friend;

        private delegate void SendMessageDelegate(Packet arg, Networking net);

        private delegate void UpdateUserInterfaceDelegate(Packet arg);

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatWindow"/> class.
        /// </summary>
        /// <param name="user">
        /// The user to chat with.
        /// </param>
        /// <param name="networking">
        /// The networking.
        /// </param>
        public ChatWindow(UserClass user, ref Networking networking)
        {
            InitializeComponent();
            this.friend = user;
            UserLabel.Content += user.UserName;
            this.net = networking;
        }

        /// <summary>
        /// The message textbox_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MessageTextboxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return;
            }

            this.CallSend();

            //var send = new Task(() => this.net.SendMessage(packet));
            //send.ContinueWith(task => send.Dispose());
            //send.Start();
        }

        /// <summary>
        /// The call send.
        /// </summary>
        private void CallSend()
        {
            if (string.IsNullOrEmpty(this.MessageTextbox.Text))
            {
                return;
            }
            
            var packet = new Packet(this.net.TheUser.UserId, this.friend.UserId, this.MessageTextbox.Text);
            SendMessageDelegate sendMessage = this.SendPacket;
            sendMessage.BeginInvoke(packet, this.net, null, null);
        }

        /// <summary>
        /// The send packet.
        /// </summary>
        /// <param name="packet">
        /// The packet.
        /// </param>
        /// <param name="net">
        /// The net.
        /// </param>
        private void SendPacket(Packet packet, Networking network)
        {
            network.SendMessage(packet);
            this.Dispatcher.BeginInvoke(
                System.Windows.Threading.DispatcherPriority.Normal,
                new UpdateUserInterfaceDelegate(this.UpdateUserInterface),
                packet);
        }

        /// <summary>
        /// The update user interface.
        /// </summary>
        /// <param name="packet">
        /// The packet.
        /// </param>
        private void UpdateUserInterface(Packet packet)
        {
            this.MessageTextbox.Clear();

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(System.Environment.NewLine);
            stringBuilder.Append(
                packet.RecipientId == this.friend.UserId ? this.net.TheUser.UserName : this.friend.UserName);
            stringBuilder.Append(" at ");
            stringBuilder.Append(System.DateTime.Now.ToShortTimeString());
            stringBuilder.Append(": ");
            stringBuilder.Append(packet.Message);

            this.ChatTextbox.AppendText(stringBuilder.ToString());
        }

        /// <summary>
        /// The send button click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SendBtnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            this.CallSend();
        }
    }
}
