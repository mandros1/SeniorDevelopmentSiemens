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
        DBConnect connect;
        public Login()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string db = database.Text;
            string userID = username.Text;
            String pwd = password.Password;
            connect = new DBConnect(db,userID, pwd);
            //connect.Connect();
            connect.createDatabase();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(1);
        }
    }
}
