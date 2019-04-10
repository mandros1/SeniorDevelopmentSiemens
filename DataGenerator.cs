using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using MySql.Data.MySqlClient;
using System.Data;

namespace SiemensPerformance
{
    class DataGenerator
    {
        public MySqlConnection conn = DBConnect.conn;
        public List<string[]> dlist { get; set; }
        public List<string[]> processes2DList { get; set; }
        public List<string[]> gloabalZero2DList { get; set; }
        public List<string[]> globalTotal2DList { get; set; }
        public List<string> processes { get; set; }
        public string[] processesDB { get; set; }
        private List<string> singleList { get; set; }
        public string fileName { get; set; }
        private List<string> processNamesList;
        private List<string[]> processData2DList;
        private List<string> filteredDataList;

        public string[] processVariables = {"TimeStamp", "Process Name", "Process ID", "WSP", "WSPPeak",
            "HC", "HCPeak", "TC", "TCPeak", "CPU", "CPUPeak",
            "GDIC", "GDICPeak", "USRC", "USRCPeak", "PRIV",
            "PRIVPeak", "VIRT", "VIRTPeak", "PFS", "PFSPeak" };

        public string[] globalVariables = { "TimeStamp", "GCPU0", "GCPU0Peak",
            "GCPU1", "GCPU1Peak", "GCPU2", "GCPU2Peak", "GCPU3", "GCPU3Peak", "GCPU4", "GCPU4Peak", "GCPU5", "GCPU5Peak",
            "GCPU6", "GCPU6Peak", "GCPU7", "GCPU7Peak", "GCPU8", "GCPU8Peak",
            "GCPU9", "GCPU9Peak", "GCPU10", "GCPU10Peak", "GCPU11", "GCPU11Peak", "GCPU12", "GCPU12Peak",
            "GCPU13", "GCPU13Peak", "GCPU14", "GCPU14Peak", "GCPU15", "GCPU15Peak"};

        public string[] globalTotalVariables = {"TimeStamp", "GCPU", "GCPUPeak",
            "GMA", "GMAPeak", "GPC", "GPCPeak", "GHC", "GHCPeak",
            "GHPF", "GCPUP", "GCPUPPeak", "GMF", "GMFPeak",
            "GMCOMM", "GMCOMMPeak", "GML", "GMLPeak",
            "GPFC", "GPFCPeak", "GMC", "GMCPeak"};

        public string[] selectProcessNames = { "WSP", "WSPPeak",
            "HC", "HCPeak", "TC", "TCPeak", "CPU", "CPUPeak",
            "GDIC", "GDICPeak", "USRC", "USRCPeak", "PRIV",
            "PRIVPeak", "VIRT", "VIRTPeak", "PFS", "PFSPeak" };

        public string[] selectGlobalZeroNames = {"GCPU0", "GCPU0Peak",
            "GCPU1", "GCPU1Peak", "GCPU2", "GCPU2Peak", "GCPU3", "GCPU3Peak", "GCPU4", "GCPU4Peak", "GCPU5", "GCPU5Peak",
            "GCPU6", "GCPU6Peak", "GCPU7", "GCPU7Peak", "GCPU8", "GCPU8Peak",
            "GCPU9", "GCPU9Peak", "GCPU10", "GCPU10Peak", "GCPU11", "GCPU12Peak",
            "GCPU13", "GCPU13Peak", "GCPU14", "GCPU14Peak", "GCPU15", "GCPU15Peak"};

        public string[] selectGlobalTotalNames = { "GCPU", "GCPUPeak",
            "GMA", "GMAPeak", "GPC", "GPCPeak", "GHC", "GHCPeak",
            "GHPF", "GCPUP", "GCPUPPeak", "GMF", "GMFPeak",
            "GMFPeak", "GMCOMM", "GMCOMMPeak", "GML", "GMLPeak",
            "GPFC", "GPFCPeak", "GMC", "GMCPeak"};


        //Gets all unique process names
        public List<string> getDistinctProcessNames()
        {
            //processNamesList = dlist.Select(list => list[1]).ToList();
            Console.WriteLine(processes2DList[0]);
            processNamesList = processes2DList.Select(list => list[1]).ToList();
            processNamesList.Insert(0, "");
            IEnumerable<string> distinctNotes = processNamesList.Distinct();
            return new List<string>(distinctNotes);
        }

