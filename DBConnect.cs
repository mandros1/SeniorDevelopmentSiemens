using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
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
        public static MySqlConnection conn;
        MySqlCommand cmd;
        ServiceController controller;

        public DBConnect()
        {
            StartMySQL();
        }
            
        public DBConnect(string db, string username, string pwd)
        {
            server = "localhost";
            uid = username;
            password = pwd;
            builder = new MySqlConnectionStringBuilder
            {
                Server = server,
                UserID = uid,
                SslMode = 0,
                Password = password,
                AllowPublicKeyRetrieval = true
            };

            conn = new MySqlConnection(builder.ToString());
            try
            {
                StartMySQL();
            }

            finally
            {
                Connect();
            }
        }
        //Connects to the Server.
        public void Connect()
        {
            try
            {
                //If the mri database doesn't exist, the database will be created by
                //the createDatabase function.
                bool mriExists = CheckDatabaseExists();
                if (mriExists == false)
                {
                    createDatabase();
                }
                //Shows the Main Window which will allow the user to to analze the
                //trace files
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
        //Starts the MYSQL Service
        public void StartMySQL()
        {
            ServiceController[] scServices;
            scServices = ServiceController.GetServices();
            try
            {
                foreach (ServiceController scTemp in scServices)
                {
                    //By default the MySQL process begins with MySQL
                    //The numbers after MySQL change depending on the version
                    //of MySQL. 
                    if (scTemp.ServiceName.StartsWith("MySQL"))
                    {
                        controller = new ServiceController(scTemp.ServiceName);
                        //Starts the MySQL Proceess
                        if (controller.Status == ServiceControllerStatus.Stopped)
                            controller.Start();
                    }
                }
            }
            finally {}
        }
        //Checks if the mri database exists
        public bool CheckDatabaseExists()
        {
            //Gets the count of how many tables are within the mri database 
            string sqlCreateDBQuery = string.Format("SELECT COUNT(*) FROM information_schema.tables WHERE TABLE_SCHEMA = '{0}'", "mri");
            try
            {
                MySqlDataReader reader;
                cmd = new MySqlCommand(sqlCreateDBQuery, conn);
                conn.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int count = reader.GetInt32(0);
                    if (count == 0 )
                    {
                        return false;
                    }

                    else if (count >= 1)
                    {
                        return true;
                    }
                }

                return false;
            }
            finally
            {

                conn.Close();
            }
        }

        public void createDatabase()
        {
            //Runs the sql script to create the database
            MySqlScript script = new MySqlScript(conn, File.ReadAllText("../../createDB.sql"));
            script.Delimiter = "$$";
            script.Execute();
        }

        public void openConnection()
        {
            builder = new MySqlConnectionStringBuilder
            {
                Server = server,
                UserID = uid,
                SslMode = 0,
                Password = password
            };
            conn = new MySqlConnection(builder.ToString());
        }
        
        public void closeConnection()
        {
            conn.Close();
        }
    }
}