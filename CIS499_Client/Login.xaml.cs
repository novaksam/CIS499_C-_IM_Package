// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Login.xaml.cs" company="Sam Novak">
//   C# Instant Messenger XAML client
// </copyright>
// <summary>
//   Interaction logic for Login.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CIS499_Client
{
    using System;
    using System.Threading;
    using System.Windows;
    using System.Windows.Input;

    using UserClass;

    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login
    {
        public delegate void AsyncMethodCaller();

        public delegate bool AsyncNetworkCaller(Networking net);

        /// <summary>
        /// Initializes a new instance of the <see cref="Login"/> class.
        /// </summary>
        public Login()
        {
            App.TaskFortress.StartNew(Settings.Default.Reload);
            //AsyncMethodCaller async = new AsyncMethodCaller(Settings.Default.Reload);
            
            //IAsyncResult result = async.BeginInvoke(null, null);
            //async.EndInvoke(result);
            //var setLoad = new Task();
            //setLoad.Start();
            InitializeComponent();
        }

        /// <summary>
        /// The button opt click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void BtnOptClick(object sender, RoutedEventArgs e)
        {
            var options = new Options();
            this.Hide();
            options.ShowDialog();
            this.Show();
        }

        /// <summary>
        /// The button register click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void BtnRegClick(object sender, RoutedEventArgs e)
        {
            var register = new RegisterWindow();
            this.Hide();
            register.ShowDialog();
            this.Show();
        }

        /// <summary>
        /// The button quit click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void BtnQuitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
            Application.Current.Shutdown();
        }

        /// <summary>
        /// The button login click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void BtnLoginClick(object sender, RoutedEventArgs e)
        {
            var user = new UserClass(this.TxtUsername.Text, this.TxtPassword.Password, false);
            var result = false;
            var networking = new Networking(user);
            // AsyncNetworkCaller asyncNetwork = LoginThread;
            // asyncNetwork.BeginInvoke(networking, Callback, null);
            //asyncNetwork.Invoke()
            var login = new Thread(() =>
                {
                    result = LoginThread(networking);
                }) { Name = "Login thread" };
            login.Start();
            login.Join();
             
            // Networking networking = new Networking();
            if (!result)
            {
                return;
            }

            var main = new MainWindow(networking);
            main.Show();
            this.Close();
        }

        private void Callback(IAsyncResult ar)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The login.
        /// </summary>
        /// <param name="net">
        /// The net.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool LoginThread(Networking net)
        {
            return net.Login(net.TheUser);
            
        }

        /// <summary>
        /// This handles when the users presses the enter key in the password field
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void PasswordEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.BtnLoginClick(sender, e);
            }
        }
    }
}
