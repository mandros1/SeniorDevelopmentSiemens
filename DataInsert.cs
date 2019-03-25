using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiemensPerformance
{
    class DataInsert
    {
        MySqlConnection conn;
        MySqlCommand cmd;
        MySqlConnectionStringBuilder builder;


        public DataInsert()
        {
            conn = DBConnect.conn;
        }

        //Working
        public void insertProcess(List<List<string>> process_name)
        {
            conn = DBConnect.conn;
            StringBuilder insertCommand = new StringBuilder("INSERT INTO process(process_name, process_name_id) VALUES ");
            List<string> Rows = new List<string>();

            //Removes the duplicate processes from the list and stores in a new array
            //String[,] new_process_array = process_name.Distinct().ToArray();
            //var dbProcess = new List<string>();

            //Writes out processes
            conn.Open();
            //MySqlCommand get_Process_Name = new MySqlCommand("USE mri; SELECT process_name, process_name_id FROM process", conn);
            //var reader = get_Process_Name.ExecuteReader();
            //while (reader.Read())
            //{
            //var item = reader.GetString(0);
            //var item2 = reader.GetString(1);
            //singleList = new List<string>();
            //singleList.Add(reader.GetString(0));
            //dbProcess.Add(reader.GetString(0));
            //}
            //reader.Close();
            //conn.Close();

            //string[] dbProcess_array = dbProcess.ToArray();
            //var missingProcess = new_process_array.Except(dbProcess_array).ToArray();
            //foreach (var item in missingProcess)
            //{
            //    Rows.Add(string.Format("('{0}')", MySqlHelper.EscapeString(item)));
            //}

            foreach (List<string> subList in process_name)
            {
                foreach (string item in subList)
                {
                    Rows.Add(string.Format("('{0}')", MySqlHelper.EscapeString(item)));
                }
            }

            insertCommand.Append(string.Join(",", Rows));
            insertCommand.Append(";");
            conn.Open();
            using (MySqlCommand myCmd = new MySqlCommand(insertCommand.ToString(), conn))
            {
                try
                {
                    myCmd.CommandType = CommandType.Text;
                    myCmd.ExecuteNonQuery();
                }
                catch(Exception e)
                {

                }
            }

            conn.Close();
        }

        //Still does not work. Method will insert into the mri_data table 
        public void insertMRI_Data(List<string> mri_trace)
        {
            conn = DBConnect.conn;

            //StringBuilder insertCommand = new StringBuilder("INSERT INTO mri_data(WSP,WSPPeak,HC,HCPeak,TC,TCPeak,CPU,CPUPeak,GDIC,GDICPeak,USRC,USRCPeak,PRIV,PRIVPEAK,VIRT,VIRTPeak,PFS,PFSPeak) VALUES ");
            StringBuilder insertCommand = new StringBuilder("INSERT INTO mri_data(WSP) VALUES ");

            List<string> Rows = new List<string>();

            //mri_trace.ForEach(i => Console.Write("{0}\t", i));

            foreach (var item in mri_trace)
            {
                Rows.Add(string.Format("('{0}')", MySqlHelper.EscapeString(item.ToString())));
            }
            insertCommand.Append(string.Join(",", Rows));
            insertCommand.Append(";");
            conn.Open();
            using (MySqlCommand myCmd = new MySqlCommand(insertCommand.ToString(), conn))
            {
                myCmd.CommandType = CommandType.Text;
                myCmd.ExecuteNonQuery();
            }

            conn.Close();
        }
        //Working
        public void insertTime(List<string> time_timestamp)
        {
            conn = DBConnect.conn;
            StringBuilder insertCommand = new StringBuilder("INSERT INTO time(timeStamp) VALUES ");
            List<string> Rows = new List<string>();

            //Removes the duplicate processes from the list and stores in a new array
            String[] new_time_array = time_timestamp.Distinct().ToArray();
            var dbTime = new List<string>();

            //Writes out processes
            conn.Open();
            MySqlCommand get_Process_Name = new MySqlCommand("USE mri;SELECT timeStamp FROM time", conn);
            var reader = get_Process_Name.ExecuteReader();
            while (reader.Read())
            {
                dbTime.Add(reader.GetString(0));
            }
            reader.Close();
            conn.Close();

            string[] dbProcess_array = dbTime.ToArray();
            var missingTime = new_time_array.Except(dbProcess_array).ToArray();
            foreach (var item in missingTime)
            {
                Rows.Add(string.Format("('{0}')", MySqlHelper.EscapeString(item)));
            }
            insertCommand.Append(string.Join(",", Rows));
            insertCommand.Append(";");
            conn.Open();
            using (MySqlCommand myCmd = new MySqlCommand(insertCommand.ToString(), conn))
            {
                try
                {
                    myCmd.CommandType = CommandType.Text;
                    myCmd.ExecuteNonQuery();
                }catch(Exception e) { }
            }

            conn.Close();
        }
    }
}
