using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineLayer
{
    public static class Validate
    {
        public static int ValidateResults(string resultFile)
        {
            try
            {
                WriteOutput.Notify("Starting validation of " + resultFile);
                List<string> failedLevels = new List<string>();
                Dictionary<string, string> levelsToErrorMessages = GetLevelErrorMessages();
                //read in results, sort by scan
                List<PrSM> allPrSMs = ReadResults.ReadSingleFile(resultFile).OrderBy(x => x.Scan).ToList();
                List<string> levels = new List<string> { "1", "4", "2C", "5", "2B", "2D", "3", "2A" }; //each of these should be identified in the vignette
                List<string> scans = new List<string> { "8", "45", "50", "54", "58", "64", "68", "88" };
                Dictionary<string, bool> scanFoundDictionary = new Dictionary<string, bool>();
                Dictionary<string, string> scanToLevelDictionary = new Dictionary<string, string>();
                bool error = false;

                foreach (string scan in scans)
                {
                    scanFoundDictionary[scan] = false;
                }
                for (int i = 0; i < levels.Count; i++)
                {
                    scanToLevelDictionary[scans[i]] = levels[i];
                }

                //iterate through each result, check if there's a header or not
                int endingIndexModifier = allPrSMs.Count > levels.Count ? allPrSMs.Count - levels.Count : 0;
                for (int i = 0; i < allPrSMs.Count - endingIndexModifier; i++)
                {
                    PrSM prsm = allPrSMs[i];

                    //find the scan
                    if (scanToLevelDictionary.TryGetValue(prsm.Scan, out string expectedLevel))
                    {
                        scanFoundDictionary[prsm.Scan] = true;
                        if (!prsm.Level.Equals(expectedLevel))
                        {
                            WriteOutput.Notify("Error! Expected scan #" + prsm.Scan + " to be a level " + expectedLevel + " PrSM, but it was a level " + prsm.Level);
                            failedLevels.Add(expectedLevel);
                            error = true;
                        }
                    }
                    else //found a weird scan...
                    {
                        if (i != 0)//if not the header
                        {
                            WriteOutput.Notify("Error! Expected a scan number in the first column, but found '" + prsm.Scan + "'");
                            error = true;
                        }
                    }
                }

                //evaluate results
                foreach (string scan in scans)
                {
                    if (!scanFoundDictionary[scan])
                    {
                        WriteOutput.Notify("Error! Expected an entry for scan #" + scan + ".");
                        error = true;
                    }
                }
                if (!error)
                {
                    WriteOutput.Notify("Success! This output is ambiguity compliant and can be classified.");
                }
                else
                {
                    foreach (string failedLevel in failedLevels)
                    {
                        if (levelsToErrorMessages.TryGetValue(failedLevel, out string message))
                        {
                            WriteOutput.Notify(message);
                        }
                    }
                    WriteOutput.Notify("Validation was unsuccessful.");
                }
                return 0;
            }
            catch (Exception e)
            {
                WriteOutput.Notify("Error: " + e.Message);
                WriteOutput.Notify("The run has been canceled.");
                return 2;
            }
        }

        private static Dictionary<string, string> GetLevelErrorMessages()
        {
            Dictionary<string, string> levelsToErrorMessages = new Dictionary<string, string>();
            levelsToErrorMessages["2A"] = "Level 2A was not found. The program might not be transparent about PTM localization.";
            levelsToErrorMessages["2B"] = "Level 2B was not found. The program might not be transparent about PTM identification. Acetyl and Trimethyl were not both reported.";
            levelsToErrorMessages["2C"] = "Level 2C was not found. The program might not be transparent about amino acid sequence ambiguity.";
            levelsToErrorMessages["2D"] = "Level 2D was not found. The program might not be transparent about gene ambiguity.";
            return levelsToErrorMessages;
        }
    }
}