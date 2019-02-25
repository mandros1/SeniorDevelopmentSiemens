using Microsoft.Win32;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SiemensPerformance
{
    class DataGenerator
    {

        private string handmadeJSON { get; set; }
        private int counter;
        public string fileName { get; set; }

        public string[] processVariables = { "WSP", "WSPPeak",
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

        public String getJsonString(OpenFileDialog dialog)
        {
            this.fileName = dialog.SafeFileName;
            handmadeJSON += "{\n\t\"" + this.fileName + "\": [";
            string line;

            System.IO.StreamReader file = new System.IO.StreamReader(dialog.FileName);
            file.ReadLine(); // skip the firstLine
            while ((line = file.ReadLine()) != null)
            {
                string[] mainDivision = line.Split('|');

                string processData = mainDivision[4];
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
            file.Close();

            System.Console.ReadLine();
            handmadeJSON += "\n\t]}";
            return handmadeJSON;
            //SortedList<String, Object> jsonObj = JsonConvert.DeserializeObject<SortedList<String, Object>>(handmadeJSON);
            //Console.WriteLine("\nJSON OBJECT: " + jsonObj["MrResourceMonitoring-RIT - Copy.utr"]);
        }
    }
}
