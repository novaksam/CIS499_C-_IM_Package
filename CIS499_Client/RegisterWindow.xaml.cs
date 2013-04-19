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
    using System.Threading.Tasks;
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
        /// The running.
        /// </summary>
        private bool running = true;

        /// <summary>
        /// The user.
        /// </summary>
        private UserClass user;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterWindow"/> class.
        /// </summary>
        public RegisterWindow()
        {
            InitializeComponent();
            progressBar1.Maximum = 10;
        }

        /// <summary>
        /// The button reg_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void BtnRegClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.TxtUsername.Text))
            {
                MessageBox.Show("Username cannot be empty.");
            }
            else if (string.IsNullOrEmpty(this.TxtPassword.Password))
            {
                MessageBox.Show("Password cannot be empty.");
            }
            else
            {
                user = new UserClass(this.TxtUsername.Text, this.TxtPassword.Password, false);
                var regTask = new Task<bool>(Reg); 
                regTask.Start();
                var progress = 0;
                while (running)
                {
                    progressBar1.Value = progress;
                    ++progress;
                    if (progress > 10)
                    {
                        progress = 0;
                    }
                }

                var result = regTask.Result;

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
        /// <returns>
        /// Returns whether the registration was successful.
        /// </returns>
        private bool Reg()
        {
            var net = new Networking(this.user);
            var result = false;
            try
            {
                result = net.Register(user);
            }
            catch
            {
            }
            finally
            {
                net.CloseNetwork();
            }

            running = false;
            return result;
        }

        /// <summary>
        /// The button cancel click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void BtnCancelClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
