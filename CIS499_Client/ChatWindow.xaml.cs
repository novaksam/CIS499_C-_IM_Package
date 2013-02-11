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
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for ChatWindow
    /// </summary>
    public partial class ChatWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChatWindow"/> class.
        /// </summary>
        /// <param name="user">
        /// The user to chat with.
        /// </param>
        public ChatWindow(UserClass user)
        {
            InitializeComponent();
            UserLabel.Content += user.UserName;
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
            if (e.Key == Key.Enter)
            {
                // TODO call the send method
            }
        }
    }
}