        public List<string> getDistinctProcessIDs(string processName)
        {
            if (String.IsNullOrEmpty(processName))
            {
                return new List<string>();
            }
            List<string[]> filteredList = getProcessData(processName, null);
            processNamesList = filteredList.Select(list => list[2]).ToList();
            processNamesList.Insert(0, "");
            IEnumerable<string> distinctNotes = processNamesList.Distinct();
            return new List<string>(distinctNotes);
        }

        //Gets data for one process
        public List<string[]> getProcessData(string processName, string processId)
        {
            if ( !String.IsNullOrEmpty(processName) && String.IsNullOrEmpty(processId) && processName != "None")
            {
                processData2DList = processes2DList.Where(x => x[1] == processName).ToList();
                Console.WriteLine("Process name is {0}, but it's ID is null\nReturning the list filtered by process name only", processName);
                return processData2DList;
            }
            else if ( !String.IsNullOrEmpty(processName) && !String.IsNullOrEmpty(processId))
            {
                Console.WriteLine("Process name is {0}, process ID is {1}\nReturning the list filtered by process name and ID", processName, processId);
                processData2DList = processes2DList.Where(x => x[1] == processName && x[2] == processId).ToList();
                return processData2DList;
            }
            Console.WriteLine("Both process name and it's ID are null\nReturning the whole list");
            return processes2DList;
        }
        /// <summary>
        /// returns all the data and inserts it to list, problem with Ticks , probably with the way i create List.
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        public List<string[]> getDataFromDb(string processName, string processId) {
            Console.WriteLine(processName);
            List<string[]> all = new List<string[]>();

            try
            {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();

                string sql = "SELECT  *, DATE_FORMAT(TimeStamp, '%Y/%m/%d-%H:%i:%s.%f') AS date FROM mri_data WHERE process_name= '" + processName + "' AND process_Id = '" + processId + "';";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                  
                    string a = "";
                    for (int i = 0; i < rdr.FieldCount; i++)
                    {
                        //Console.WriteLine(i+" "+rdr[i]);
                        if (i == 1)
                        {
                            a += rdr[22].ToString() + ";";
                            //Console.WriteLine(a);
                        }
                        else if (i == 22) {
                            //Console.WriteLine("Didnt write to array "+ rdr[i]); 
                        }
                        else
                        {
                            a += rdr[i].ToString() + ";";
                            //Console.WriteLine(a);
                        }
                        
                        if (i+1 == rdr.FieldCount)
                        {
                            //Console.WriteLine(a);
                            
                            string[] everything = a.Split(';');
                            //Remove first and last element from array
                            everything = everything.Skip(1).ToArray();
                            everything = everything.Take(everything.Count() - 1).ToArray();
                            all.Add(everything);
                        }
        
                    };
                }
                rdr.Close();
 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            conn.Close();

            processData2DList = all.ToList<String[]>(); //all.Where(x => x[2] == processName).ToList();
            Console.WriteLine("Done.");
            return processData2DList;

        }

        public List<string[]> getWhereProcessData(  List<string[]> processDataFilteredNameAndID, 
                                                    string whereColumn, 
                                                    string whereOperator, 
                                                    string whereValue)
        {

            int variableIndex = Array.IndexOf(processVariables, whereColumn);

            // sanitize provided whereValue
            whereValue = Regex.Replace(whereValue, @"[^0-9.]", "");
            Double value;
            try
            {
                value = Double.Parse(whereValue.Replace(".", ","));
                switch (whereOperator)
                {
                    case "==":
                        processData2DList = processDataFilteredNameAndID.Where(x => Double.Parse(x[variableIndex].Replace(".", ",")) == value).ToList();
                        break;
                    case ">":
                        processData2DList = processDataFilteredNameAndID.Where(x => Double.Parse(x[variableIndex].Replace(".", ",")) > value).ToList();
                        break;
                    case ">=":
                        processData2DList = processDataFilteredNameAndID.Where(x => Double.Parse(x[variableIndex].Replace(".", ",")) >= value).ToList();
                        break;
                    case "<":
                        processData2DList = processDataFilteredNameAndID.Where(x => Double.Parse(x[variableIndex].Replace(".", ",")) < value).ToList();
                        break;
                    case "<=":
                        processData2DList = processDataFilteredNameAndID.Where(x => Double.Parse(x[variableIndex].Replace(".", ",")) <= value).ToList();
                        break;
                    case "!=":
                        processData2DList = processDataFilteredNameAndID.Where(x => Double.Parse(x[variableIndex].Replace(".", ",")) != value).ToList();
                        break;
                }
            }
            catch (IndexOutOfRangeException t) { Console.WriteLine(t); }
            catch (Exception e) { Console.WriteLine(e); }
            
            return processData2DList;
        }


