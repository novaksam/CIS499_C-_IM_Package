// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Sam Novak">
//   C# Instant Messenger XAML client
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CIS499_Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Networking net;

        public MainWindow(Networking networking)
        {
            InitializeComponent();
            //App.FillUser(ref App.User);
            this.net = networking;
            OnlineList.ItemsSource = net.TheUser.Friends.Where(@class => @class.LoggedIn == true);
            OfflineList.ItemsSource = net.TheUser.Friends.Where(@class => @class.LoggedIn == false);
            
        }

        // TODO add in code to update the displays based on if a user is online or not

        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void listBox3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            //listBox3.SelectedItems.Clear();
        }

        private void OnlineList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Application.Current.Shutdown();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var login = new Login();
            login.Show();
            this.Close();
        }
    }
}
