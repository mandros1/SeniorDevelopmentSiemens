using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SiemensPerformance
{
    class DBConnect
    {
        public string server { get; set; }
        public string database { get; set; }
        public string uid { get; set; }
        public string password { get; set; }
        MySqlConnectionStringBuilder builder;
        MySqlConnection conn;
        MySqlCommand cmd;
        String query; 

        public DBConnect(string db, string username, string pwd)
        {
            server = "localhost";
            database = db;
            uid = username;
            password = pwd;

            builder = new MySqlConnectionStringBuilder
            {
                Server = server,
                Database = database,
                UserID = uid,
                SslMode = 0,
                Password = password
            };

            conn = new MySqlConnection(builder.ToString());
        }
        public void Connect()
        {
            try
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();
                MessageBox.Show("Connection Open!");
                var loginWindow = (Application.Current.MainWindow as Login);
                if (loginWindow != null)
                {
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    loginWindow.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            finally
            {
                conn.Close();
            }

        }

        public void dBExists()
        {

        }

        public void createDatabase()
        {
            try
            {
                conn.Open();
                query = "CREATE SCHEMA IF NOT EXISTS `mri`;";
                cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            finally
            {
                conn.Close();
            }
        }
    }
}
