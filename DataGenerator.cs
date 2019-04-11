using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace SiemensPerformance
{
    class DataGenerator
    {

        // Constants
        public string[] processVariables = {"TimeStamp", "Process Name", "Process ID", "WSP", "WSPPeak",
            "HC", "HCPeak", "TC", "TCPeak", "CPU", "CPUPeak",
            "GDIC", "GDICPeak", "USRC", "USRCPeak", "PRIV",
            "PRIVPeak", "VIRT", "VIRTPeak", "PFS", "PFSPeak" };

        public string[] globalZeroVariables = { "TimeStamp", "GCPU0", "GCPU0Peak",
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
        

        public List<string[]> processes2DList { get; set; }

        public List<string[]> globalZero2DList { get; set; }

        public List<string[]> globalTotal2DList { get; set; }
        
        private List<string> singleList { get; set; }

        public string fileName { get; set; }

        private List<string> processNamesList { get; set; }
        private List<string[]> processData2DList { get; set; }
        private List<DateModel> data { get; set; }
        private IEnumerable<string> distinctNotes { get; set; }
        private List<string[]> filteredList { get; set; }
        private string line { get; set; }
        private System.IO.StreamReader file { get; set; }

        private int variableIndex { get; set; }
        private Double value { get; set; }
        private List<string> processReusableList { get; set; }
        private int counter { get; set; }

        // Reusables for reading the file

        private string dataType;
        private string dataString;
        private string processName;
        private int bracketPosition;
        private string procID;
        private int finalLength;
        private int i;
        private int j;
        private int colonIndex;
        private string subbedLine;
        private int index;


        /*
         * This part is for generating data for PROCESSES
        * ########################################################################################################
        */

        /*
         * Returns the distinct List of all the processNames found in the current process2DList
         */ 
        public List<string> getDistinctProcessNames()
        {
            processNamesList = processes2DList.Select(list => list[1]).ToList();
            distinctNotes = processNamesList.Distinct();
            processReusableList = new List<string>(distinctNotes);
            return processReusableList;
        }

        /*
         * Returns the distinct List of all the processIDs found in the current process2DList for the specific process name
         * @parameter processName is the name of the process for which IDs are filtered
         */
        public List<string> getDistinctProcessIDs(string processName)
        {
            if (String.IsNullOrEmpty(processName))
            {
                return new List<string>();
            }
            filteredList = getProcessData(processName, null);
            processNamesList = filteredList.Select(list => list[2]).ToList();
            distinctNotes = processNamesList.Distinct();
            processReusableList = new List<string>(distinctNotes);
            return processReusableList;
        }

        /*
         * This method accepts processName and processId and generates the 2d list of data for those specific parameters
         */
        public List<string[]> getProcessData(string processName, string processId)
        {
            if (!String.IsNullOrEmpty(processName) && String.IsNullOrEmpty(processId) && processName != "None")
            {
                processData2DList = processes2DList.Where(x => x[1] == processName).ToList();
                Console.WriteLine("Process name is {0}, but it's ID is null\nReturning the list filtered by process name only", processName);
                return processData2DList;
            }

            else if (!String.IsNullOrEmpty(processName) && !String.IsNullOrEmpty(processId))
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

                string sql = "SELECT  *, DATE_FORMAT(time_fk, '%Y/%m/%d-%H:%i:%s.%f') AS date FROM mri_data WHERE process_name= '" + processName + "' AND process_Id = '" + processId + "';";
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

        public List<string[]> getWhereProcessData(List<string[]> processDataFilteredNameAndID,
                                                    string whereColumn,
                                                    string whereOperator,
                                                    string whereValue,
                                                    string[] columnNames)
        {
            variableIndex = Array.IndexOf(columnNames, whereColumn);

            // sanitize provided whereValue
            whereValue = Regex.Replace(whereValue, @"[^0-9.]", "");
            try
            {
                value = Double.Parse(whereValue.Replace(".", ","));
                if (whereOperator == "==") processDataFilteredNameAndID = processDataFilteredNameAndID.Where(x => Double.Parse(x[variableIndex].Replace(".", ",")) == value).ToList();
                else if (whereOperator == ">") processDataFilteredNameAndID = processDataFilteredNameAndID.Where(x => Double.Parse(x[variableIndex].Replace(".", ",")) > value).ToList();
                else if (whereOperator == ">=") processDataFilteredNameAndID = processDataFilteredNameAndID.Where(x => Double.Parse(x[variableIndex].Replace(".", ",")) >= value).ToList();
                else if (whereOperator == "<") processDataFilteredNameAndID = processDataFilteredNameAndID.Where(x => Double.Parse(x[variableIndex].Replace(".", ",")) < value).ToList();
                else if (whereOperator == "<=") processDataFilteredNameAndID = processDataFilteredNameAndID.Where(x => Double.Parse(x[variableIndex].Replace(".", ",")) <= value).ToList();
                else if (whereOperator == "!=") processDataFilteredNameAndID = processDataFilteredNameAndID.Where(x => Double.Parse(x[variableIndex].Replace(".", ",")) != value).ToList(); 
            }
            catch (IndexOutOfRangeException t) { Console.WriteLine(t); }
            catch (Exception e) { Console.WriteLine(e); }

            return processDataFilteredNameAndID;
        }

        public void sortProcessesByTimeStamp()
        {
            if (processes2DList != null) processes2DList = processes2DList.OrderBy(x => x[0]).ToList();
            if (globalZero2DList != null) globalZero2DList = globalZero2DList.OrderBy(x => x[0]).ToList();
            if (globalTotal2DList != null) globalTotal2DList = globalTotal2DList.OrderBy(x => x[0]).ToList();
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
            var time_array = new List<string>();
            List<List<string>> process_array = new List<List<string>>();

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
                        }
                    }
                    if (singleList.Count == 22 && singleList[0] == "Process:")
                    {
                        // for processess
                        processes2DList.Add(singleList.Skip(1).ToArray());
                    } else if (singleList.Count == 34 && singleList[0] == "Global: ")
                    {
                        // for global 0
                        globalZero2DList.Add(singleList.Skip(1).ToArray());
                    } else if (singleList.Count == 23 && singleList[0] == "Global: ")
                    {
                        // for global total
                        globalTotal2DList.Add(singleList.Skip(1).ToArray());
                    }
                    counter++;
                    //double calc = ((double)counter / (double)lineCount);
                    //pbar.Value = Math.Ceiling(calc * 100);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                file.Close();
            }
            this.sortProcessesByTimeStamp();
        }
        

        /*
         * Method that reads the each line from the file and creates an IEnumerable from them
         */
        public IEnumerable<string> GetSplitData(string textData)
        {
            index = textData.LastIndexOf('|');
            dataType = textData.Substring(index+1, 8);
            if(dataType != "Process:" && dataType != "Global: ")
            {
                yield return "ErrorLine";
                yield break;
            }
            yield return dataType;
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
                    line = dataString.Substring(i, j - i);
                    colonIndex = line.IndexOf(':');
                    subbedLine = line.Substring(colonIndex + 2);
                    if (subbedLine.Contains("n.a.")) subbedLine = "0.0";
                    yield return Regex.Replace(subbedLine, @"[^0-9.]", "");
                }

                i = j + 1;
                j = dataString.IndexOf(';', i, finalLength - i);
            }
        }
        
        public List<DateModel> getDateModelList(List<string[]> dataList, string whereColumn, string[] columnNames)
        {
            variableIndex = Array.IndexOf(columnNames, whereColumn);
            data = new List<DateModel>();

            foreach (string[] list in dataList)
            {
                data.Add(new DateModel
                {
                    DateTime = DateTime.ParseExact(list[0], "yyyy/MM/dd-HH:mm:ss.ffffff", null),
                    Value = Double.Parse(list[variableIndex].Replace(".", ","))
                });
            }
            return data;
        }
    }
}
