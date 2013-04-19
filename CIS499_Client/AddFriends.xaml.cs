// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddFriends.xaml.cs" company="Sam Novak">
//   C# Instant Messenger XAML client
// </copyright>
// <summary>
//   Interaction logic for AddFriends.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
using System.Windows.Shapes;

namespace CIS499_Client
{
    using System.ComponentModel;

    using UserClass;

    /// <summary>
    /// Interaction logic for AddFriends.xaml
    /// </summary>
    public partial class AddFriends : Window
    {
        /// <summary>
        /// The net.
        /// </summary>
        private Networking net;

        /// <summary>
        /// The list.
        /// </summary>
        private List<UserClass> list;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddFriends"/> class.
        /// </summary>
        /// <param name="networking">
        /// The networking.
        /// </param>
        // TODO pass by ref
        public AddFriends(ref Networking networking)
        {
            InitializeComponent();
            this.net = networking;
        }

        /// <summary>
        /// The button search click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void BtnSearchClick(object sender, RoutedEventArgs e)
        {
            //var search = new Networking.SearchDelegate(net.Search);
            //var async = AsyncOperationManager.CreateOperation(null);
            //var callback = new AsyncCallback(delegate
            //{
            //    ResultsBox.ItemsSource = list;
            //    ResultsBox.Items.Refresh();
            //});

            //var temp = search.BeginInvoke(TxtUserName.Text, callback, async);
            //list = search.EndInvoke(temp);

            list = this.net.Search(TxtUserName.Text);
            ResultsBox.ItemsSource = list;
            ResultsBox.Items.Refresh();
        }

        /// <summary>
        /// The btn add click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void BtnAddClick(object sender, RoutedEventArgs e)
        {
            List<UserClass> users = new List<UserClass>();
            users.Add((UserClass)this.ResultsBox.SelectedItem);
            this.net.AddFriend(this.net.TheUser, users);
            this.DialogResult = true;
        }

        /// <summary>
        /// The btn cancel_ click.
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
