using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace SiemensPerformance
{
    class DataGenerator
    {

        public List<string[]> dlist { get; set; }
        public List<string> processes { get; set; }
        private List<string> singleList { get; set; }
        public string fileName { get; set; }
        private List<string> processNamesList;
        private List<string[]> processData2DList;


        public string[] processVariables = {"TimeStamp", "Process Name", "Process ID", "WSP", "WSPPeak",
            "HC", "HCPeak", "TC", "TCPeak", "CPU", "CPUPeak",
            "GDIC", "GDICPeak", "USRC", "USRCPeak", "PRIV",
            "PRIVPeak", "VIRT", "VIRTPeak", "PFS", "PFSPeak" };

        public string[] globalVariables = { "TimeStamp", "GCPU0", "GCPU0Peak",
            "GCPU1", "GCPU1Peak", "GCPU2", "GCPU2Peak", "GCPU3", "GCPU3Peak", "GCPU4", "GCPU4Peak", "GCPU5", "GCPU5Peak",
            "GCPU6", "GCPU6Peak", "GCPU7", "GCPU7Peak", "GCPU8", "GCPU8Peak",
            "GCPU9", "GCPU9Peak", "GCPU10", "GCPU10Peak", "GCPU11", "GCPU12Peak",
            "GCPU13", "GCPU13Peak", "GCPU14", "GCPU14Peak", "GCPU15", "GCPU15Peak"};

        public string[] gloabalTotalVariables = {"TimeStamp", "GCPU", "GCPUPeak",
            "GMA", "GMAPeak", "GPC", "GPCPeak", "GHC", "GHCPeak",
            "GHPF", "GCPUP", "GCPUPPeak", "GMF", "GMFPeak",
            "GMFPeak", "GMCOMM", "GMCOMMPeak", "GML", "GMLPeak",
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
            processNamesList = dlist.Select(list => list[1]).ToList();
            IEnumerable<string> distinctNotes = processNamesList.Distinct();
            return new List<string>(distinctNotes);
        }

        public List<string> getDistinctProcessIDs(string processName)
        {
            List<string[]> filteredList = getProcessData(processName, null);
            processNamesList = filteredList.Select(list => list[2]).ToList();
            IEnumerable<string> distinctNotes = processNamesList.Distinct();
            return new List<string>(distinctNotes);
        }

        //Gets data for one process
        public List<string[]> getProcessData(string processName, string processId)
        {
            if (processName != null && processId == null && processName != "None")
            {
                processData2DList = dlist.Where(x => x[1] == processName).ToList();
                Console.WriteLine("Process name is {0}, but it's ID is null\nReturning the list filtered by process name only", processName);
                return processData2DList;
            }
            else if (processName != null && processId != null)
            {
                Console.WriteLine("Process name is {0}, process ID is {1}\nReturning the list filtered by process name and ID", processName, processId);
                processData2DList = dlist.Where(x => x[1] == processName && x[2] == processId).ToList();
                return processData2DList;
            }
            //else if (processName == null && processId == null)
            //{
            //return new List<string[]>();
                
            //}
            Console.WriteLine("Both process name and it's ID are null\nReturning the whole list");
            return dlist;
            
        }
        

        public void getJsonString(OpenFileDialog dialog)
        {
            dlist = new List<string[]>();
            this.fileName = dialog.SafeFileName;
            string line;
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
                    if (singleList.Count > 0) { 
                        dlist.Add(singleList.ToArray());
                    }
                    counter++;
                }
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
            
            yield return procName;
            yield return procID;
            data = data.Substring(data.IndexOf(":") + 2);

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

/*
        public List<int> getProcessPositions(string process)
        {
            List<int> positionList = new List<int>();
            positionList = Enumerable.Range(0, processNames.Count)
                .Where(i => processNames[i] == process)
                .ToList();

            return positionList;
        }
        */

/*
 * Writing a JSON -> might be useful later, don't wanna delete
public void InitialJSON()
{
    this.fileName = dialog.SafeFileName;
    //handmadeJSON += "{\n\t\"" + this.fileName + "\": [";
    string line;

    System.IO.StreamReader file = new System.IO.StreamReader(dialog.FileName);

    file.ReadLine(); // skip the firstLine

    while ((line = file.ReadLine()) != null)
    {
        string[] mainDivision = line.Split('|');

        string processData = mainDivision[4];
        string timeStamp = mainDivision[0];
        TSList.Add(processData);
        DataList.Add(processData);
        /*
        char[] delimiterChars = { ':', ';' };
        string[] singleDataPoints = processData.Split(delimiterChars);

        if (singleDataPoints.Length < 19) continue;

        counter = 0;


        if (singleDataPoints[0] == "Process")
        {
            handmadeJSON += "\n\t\t{\n\t\t\"TimeStamp\": \"" + mainDivision[0] + "\",";
            handmadeJSON += "\n\t\t\"ProcessName\": \"" + singleDataPoints[1].Trim() + "\",";

            for (int i = 3; i < singleDataPoints.Length; i += 2)
            {
                char[] dels = { 'M', 'C', '%' };
                string droppedExt = singleDataPoints[i].Split(dels)[0];

                if (counter != 17)
                    handmadeJSON += "\n\t\t\"" + processVariables[counter] + "\":" + droppedExt + ",";
                else
                    handmadeJSON += "\n\t\t\"" + processVariables[counter] + "\":" + droppedExt;
                counter++;

            }

            handmadeJSON += "\n\t\t},";

        }

    }

    handmadeJSON = handmadeJSON.Substring(0, (handmadeJSON.Length - 1));


    //Console.WriteLine("JSON STRING: " + handmadeJSON);

    System.Console.ReadLine();
    handmadeJSON += "\n\t]}";
    return handmadeJSON;


    file.Close();
    return "";
    //SortedList<String, Object> jsonObj = JsonConvert.DeserializeObject<SortedList<String, Object>>(handmadeJSON);
    //Console.WriteLine("\nJSON OBJECT: " + jsonObj["MrResourceMonitoring-RIT - Copy.utr"]);
}
*/
