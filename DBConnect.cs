using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
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
        ServiceController controller;
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
        public void startMySQL()
        {
            ServiceController[] scServices;
            scServices = ServiceController.GetServices();

            foreach (ServiceController scTemp in scServices)
            {
                if (scTemp.ServiceName.StartsWith("MySQL"))
                {
                    controller = new ServiceController(scTemp.ServiceName);
                    if (controller.Status == ServiceControllerStatus.Running)
                        MessageBox.Show("Service is already running");
                    if (controller.Status == ServiceControllerStatus.Stopped)
                        controller.Start();
                }
            }
        }
    }
}
