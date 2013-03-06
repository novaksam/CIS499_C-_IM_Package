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
    using System.Windows.Shapes;

    using UserClass;

    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var user = new UserClass(this.textBox1.Text, this.passwordBox1.Password, false);
            Networking networking = new Networking(user);
            
            //Networking networking = new Networking();
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnOpt_Click(object sender, RoutedEventArgs e)
        {
            Options options = new Options();
            options.ShowDialog();
        }

        private void BtnReg_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow register = new RegisterWindow();
            register.Show();
            this.Hide();

        }
    }
}
