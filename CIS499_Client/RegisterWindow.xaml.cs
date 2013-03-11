// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegisterWindow.xaml.cs" company="Sam Novak">
//   C# Instant Messenger XAML client
// </copyright>
// <summary>
//   Interaction logic for RegisterWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CIS499_Client
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Windows;

    using UserClass;

    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    // ReSharper disable RedundantExtendsListEntry
    public partial class RegisterWindow : Window
    // ReSharper restore RedundantExtendsListEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterWindow"/> class.
        /// </summary>
        public RegisterWindow()
        {
            //var setLoad = new Task(Settings.Default.Reload);
            //setLoad.Start();
            InitializeComponent();
        }

        /// <summary>
        /// The btn reg_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void BtnRegClick(object sender, RoutedEventArgs e)
        {

            if (this.TxtUsername.Text == string.Empty)
            {
                MessageBox.Show("Username cannot be empty.");
            }
            else if (this.TxtPassword.Password == string.Empty)
            {
                MessageBox.Show("Password cannot be empty.");
            }
            else
            {
                var result = false;

                var user = new UserClass(this.TxtUsername.Text, this.TxtPassword.Password, false);
                var reg = new Thread(() =>
                {
                    result = Reg(user);
                });
                reg.Name = "Register thread";
                reg.Priority = ThreadPriority.BelowNormal;
                reg.Start();
                reg.Join();
                switch (result)
                {
                    case false:
                        MessageBox.Show("Unfortunately the registration was not successful, please try again.");
                        break;
                    case true:
                        MessageBox.Show("Registration was successful! You will now be returned to the main screen where you can log in.");
                        this.DialogResult = true;
                        break;
                }
            }

        }

        /// <summary>
        /// Method to register the user. For use by threading to prevent UI thread blocking.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <returns>
        /// Returns whether the registration was successful.
        /// </returns>
        private static bool Reg(UserClass user)
        {
            
            var net = new Networking(user);
            var result = net.Register(user);
            net.Dispose();
            return result;
        }
    }
}