        public void getJsonString(OpenFileDialog dialog)
        {
            dlist = new List<string[]>();
            processes2DList = new List<string[]>();
            globalTotal2DList = new List<string[]>();
            gloabalZero2DList = new List<string[]>();
            this.fileName = dialog.SafeFileName;
            string line;
            DataInsert dataInsert = new DataInsert();
            //var time_array = new List<string>();
            //List<List<string>> process_array = new List<List<string>>();

            //StreamWriter sw = new StreamWriter("D:\\WPF_Applications\\SeniorDevelopmentSiemens\\Data.txt");
            System.IO.StreamReader file = new System.IO.StreamReader(dialog.FileName);
            
            try
            {
                file.ReadLine(); // skip the firstLine
                int counter = 1;
                while ((line = file.ReadLine()) != null)
                {
                    singleList = new List<string>();
                    var enumerate = GetSplitData(line);

                    foreach (var item in enumerate)
                    {
                        if( item != "ErrorLine") { 
                            singleList.Add(item);
                            //sw.Write(item+",");
                        }
                    }
                    //sw.Write("\n");
                    //
                    if (singleList.Count == 21)
                    {
                        // for processess
                        processes2DList.Add(singleList.ToArray());
                        //time_array.Add(singleList[0].Split('.')[0]);
                        //process_array.Add(new List<string> { singleList[1], singleList[2] });
                        dlist.Add(singleList.ToArray());
                    } else if (singleList.Count == 33)
                    {
                        // for global 0
                        gloabalZero2DList.Add(singleList.ToArray());
                        //time_array.Add(singleList[0].Split('.')[0]);
                        dlist.Add(singleList.ToArray());
                    }
                    else if (singleList.Count == 22)
                    {
                        // for global total
                        globalTotal2DList.Add(singleList.ToArray());
                        //time_array.Add(singleList[0].Split('.')[0]);
                        dlist.Add(singleList.ToArray());
                    }
                    counter++;
                }
                //dataInsert.insertTime(time_array);
                //dataInsert.insertProcess(process_array);
                dataInsert.insertMRI_Data(processes2DList);
                dataInsert.insertGlobal0(gloabalZero2DList);
                dataInsert.insertGlobalTotal(globalTotal2DList);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                //sw.Close();
                file.Close();
            }

        }

        public IEnumerable<string> GetSplitData(string textData)
        {
            int lastPositionOfWall = textData.LastIndexOf('|');
            string dataType = textData.Substring(lastPositionOfWall+1, 8);
            if(dataType != "Process:" && dataType != "Global: ")
            {
                yield return "ErrorLine";
                yield break;
            }
            yield return textData.Substring(0, 26); //timestamp
            string data = textData.Substring(textData.IndexOf(':', 26) + 2);
            string processName = data.Substring(0, data.IndexOf(":")); // process name 

            int bracketPosition = processName.IndexOf("(");
            string procName = processName.Substring(0, bracketPosition);
            string procID = processName.Substring(bracketPosition+1, (processName.Length - bracketPosition-2));
            
            if(procName != "GCPU") { 
                yield return procName;
                yield return procID;
            }
            data = data.Substring(data.IndexOf(":") + 1);

            int finalLength = data.Length;
            int i = 0; // position after the found ';' 
            int j = data.IndexOf(';', 0, finalLength); // position of the initial ';'
            string line;
            int colonIndex;

            if (j == -1) // No such substring
            {
                yield return "ErrorLine"; // returned by the lines that doesn't have valid data
            } 

            // while ';' is found in the string
            while (j != -1)
            {
                if (j - i > 0) // Non empty? 
                {
                    line = data.Substring(i, j - i);
                    colonIndex = line.IndexOf(':');
                    string subbedLine = line.Substring(colonIndex + 2);
                    yield return Regex.Replace(subbedLine, @"[^0-9.]", "");
                }

                i = j + 1;
                j = data.IndexOf(';', i, finalLength - i);
            }
        }
    }
}
