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
        string fileName;
        public DataInsert()
        {
            conn = DBConnect.conn;
        }

        public void insertProcess(List<List<string>> process_name)
        {
            fileName = DataDisplayTab.utfFileName.Split('.')[0];
            conn = DBConnect.conn;
            StringBuilder insertCommand = new StringBuilder("USE mri; INSERT INTO process(process_name, process_name_id) VALUES ");
            List<string> Rows = new List<string>();
            var dbProcess = new List<string>();

            //Removes the duplicate process name and ids
            DataTable processTable = new DataTable();
            processTable.Columns.Add("Process Name");
            processTable.Columns.Add("Process Id");

            foreach (var process in process_name)
            {
                DataRow newRow = processTable.NewRow();
                newRow["Process Name"] = process[0];
                newRow["Process Id"] = process[1];
                processTable.Rows.Add(newRow);
            }

            String get_Process_Name = "USE mri; SELECT process_name, process_name_id FROM process";
            MySqlDataAdapter sda = new MySqlDataAdapter(get_Process_Name, conn);

            DataTable dbProcessTable = new DataTable();
            try
            {
                conn.Open();
                sda.Fill(dbProcessTable);
            }
            catch (MySqlException se)
            {
            }

            finally
            {
                conn.Close();
            }

            //Prints out the size of the process data table
            DataTable uniqueProcessTable = processTable.DefaultView.ToTable(true, "Process Name", "Process Id");
            Console.WriteLine("Unique Count: " + uniqueProcessTable.Rows.Count);

            var diff = uniqueProcessTable.AsEnumerable().Except(dbProcessTable.AsEnumerable(), DataRowComparer.Default);
            DataTable differenceTable = diff.CopyToDataTable<DataRow>();
            Console.WriteLine("Difference Count: " + differenceTable.Rows.Count);

            //Prints all of the values in the process data table
            foreach (DataRow dataRow in differenceTable.Rows)
            {
                Rows.Add(string.Format("('{0}','{1}')", MySqlHelper.EscapeString(dataRow[0].ToString()), MySqlHelper.EscapeString(dataRow[1].ToString())));
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
                catch (Exception e)
                {

                }
            }

            conn.Close();
        }

        public void insertTime(List<string> time_timestamp)
        {
            conn = DBConnect.conn;
            fileName = DataDisplayTab.utfFileName.Split('.')[0];
            StringBuilder insertCommand = new StringBuilder("INSERT INTO time(timeStamp) VALUES ");
            List<string> Rows = new List<string>();

            String[] new_time_array = time_timestamp.Distinct().ToArray();
            var dbTime = new List<string>();

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
                }
                catch (Exception e) { }
            }

            conn.Close();
        }
        /*Working with Filenames*/
        public void insertGlobal0(List<string[]> global0_data)
        {
            conn = DBConnect.conn;
            fileName = DataDisplayTab.utfFileName.Split('.')[0];
            StringBuilder insertCommand = new StringBuilder("USE mri; INSERT INTO global0(TimeStamp, FileName, GCPU0, GCPU0Peak,GCPU1, GCPU1Peak, GCPU2, GCPU2Peak, GCPU3, GCPU3Peak, GCPU4, GCPU4Peak, GCPU5, GCPU5Peak, GCPU6, GCPU6Peak, GCPU7, GCPU7Peak, GCPU8, GCPU8Peak, GCPU9, GCPU9Peak, GCPU10, GCPU10Peak, GCPU11, GCPU11Peak, GCPU12, GCPU12Peak, GCPU13, GCPU13Peak, GCPU14, GCPU14Peak, GCPU15, GCPU15Peak) VALUES ");
            List<string> Rows = new List<string>();
            DataTable global0Table = new DataTable();

            global0Table.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("TimeStamp"),new DataColumn("FileName"),
                new DataColumn("GCPU0"), new DataColumn("GCPU0Peak"),
                new DataColumn("GCPU1"), new DataColumn("GCPU1Peak"),
                new DataColumn("GCPU2"), new DataColumn("GCPU2Peak"),
                new DataColumn("GCPU3"), new DataColumn("GCPU3Peak"),
                new DataColumn("GCPU4"), new DataColumn("GCPU4Peak"),
                new DataColumn("GCPU5"), new DataColumn("GCPU5Peak"),
                new DataColumn("GCPU6"), new DataColumn("GCPU6Peak"),
                new DataColumn("GCPU7"), new DataColumn("GCPU7Peak"),
                new DataColumn("GCPU8"), new DataColumn("GCPU8Peak"),
                new DataColumn("GCPU9"), new DataColumn("GCPU9Peak"),
                new DataColumn("GCPU10"), new DataColumn("GCPU10Peak"),
                new DataColumn("GCPU11"), new DataColumn("GCPU11Peak"),
                new DataColumn("GCPU12"), new DataColumn("GCPU12Peak"),
                new DataColumn("GCPU13"), new DataColumn("GCPU13Peak"),
                new DataColumn("GCPU14"), new DataColumn("GCPU14Peak"),
                new DataColumn("GCPU15"), new DataColumn("GCPU15Peak"),
             });

            foreach (var line in global0_data)
            {
                DataRow newRow = global0Table.NewRow();
                newRow["TimeStamp"] = line[0]; newRow["FileName"] = fileName;
                newRow["GCPU0"] = line[1]; newRow["GCPU0Peak"] = line[2];
                newRow["GCPU1"] = line[3]; newRow["GCPU1Peak"] = line[4];
                newRow["GCPU2"] = line[5]; newRow["GCPU2Peak"] = line[6];
                newRow["GCPU3"] = line[7]; newRow["GCPU3Peak"] = line[8];
                newRow["GCPU4"] = line[9]; newRow["GCPU4Peak"] = line[10];
                newRow["GCPU5"] = line[11]; newRow["GCPU5Peak"] = line[12];
                newRow["GCPU6"] = line[13]; newRow["GCPU6Peak"] = line[14];
                newRow["GCPU7"] = line[15]; newRow["GCPU7Peak"] = line[16];
                newRow["GCPU8"] = line[17]; newRow["GCPU8Peak"] = line[18];
                newRow["GCPU9"] = line[19]; newRow["GCPU9Peak"] = line[20];
                newRow["GCPU10"] = line[21]; newRow["GCPU10Peak"] = line[22];
                newRow["GCPU11"] = line[23]; newRow["GCPU11Peak"] = line[24];
                newRow["GCPU12"] = line[25]; newRow["GCPU12Peak"] = line[26];
                newRow["GCPU13"] = line[27]; newRow["GCPU13Peak"] = line[28];
                newRow["GCPU14"] = line[29]; newRow["GCPU14Peak"] = line[30];
                newRow["GCPU15"] = line[31]; newRow["GCPU15Peak"] = line[32];
                global0Table.Rows.Add(newRow);
            }

            foreach (DataRow dataRow in global0Table.Rows)
            {
                Rows.Add(string.Format("('{0}','{1}',{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33})",

                MySqlHelper.EscapeString(dataRow[0].ToString()), MySqlHelper.EscapeString(dataRow[1].ToString()),
                MySqlHelper.EscapeString(dataRow[2].ToString()), MySqlHelper.EscapeString(dataRow[3].ToString()),
                MySqlHelper.EscapeString(dataRow[4].ToString()), MySqlHelper.EscapeString(dataRow[5].ToString()),
                MySqlHelper.EscapeString(dataRow[6].ToString()), MySqlHelper.EscapeString(dataRow[7].ToString()),
                MySqlHelper.EscapeString(dataRow[8].ToString()), MySqlHelper.EscapeString(dataRow[9].ToString()),
                MySqlHelper.EscapeString(dataRow[10].ToString()), MySqlHelper.EscapeString(dataRow[11].ToString()),
                MySqlHelper.EscapeString(dataRow[12].ToString()), MySqlHelper.EscapeString(dataRow[13].ToString()),
                MySqlHelper.EscapeString(dataRow[14].ToString()), MySqlHelper.EscapeString(dataRow[15].ToString()),
                MySqlHelper.EscapeString(dataRow[16].ToString()), MySqlHelper.EscapeString(dataRow[17].ToString()),
                MySqlHelper.EscapeString(dataRow[18].ToString()), MySqlHelper.EscapeString(dataRow[19].ToString()),
                MySqlHelper.EscapeString(dataRow[20].ToString()), MySqlHelper.EscapeString(dataRow[21].ToString()),
                MySqlHelper.EscapeString(dataRow[22].ToString()), MySqlHelper.EscapeString(dataRow[23].ToString()),
                MySqlHelper.EscapeString(dataRow[24].ToString()), MySqlHelper.EscapeString(dataRow[25].ToString()),
                MySqlHelper.EscapeString(dataRow[26].ToString()), MySqlHelper.EscapeString(dataRow[27].ToString()),
                MySqlHelper.EscapeString(dataRow[28].ToString()), MySqlHelper.EscapeString(dataRow[29].ToString()),
                MySqlHelper.EscapeString(dataRow[30].ToString()), MySqlHelper.EscapeString(dataRow[31].ToString()),
                MySqlHelper.EscapeString(dataRow[32].ToString()), MySqlHelper.EscapeString(dataRow[33].ToString())));
            }
            insertCommand.Append(string.Join(",", Rows));
            insertCommand.Append(";");
            insertCommand.Replace("..", "0");
            conn.Open();
            using (MySqlCommand myCmd = new MySqlCommand(insertCommand.ToString(), conn))
            {
                myCmd.CommandType = CommandType.Text;
                myCmd.ExecuteNonQuery();
            }

            conn.Close();
        }
        //Working with Filenames                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      */
        public void insertMRI_Data(List<string[]> mri_data)
        {
            conn = DBConnect.conn;
            //fileName = DataDisplayTab.utfFileName.Split('.')[0];
            StringBuilder insertCommand = new StringBuilder("USE mri; SET GLOBAL max_allowed_packet=1024*1024*1024; INSERT INTO mri_data(TimeStamp, FileName, Process_Name, Process_Id,WSP,WSPPeak,HC,HCPeak,TC,TCPeak,CPU,CPUPeak,GDIC,GDICPeak,USRC,USRCPeak,PRIV,PRIVPeak,VIRT,VIRTPeak,PFS,PFSPeak) VALUES ");
            List<string> Rows = new List<string>();
            DataTable globalTotalTable = new DataTable();

            globalTotalTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("TimeStamp"),new DataColumn("FileName"),new DataColumn("Process_Name"), new DataColumn("Process_Id"),
                new DataColumn("WSP"), new DataColumn("WSPPeak"),new DataColumn("HC"), new DataColumn("HCPeak"),
                new DataColumn("TC"), new DataColumn("TCPeak"),new DataColumn("CPU"), new DataColumn("CPUPeak"),
                new DataColumn("GDIC"), new DataColumn("GDICPeak"),new DataColumn("USRC"), new DataColumn("USRCPeak"),
                new DataColumn("PRIV"), new DataColumn("PRIVPeak"),new DataColumn("VIRT"), new DataColumn("VIRTPeak"),
                new DataColumn("PFS"), new DataColumn("PFSPeak"),
            });

            foreach (var line in mri_data)
            {
                DataRow newRow = globalTotalTable.NewRow();
                newRow["TimeStamp"] = line[0]; newRow["FileName"] = fileName;
                newRow["Process_Name"] = line[1]; newRow["Process_Id"] = line[2];
                newRow["WSP"] = line[3]; newRow["WSPPeak"] = line[4];
                newRow["HC"] = line[5]; newRow["HCPeak"] = line[6];
                newRow["TC"] = line[7]; newRow["TCPeak"] = line[8];
                newRow["CPU"] = line[9]; newRow["CPUPeak"] = line[10];
                newRow["GDIC"] = line[11]; newRow["GDICPeak"] = line[12];
                newRow["USRC"] = line[13]; newRow["USRCPeak"] = line[14];
                newRow["PRIV"] = line[15]; newRow["PRIVPeak"] = line[16];
                newRow["VIRT"] = line[17]; newRow["VIRTPeak"] = line[18];
                newRow["PFS"] = line[19]; newRow["PFSPeak"] = line[20];
                globalTotalTable.Rows.Add(newRow);
            }

            foreach (DataRow dataRow in globalTotalTable.Rows)
            {
                Rows.Add(string.Format("('{0}','{1}','{2}',{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21})",

                MySqlHelper.EscapeString(dataRow[0].ToString()), MySqlHelper.EscapeString(dataRow[1].ToString()),
                MySqlHelper.EscapeString(dataRow[2].ToString()), MySqlHelper.EscapeString(dataRow[3].ToString()),
                MySqlHelper.EscapeString(dataRow[4].ToString()), MySqlHelper.EscapeString(dataRow[5].ToString()),
                MySqlHelper.EscapeString(dataRow[6].ToString()), MySqlHelper.EscapeString(dataRow[7].ToString()),
                MySqlHelper.EscapeString(dataRow[8].ToString()), MySqlHelper.EscapeString(dataRow[9].ToString()),
                MySqlHelper.EscapeString(dataRow[10].ToString()), MySqlHelper.EscapeString(dataRow[11].ToString()),
                MySqlHelper.EscapeString(dataRow[12].ToString()), MySqlHelper.EscapeString(dataRow[13].ToString()),
                MySqlHelper.EscapeString(dataRow[14].ToString()), MySqlHelper.EscapeString(dataRow[15].ToString()),
                MySqlHelper.EscapeString(dataRow[16].ToString()), MySqlHelper.EscapeString(dataRow[17].ToString()),
                MySqlHelper.EscapeString(dataRow[18].ToString()), MySqlHelper.EscapeString(dataRow[19].ToString()),
                MySqlHelper.EscapeString(dataRow[20].ToString()), MySqlHelper.EscapeString(dataRow[21].ToString())));
            }
            insertCommand.Append(string.Join(",", Rows));
            insertCommand.Append(";");
            insertCommand.Replace("..", "0");
            conn.Open();
            using (MySqlCommand myCmd = new MySqlCommand(insertCommand.ToString(), conn))
            {
                myCmd.CommandType = CommandType.Text;
                myCmd.ExecuteNonQuery();
            }

            conn.Close();
        }

        public void insertGlobalTotal(List<string[]> globalTotal_data)
        {
            conn = DBConnect.conn;
            fileName = DataDisplayTab.utfFileName.Split('.')[0];
            StringBuilder insertCommand = new StringBuilder("USE mri; INSERT INTO globaltotal(TimeStamp,FileName,GCPU, GCPUPeak, GMA, GMAPeak, GPC, GPCPeak, GHC, GHCPeak, GHPF, GCPUP, GCPUPPeak, GMF, GMFPeak, GMCOMM, GMCOMMPeak, GML, GMLPeak, GPFC, GPFCPeak, GMC, GMCPeak) VALUES ");
            List<string> Rows = new List<string>();
            DataTable globalTotalTable = new DataTable();

            globalTotalTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("TimeStamp"),new DataColumn("FileName"),
                new DataColumn("GCPU"), new DataColumn("GCPUPeak"),
                new DataColumn("GMA"), new DataColumn("GMAPeak"),
                new DataColumn("GPC"), new DataColumn("GPCPeak"),
                new DataColumn("GHC"), new DataColumn("GHCPeak"),
                new DataColumn("GHPF"),
                new DataColumn("GCPUP"), new DataColumn("GCPUPPeak"),
                new DataColumn("GMF"), new DataColumn("GMFPeak"),
                new DataColumn("GMCOMM"), new DataColumn("GMCOMMPeak"),
                new DataColumn("GML"), new DataColumn("GMLPeak"),
                new DataColumn("GPFC"), new DataColumn("GPFCPeak"),
                new DataColumn("GMC"), new DataColumn("GMCPeak"),
             });

            foreach (var line in globalTotal_data)
            {
                //Console.WriteLine(line);
                DataRow newRow = globalTotalTable.NewRow();
                newRow["TimeStamp"] = line[0]; newRow["FileName"] = fileName;
                newRow["GCPU"] = line[1]; newRow["GCPUPeak"] = line[2];
                newRow["GMA"] = line[3]; newRow["GMAPeak"] = line[4];
                newRow["GPC"] = line[5]; newRow["GPCPeak"] = line[6];
                newRow["GHC"] = line[7]; newRow["GHCPeak"] = line[8];
                newRow["GHPF"] = line[9];
                newRow["GCPUP"] = line[10]; newRow["GCPUPPeak"] = line[11];
                newRow["GMF"] = line[12]; newRow["GMFPeak"] = line[13];
                newRow["GMCOMM"] = line[14]; newRow["GMCOMMPeak"] = line[15];
                newRow["GML"] = line[16]; newRow["GMLPeak"] = line[17];
                newRow["GPFC"] = line[18]; newRow["GPFCPeak"] = line[19];
                newRow["GMC"] = line[20]; newRow["GMCPeak"] = line[21];
                globalTotalTable.Rows.Add(newRow);
            }

            foreach (DataRow dataRow in globalTotalTable.Rows)
            {
                Rows.Add(string.Format("('{0}','{1}',{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22})",

                MySqlHelper.EscapeString(dataRow[0].ToString()), MySqlHelper.EscapeString(dataRow[1].ToString()),
                MySqlHelper.EscapeString(dataRow[2].ToString()), MySqlHelper.EscapeString(dataRow[3].ToString()),
                MySqlHelper.EscapeString(dataRow[4].ToString()), MySqlHelper.EscapeString(dataRow[5].ToString()),
                MySqlHelper.EscapeString(dataRow[6].ToString()), MySqlHelper.EscapeString(dataRow[7].ToString()),
                MySqlHelper.EscapeString(dataRow[8].ToString()), MySqlHelper.EscapeString(dataRow[9].ToString()),
                MySqlHelper.EscapeString(dataRow[10].ToString()), MySqlHelper.EscapeString(dataRow[11].ToString()),
                MySqlHelper.EscapeString(dataRow[12].ToString()), MySqlHelper.EscapeString(dataRow[13].ToString()),
                MySqlHelper.EscapeString(dataRow[14].ToString()), MySqlHelper.EscapeString(dataRow[15].ToString()),
                MySqlHelper.EscapeString(dataRow[16].ToString()), MySqlHelper.EscapeString(dataRow[17].ToString()),
                MySqlHelper.EscapeString(dataRow[18].ToString()), MySqlHelper.EscapeString(dataRow[19].ToString()),
                MySqlHelper.EscapeString(dataRow[20].ToString()), MySqlHelper.EscapeString(dataRow[21].ToString()),
                MySqlHelper.EscapeString(dataRow[22].ToString())
                ));
            }
            insertCommand.Append(string.Join(",", Rows));
            insertCommand.Append(";");
            insertCommand.Replace("..", "0");
            conn.Open();
            using (MySqlCommand myCmd = new MySqlCommand(insertCommand.ToString(), conn))
            {
                myCmd.CommandType = CommandType.Text;
                myCmd.ExecuteNonQuery();
            }

            conn.Close();
        }

        public void insertQuery(List<string> query)
        {

            conn = DBConnect.conn;
            conn.Open();
            MySqlCommand comm = conn.CreateCommand();
            comm.CommandText = "INSERT INTO trace_queries (name,parameters) VALUES(?name,?parameters)";
            comm.Parameters.AddWithValue("?name", query[0]);
            comm.Parameters.AddWithValue("?parameters", query[1]);
            comm.ExecuteNonQuery();
            conn.Close();
        }

    }
}