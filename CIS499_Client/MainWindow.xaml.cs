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
        public MainWindow()
        {
            InitializeComponent();
            App.FillUser(ref App.User);
            OnlineList.ItemsSource = App.User.Friends.Where(@class => @class.LoggedIn == true);
            OfflineList.ItemsSource = App.User.Friends.Where(@class => @class.LoggedIn == false);
            
        }

        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
