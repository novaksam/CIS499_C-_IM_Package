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
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;

    using UserClass;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Networking net;

        private static List<UserClass> onlineList = new List<UserClass>();

        private static List<UserClass> offlineList = new List<UserClass>();

        System.Threading.Thread onThread = new Thread(o =>
        {
            onlineList = OnlineLinqList(o);
        });
        System.Threading.Thread offThread = new Thread(o =>
        {
            offlineList = OfflineLinqList(o);
        });

        public MainWindow(Networking networking)
        {
            InitializeComponent();
            //App.FillUser(ref App.User);
            this.net = networking;
            // App.TaskFortress.StartNew(this.OnlineLinqList, net);
            //System.Threading.Thread onThread = new Thread(o =>
            //    {
            //        onlineList = this.OnlineLinqList(o);
            //    });
            //System.Threading.Thread offThread = new Thread(o =>
            //{
            //    offlineList = this.OfflineLinqList(o);
            //});
            //var on = net.TheUser.Friends.Where(@class => @class.LoggedIn == true).ToList();
            //var off = net.TheUser.Friends.Where(@class => @class.LoggedIn == false).ToList();
            
            onThread.Start(net);
            offThread.Start(net);
            onThread.Join();
            this.OnlineList.ItemsSource = onlineList;
            offThread.Join();
            this.OfflineList.ItemsSource = offlineList;
            this.LogInText.Text += net.TheUser.UserName;
        }

        private static List<UserClass> OnlineLinqList(object o)
        {
            return ((Networking)o).TheUser.Friends.Where(@class => @class.LoggedIn == true).ToList();
        }

        private static List<UserClass> OfflineLinqList(object o)
        {
            return ((Networking)o).TheUser.Friends.Where(@class => @class.LoggedIn == false).ToList();
        }

        // TODO add in code to update the displays based on if a user is online or not

        private void OnlineList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var listBox = (ListBox)sender;
            if (listBox.SelectedItem != null)
            {
                var friend = listBox.SelectedItem as UserClass;
                ChatWindow chat = new ChatWindow(friend, ref net);
                chat.Show();
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Application.Current.Shutdown();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var login = new Login();
            net.Dispose();
            login.Show();
            this.Close();
        }

        private void BtnAddFriends_Click(object sender, RoutedEventArgs e)
        {
            var addFri = new AddFriends(ref this.net);
            this.Hide();
            addFri.ShowDialog();
            this.refresh();
            this.Show();

        }

        private void OnlineList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void refresh()
        {
        //    this.OnlineList.Items.Refresh();
        //    this.OfflineList.Items.Refresh();
        //    var on = this.net.TheUser.Friends.Where(@class => @class.LoggedIn == true).ToList();
        //    var off = this.net.TheUser.Friends.Where(@class => @class.LoggedIn == false).ToList();
        //    this.OnlineList.ItemsSource = on;
        //    this.OfflineList.ItemsSource = off;
            onThread.Start(net);
            offThread.Start(net);
            onThread.Join();
            OnlineList.Items.Refresh();
            offThread.Join();
            OfflineList.Items.Refresh();
        }

        private void OfflineList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var listBox = (ListBox)sender;
            if (listBox.SelectedItem != null)
            {
                var friend = listBox.SelectedItem as UserClass;
                ChatWindow chat = new ChatWindow(friend, ref net);
                chat.Show();
            }
        }
    }
}
