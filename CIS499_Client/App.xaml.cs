// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Sam Novak">
//   C# Instant Messenger XAML client
// </copyright>
// <summary>
//   Interaction logic for App
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CIS499_Client
{
    using System.Windows;
    using System.Threading;
    using UserClass;

    /// <summary>
    /// Interaction logic for App
    /// </summary>
    public partial class App
    {
        /// <summary>
        /// The user.
        /// </summary>
        internal static UserClass User = new UserClass();

        /// <summary>
        /// The net.
        /// </summary>
        internal static Networking Net { get; set; }

        

        ///// <summary>
        ///// The fill user.
        ///// </summary>
        ///// <param name="user">
        ///// The user.
        ///// </param>
        //internal static void FillUser(ref UserClass user)
        //{
        //    for (int i = 0; i < 5; i++)
        //    {
        //        UserClass tempUser = new UserClass(i.ToString(), (uint)i, "taco", true);
        //        user.Friends.Add(tempUser);

        //    }

        //}

    }
}
