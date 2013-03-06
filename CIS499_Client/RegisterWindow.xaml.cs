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
            InitializeComponent();
        }

        private void btnReg_Click(object sender, RoutedEventArgs e)
        {
            UserClass user = new UserClass(txtUsername.Text, txtPassword.Password, false);
            Networking net = new Networking(user);
            net.Register(user);
        }
    }
}
