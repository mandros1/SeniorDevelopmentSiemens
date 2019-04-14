using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace SiemensPerformance
{
    class DataGenerator
    {
        public MySqlConnection conn = DBConnect.conn;

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
        
        
        public List<string[]> processes2DList { get; set; } // Store all process data from the file

        public List<string[]> globalZero2DList { get; set; } // Store all the Global(0) data from the file

        public List<string[]> globalTotal2DList { get; set; } // Store all the Global(_Total) data from the file

        private List<string> singleList { get; set; } // Used to store each read line while reading from the file

        private List<string[]> filterProcessData2DList { get; set; } // list used to store filtered data
        
        Dictionary<string, List<string[]>> multiDimenstionalDictionary { get; set; } // dictionary used to store data for JSON exporting and importing
        
        private List<string> processNamesList { get; set; } // list of all process names
        

        private List<DateModel> data { get; set; } // List used to hold the feed for the graphs
        private IEnumerable<string> distinctNotes { get; set; } // this IEnumerable is used to get store only distinct/not repeating process names
        private List<string[]> filteredList { get; set; } // this list is used for storing all the processnames from the loaded file - not distinct
        private List<string> processReusableList { get; set; } // list containing processNames from a loaded file - distinct
        private System.IO.StreamReader file { get; set; } // file object that will be read through
        public string fileName { get; set; } // name of the loaded utr file

        // Data connected to the graph
        public List<string[]> graphData { get; set; } // list used to store the data that is currently loaded in the Graph
        public string graphColumn { get; set; }
        public string[] graphColumnNames { get; set; }

        // reusable variables
        private string line { get; set; } // used when reading through file
        private int variableIndex { get; set; } // reusable component to get the column number
        private Double value { get; set; } // reusable used for reading numerical values from the utr file
        private int counter { get; set; } // reusable counter used while reading through the file
        private string dataType, subbedLine, dataString, processName, procID;
        private int i, j, colonIndex, index, bracketPosition, finalLength;
        private List<string[]> graphData2, all;
        private MySqlCommand cmd;
        private MySqlDataReader rdr;


        /*
         * Method that creates a JSON string for the result file export
         */
        public string resultFileDataGenerator()
        {
            multiDimenstionalDictionary = new Dictionary<string, List<string[]>>();
            multiDimenstionalDictionary.Add("graphData", graphData);
            string[] container = new string[2] { graphColumn, fileName };
            graphData2 = new List<string[]>();
            graphData2.Add(container);
            graphData2.Add(graphColumnNames);
            multiDimenstionalDictionary.Add("graphColumns", graphData2);
            multiDimenstionalDictionary.Add("Process", processes2DList);
            multiDimenstionalDictionary.Add("Global_Zero", globalZero2DList);
            multiDimenstionalDictionary.Add("Global_Total", globalTotal2DList);

            
            return JsonConvert.SerializeObject(multiDimenstionalDictionary, Formatting.Indented);
        }

        /*
         * Returns the distinct List of all the processNames found in the current process2DList, no repeated data
         * @return list containing distinct string process names 
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
         * @param processName is the name of the process for which IDs are filtered
         * @return list containing distinct string process IDs
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
         * Filtering process data on specific process name and process id and consoling the filter process that was put in place
         * @param processName is a name of the process
         * @param processId is the id of the latter process name
         * @return 2D list with data filtered by process name and process id
         */
        public List<string[]> getProcessData(string processName, string processId)
        {
            if (!String.IsNullOrEmpty(processName) && String.IsNullOrEmpty(processId) && processName != "None")
            {
                filterProcessData2DList = processes2DList.Where(x => x[1] == processName).ToList();
                Console.WriteLine("Process name is {0}, but it's ID is null\nReturning the list filtered by process name only", processName);
                return filterProcessData2DList;
            }
            else if (!String.IsNullOrEmpty(processName) && !String.IsNullOrEmpty(processId))
            {
                Console.WriteLine("Process name is {0}, process ID is {1}\nReturning the list filtered by process name and ID", processName, processId);
                filterProcessData2DList = processes2DList.Where(x => x[1] == processName && x[2] == processId).ToList();
                return filterProcessData2DList;
            }
            Console.WriteLine("Both process name and it's ID are null\nReturning the whole list");
            return processes2DList;
        }

        /// <summary>
        /// POGLEDAJ OVU METODU I METODU ISPOD OVE, STA TREBA TVOJOJ METODI DATEMODELLIST, KAKAV FORMAT PODATAKA
        /// </summary>
        /// <param name="processName"></param>
        /// <param name="processId"></param>
        /// <returns></returns>
        public List<string[]> getProcessDataFromDB(string processName, string processId)
        {
            all = new List<string[]>();
            string sql;

            if (!String.IsNullOrEmpty(processName) && String.IsNullOrEmpty(processId) && processName != "None")
            {
                //Console.WriteLine("Process name is {0}, but it's ID is null\nReturning the list filtered by process name only", processName);
                //sql = "USE mri; SELECT  *, DATE_FORMAT(TimeStamp, '%Y/%m/%d-%H:%i:%s.%f') AS date FROM mri_data WHERE process_name= '" + processName + "';";
                return null;
            }
            else if (!String.IsNullOrEmpty(processName) && !String.IsNullOrEmpty(processId))
            {
                //Console.WriteLine("Process name is {0}, process ID is {1}\nReturning the list filtered by process name and ID", processName, processId);
                sql = "USE mri; SELECT  *, DATE_FORMAT(TimeStamp, '%Y/%m/%d-%H:%i:%s.%f') AS date FROM mri_data WHERE process_name= '" + processName + "' AND process_Id = '" + processId + "';";
            }
            else
            {
                //Console.WriteLine("Both process name and it's ID are null\nReturning the whole list");
                sql = "USE mri; SELECT  *, DATE_FORMAT(TimeStamp, '%Y/%m/%d-%H:%i:%s.%f') AS date FROM mri_data;";
                //return null;
            }


            try
            {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();

                cmd = new MySqlCommand(sql, conn);
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {

                    string a = "";
                    for (int i = 0; i < rdr.FieldCount; i++)
                    {
                        //Console.WriteLine(i+" "+rdr[i]);
                        if (i == 1)
                        {
                            a += rdr[23].ToString() + ";";
                            //Console.WriteLine(a);
                        } else if (i == 2) {

                        }
                        else if (i == 23)
                        {
                            //Console.WriteLine("Didnt write to array "+ rdr[i]); 
                        }
                        else
                        {
                            a += rdr[i].ToString() + ";";
                            //Console.WriteLine(a);
                        }

                        if (i + 1 == rdr.FieldCount)
                        {
                            //Console.WriteLine(a);

                            string[] everything = a.Split(';');
                            //Remove first and last element from array
                            everything = everything.Skip(1).ToArray();
                            //everything = everything.Skip(3).ToArray();
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

            processes2DList = all.ToList<String[]>(); //all.Where(x => x[2] == processName).ToList();
            Console.WriteLine("Done.");
            return processes2DList;

        }
        /// <summary>
        ///  POGLEDAJ OVU METODU, STA TREBA TVOJOJ METODI DATEMODELLIST, KAKAV FORMAT PODATAKA
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<string[]> getDataFromQueryDb(string query)
        {
            Console.WriteLine(query);
            List<string[]> all = new List<string[]>();

            try
            {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();

                string sql = "USE mri; SELECT "+ query+";";
                Console.WriteLine(sql);
                cmd = new MySqlCommand(sql, conn);
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {

                    string a = "";
                    //Console.WriteLine(rdr.FieldCount);
                    for (int i = 0; i < rdr.FieldCount; i++)
                    {

                        if (i == 1)
                        {
                            a += rdr[rdr.FieldCount - 1].ToString() + ";";
                            //Console.WriteLine(a);
                        }
                        else if (i == 2)
                        {

                        } else if(i +1 == rdr.FieldCount){


                        }

                        else
                        {
                            a += rdr[i].ToString() + ";";
                            //Console.WriteLine(a);
                        }

                        if (i + 1 == rdr.FieldCount)
                        {
                            //Console.WriteLine(a);

                            string[] everything = a.Split(';');
                            
                            //Remove first and last element from array
                            everything = everything.Skip(1).ToArray();
                            //everything = everything.Skip(2).ToArray();
                            everything = everything.Take(everything.Count() - 1).ToArray();
                            //Console.WriteLine( everything.Length);
                           
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

            //processes2DList = all.ToList<String[]>(); //all.Where(x => x[2] == processName).ToList();
            Console.WriteLine("Done.");
            return all.ToList<String[]>();
            //return processes2DList;

        }


        /*
         * Method that handles in code query WHERE functionality by filtering the 2D lists depending on the provided params
         * @param processDataFilteredNameAndID 
         */
        public List<string[]> getWhereProcessData(  List<string[]> processDataFilteredNameAndID,
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


        /*
         * Sort all the 2D arrays of data by its respective timestamps
         */
        public void sortProcessesByTimeStamp()
        {
            if (processes2DList != null) processes2DList = processes2DList.OrderBy(x => x[0]).ToList();
            if (globalZero2DList != null) globalZero2DList = globalZero2DList.OrderBy(x => x[0]).ToList();
            if (globalTotal2DList != null) globalTotal2DList = globalTotal2DList.OrderBy(x => x[0]).ToList();
        }
        
        
        public void importResultFile(OpenFileDialog ofd)
        {
            /*
                multiDimenstionalDictionary = new Dictionary<string, List<string[]>>();
                multiDimenstionalDictionary.Add("graphData", graphData);
                string[] container = new string[2] { graphColumn, fileName };
                graphData2 = new List<string[]>();
                graphData2.Add(container);
                graphData2.Add(graphColumnNames);
                multiDimenstionalDictionary.Add("graphColumns", graphData2);
                multiDimenstionalDictionary.Add("Process", processes2DList);
                multiDimenstionalDictionary.Add("Global_Zero", globalZero2DList);
                multiDimenstionalDictionary.Add("Global_Total", globalTotal2DList);
             */

            string[] dictKeys = new string[5] { "graphData", "graphColumns", "Process", "Global_Zero", "Global_Total" };
            this.fileName = ofd.SafeFileName;
            file = new System.IO.StreamReader(ofd.FileName);
            try
            {
                string json = file.ReadToEnd();

                multiDimenstionalDictionary = new Dictionary<string, List<string[]>>();
                multiDimenstionalDictionary = JsonConvert.DeserializeObject<Dictionary<string, List<string[]>>>(json);
                graphData = multiDimenstionalDictionary[dictKeys[0]];
                graphData2 = multiDimenstionalDictionary[dictKeys[1]];
                graphColumn = graphData2[0][0];
                this.fileName = graphData2[0][1];
                Console.WriteLine(this.fileName);
                graphColumnNames = graphData2[1];
                processes2DList = multiDimenstionalDictionary[dictKeys[2]];
                globalZero2DList = multiDimenstionalDictionary[dictKeys[3]];
                globalTotal2DList = multiDimenstionalDictionary[dictKeys[4]];
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                file.Close();
            }
        }


        /*
         * Method that accepts the file selected from the dialog window and reads through it storing them into an appropriate 2DList
         */
        public void getJsonString(OpenFileDialog dialog)
        {
            processes2DList = new List<string[]>();
            globalTotal2DList = new List<string[]>();
            globalZero2DList = new List<string[]>();
            this.fileName = dialog.SafeFileName;
            string line;
            DataInsert dataInsert = new DataInsert();
            /*
            DataInsert dataInsert = new DataInsert();
            var time_array = new List<string>();
            List<List<string>> process_array = new List<List<string>>();
          */
            file = new System.IO.StreamReader(dialog.FileName);
            
            try
            {
                file.ReadLine(); // skip the firstLine
                counter = 1;
                while ((line = file.ReadLine()) != null)
                {
                    singleList = new List<string>();
                    var enumerate = GetSplitData(line);

                    foreach (var item in enumerate)
                    {
                        if (item != "ErrorLine")
                        {
                            singleList.Add(item);
                        }
                    }
                    if (singleList.Count == 22)
                    {
                        // for processess
                        processes2DList.Add(singleList.Skip(1).ToArray());
                    }
                    else if (singleList.Count == 34)
                    {
                        // for global 0
                        globalZero2DList.Add(singleList.Skip(1).ToArray());
                    }
                    else if (singleList.Count == 23)
                    {
                        // for global total
                        globalTotal2DList.Add(singleList.Skip(1).ToArray());
                    }
                    counter++;
                }
                List<String[]> count = getDataFromQueryDb("Count(*), Count(*) from mri_data WHERE FileName = '" + this.fileName.Split('.')[0] + "'");
                //Console.WriteLine(count[0]);
                if (Int32.Parse(count[0][0]) == 0)
                {
                    dataInsert.insertMRI_Data(processes2DList);
                    dataInsert.insertGlobal0(globalZero2DList);
                    dataInsert.insertGlobalTotal(globalTotal2DList);
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

            //// this is used for exporting to JSON
            //multiDimenstionalDictionary = new Dictionary<string, List<string[]>>();
            //multiDimenstionalDictionary.Add("Process", processes2DList);
            //multiDimenstionalDictionary.Add("Global_Zero", globalZero2DList);
            //multiDimenstionalDictionary.Add("Global_Total", globalTotal2DList); 
            //string json = JsonConvert.SerializeObject(multiDimenstionalDictionary, Formatting.Indented);

            //Dictionary<string, List<string[]>>  mdDict = new Dictionary<string, List<string[]>>();
            //mdDict = JsonConvert.DeserializeObject<Dictionary<string, List<string[]>>>(json);
        }

        public void writeDataToDB(OpenFileDialog dialog)
        {
           // dlist = new List<string[]>();
            processes2DList = new List<string[]>();
            globalTotal2DList = new List<string[]>();
            globalZero2DList = new List<string[]>();
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
                        if (item != "ErrorLine")
                        {
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
                        time_array.Add(singleList[0].Split('.')[0]);
                        process_array.Add(new List<string> { singleList[1], singleList[2] });
                       // dlist.Add(singleList.ToArray());
                    }
                    else if (singleList.Count == 33)
                    {
                        // for global 0
                        globalZero2DList.Add(singleList.ToArray());
                        time_array.Add(singleList[0].Split('.')[0]);
                        //dlist.Add(singleList.ToArray());
                    }
                    else if (singleList.Count == 22)
                    {
                        // for global total
                        globalTotal2DList.Add(singleList.ToArray());
                        time_array.Add(singleList[0].Split('.')[0]);
                       // dlist.Add(singleList.ToArray());
                    }
                    counter++;
                }
               
                
                    dataInsert.insertTime(time_array);
                    dataInsert.insertProcess(process_array);
                    //Throws Object reference not set to an instance of an object. 'System.NullReferenceException'
                    dataInsert.insertMRI_Data(processes2DList);
                    dataInsert.insertGlobal0(globalZero2DList);
                    dataInsert.insertGlobalTotal(globalTotal2DList);
                
            }
            catch (NullReferenceException nre)
            {

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
            dataString = textData.Substring(textData.IndexOf(':', 26) + 2);
            processName = dataString.Substring(0, dataString.IndexOf(":")); // process name 
            
            bracketPosition = processName.IndexOf("(");
            procID = processName.Substring(bracketPosition + 1, (processName.Length - bracketPosition - 2));
            processName = processName.Substring(0, bracketPosition);
            
            if (processName != "GCPU") { 
                yield return processName;
                yield return procID;
            }
            dataString = dataString.Substring(dataString.IndexOf(":") + 1);

            finalLength = dataString.Length;
            i = 0; // position after the found ';' 
            j = dataString.IndexOf(';', 0, finalLength); // position of the initial ';'
        

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
            graphData = dataList;
            graphColumn = whereColumn;
            graphColumnNames = columnNames;
            variableIndex = Array.IndexOf(columnNames, whereColumn);
            data = new List<DateModel>();
            foreach (string[] list in dataList)
            {
                data.Add(new DateModel
                {
                    DateTime = DateTime.ParseExact(list[0], "yyyy/MM/dd-HH:mm:ss.ffffff", null),
                    Value = Double.Parse(list[variableIndex])
                });
            }
            return data;
        }
    }
}
