using Microsoft.Win32;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;

namespace SiemensPerformance
{
    class DataGenerator
    {

        public List<string[]> dlist { get; set; }
        private List<string> singleList { get; set; }
        private string handmadeJSON { get; set; }
        public string fileName { get; set; }


        public string[] processVariables = {"TimeStamp", "Process Name", "WSP", "WSPPeak",
            "HC", "HCPeak", "TC", "TCPeak", "CPU", "CPUPeak",
            "GDIC", "GDICPeak", "USRC", "USRCPeak", "PRIV",
            "PRIVPeak", "VIRT", "VIRTPeak", "PFS", "PFSPeak" };

        private string[] globalVariables = { "GCPU0", "GCPU0Peak",
            "GCPU1", "GCPU1Peak", "GCPU2", "GCPU2Peak", "GCPU3", "GCPU3Peak", "GCPU4", "GCPU4Peak", "GCPU5", "GCPU5Peak",
            "GCPU6", "GCPU6Peak", "GCPU7", "GCPU7Peak", "GCPU8", "GCPU8Peak",
            "GCPU9", "GCPU9Peak", "GCPU10", "GCPU10Peak", "GCPU11", "GCPU12Peak",
            "GCPU13", "GCPU13Peak", "GCPU14", "GCPU14Peak", "GCPU15", "GCPU15Peak"};

        private string[] gloabalTotalVariables = { "GCPU", "GCPUPeak",
            "GMA", "GMAPeak", "GPC", "GPCPeak", "GHC", "GHCPeak",
            "GHPF", "GCPUP", "GCPUPPeak", "GMF", "GMFPeak",
            "GMFPeak", "GMCOMM", "GMCOMMPeak", "GML", "GMLPeak",
            "GPFC", "GPFCPeak", "GMC", "GMCPeak"};

        public string[] getProcessVars()
        {
            return this.processVariables;
        }

        public string[] getGlobalVars()
        {
            return this.globalVariables;
        }

        public string[] getTotalGlobalVars()
        {
            return this.gloabalTotalVariables;
        }

        /*
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
                        singleList.Add(item);
                        //sw.Write(item+",");
                    }
                    //sw.Write("\n");
                    dlist.Add(singleList.ToArray());
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
            yield return textData.Substring(0, 26); //timestamp
            string data = textData.Substring(textData.IndexOf(':', 26) + 2);
            string processName = data.Substring(0, data.IndexOf(":")); // process name
            yield return processName;
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
 

    public IEnumerable<string> GetSplitData(string textData)
        {
            Boolean initial = true;
            yield return textData.Substring(0, 26); //timestamp
            string data = textData.Substring(textData.IndexOf(':', 26) + 2);
            string processName = data.Substring(0, data.IndexOf(":")); // process name
            yield return processName;
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
                    if (initial)
                    {
                        if (processName == "GCPU(0)")
                        {
                            yield return "GCPU(0)";
                            yield return line;
                        }
                        else if (processName == "GCPU(_Total)")
                        {
                            yield return "GCPU(0)";
                            yield return line;
                        }
                        else
                        {
                            yield return line.Substring(0, colonIndex);
                            yield return line.Substring(colonIndex + 2);
                        }
                    }
                    else
                    {
                        line = line.Trim();
                        string result = Regex.Replace(line, @"[^\d]", "");
                        yield return result;
                        yield return line.Substring(0, colonIndex);
                        yield return line.Substring(colonIndex + 2);
                    }
                    
                }

                i = j + 1;
                j = data.IndexOf(';', i, finalLength - i);
                initial = false;
            }
            /*
            if (i < finalLength) // Has remainder?
            {
                yield return data.Substring(i, finalLength - i); // Return remaining trail
            }
        }
    }



if (i < finalLength) // Has remainder?
{
    dataDict.Add(line.Substring(0, line.IndexOf(":")), line.Substring(line.IndexOf(":") + 1));
    yield return data.Substring(i, finalLength - i); // Return remaining trail
}

int len = textData.Length;
string timeStamp = textData.Substring(0, 26);

int dataStartPos = textData.LastIndexOf('|')+1;
string data = textData.Substring(dataStartPos, (len-dataStartPos));

int colonPosition = data.IndexOf(":");

string type = data.Substring(0, colonPosition);
if(type == "Process")
{

}else if (type == "Global")
{

}
/*
int i = 0, j = s.IndexOf(c, 0, l);
if (j == -1) // No such substring
{
    yield return s; // Return original and break
    yield break;
}

while (j != -1)
{
    if (j - i > 0) // Non empty? 
    {
        yield return s.Substring(i, j - i); // Return non-empty match
    }
    i = j + 1;
    j = s.IndexOf(c, i, l - i);
}

if (i < l) // Has remainder?
{
    yield return s.Substring(i, l - i); // Return remaining trail
}

     /*
             * 
            while (j != -1)
            {
                if (j - i > 0) // Non empty? 
                {
                    line = data.Substring(i, j - i);
                    colonIndex = line.IndexOf(':');
                    if (initial)
                    {
                        if (processName == "GCPU(0)")
                        {
                            yield return "GCPU(0)";
                            yield return line;
                        }
                        else if (processName == "GCPU(_Total)")
                        {
                            yield return "GCPU(0)";
                            yield return line;
                        }
                        else
                        {
                            yield return line.Substring(0, colonIndex);
                            yield return line.Substring(colonIndex + 2);
                        }
                    }
                    else
                    {
                        line = line.Trim();
                        string result = Regex.Replace(line, @"[^\d]", "");
                        yield return result;
                        yield return line.Substring(0, colonIndex);
                        yield return line.Substring(colonIndex + 2);
                    }
                }
                i = j + 1;
                j = data.IndexOf(';', i, finalLength - i);
                initial = false;
            }
            */
