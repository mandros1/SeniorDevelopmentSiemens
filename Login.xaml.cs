using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SiemensPerformance
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
            {
                Server = "localhost",
                Database = database.Text,
                UserID = username.Text,
                SslMode = 0,
                Password = password.Password
            };

            MySqlConnection conn = new MySqlConnection(builder.ToString());
            try
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();
                MessageBox.Show("Connection Open!");
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close(); 
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            finally
            {
                conn.Close(); 
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(1);
        }
    }
}
