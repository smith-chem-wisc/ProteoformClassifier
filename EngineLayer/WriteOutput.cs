using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EngineLayer
{
    public static class WriteOutput
    {
        public static event EventHandler<StringEventArgs> NotificationHandler;

        public static void WriteAllOutput(List<string> resultFiles, List<PrSM>[] allPrSMs, bool aggregateOutput)
        {
            if (aggregateOutput) //combine all files into a single output
            {
                List<PrSM> aggregatedPrSMs = allPrSMs.SelectMany(x => x.Select(y => y)).ToList();
                string firstResultFile = resultFiles.First();
                string filenameWithoutExtension = Path.Combine(Path.GetDirectoryName(firstResultFile), "AllPrSMs");
                string extension = Path.GetExtension(firstResultFile);
                WriteOutputForOneFile(aggregatedPrSMs, filenameWithoutExtension, extension);
            }
            else //write each output file individually
            {
                for (int fileIndex = 0; fileIndex < resultFiles.Count; fileIndex++)
                {
                    string currentFile = resultFiles[fileIndex];
                    string filenameWithoutExtension = Path.Combine(Path.GetDirectoryName(currentFile), Path.GetFileNameWithoutExtension(currentFile));
                    string extension = Path.GetExtension(currentFile);
                    WriteOutputForOneFile(allPrSMs[fileIndex], filenameWithoutExtension, extension);
                }
            }
        }

        public static void WriteOutputForOneFile(List<PrSM> prsmsForThisFile, string filenameWithoutExtension, string extension)
        {
            //get filepaths for output
            string classifiedResultsPath = filenameWithoutExtension + "_ClassifiedResults" + extension;
            string summaryOutputPath = filenameWithoutExtension + "_ClassifiedSummary" + extension;
            char columnDelimiter = ReadResults.GetColumnDelimiter();

            //Write the PrSM-specific output
            Notify("Writing " + classifiedResultsPath);
            List<string> classifiedOutput = prsmsForThisFile.Select(x => x.Line + columnDelimiter + x.Level).ToList();
            File.WriteAllLines(classifiedResultsPath, classifiedOutput);

            //Write the classification summary
            Notify("Writing " + summaryOutputPath);
            List<string> classificationLevels = new List<string> { "1", "2A", "2B", "2C", "2D", "3", "4", "5" };
            List<string> summaryOutput = new List<string> { "5-Level Classification" + columnDelimiter + "Number of PrSMs" };
            foreach (string level in classificationLevels)
            {
                summaryOutput.Add(level + columnDelimiter + prsmsForThisFile.Count(x => x.Level.Equals(level)).ToString());
            }

            int prsmsWithAmbiguity = prsmsForThisFile.Count(x => !x.Level.Equals("1"));
            summaryOutput.Add("PrSMs with Ambiguity" + columnDelimiter + prsmsWithAmbiguity.ToString());
            summaryOutput.Add("Total PrSMs" + columnDelimiter + prsmsForThisFile.Count().ToString());
            summaryOutput.Add("Fraction of PrSMs with Ambiguity" + columnDelimiter + (prsmsWithAmbiguity * 1d / prsmsForThisFile.Count()).ToString());
            File.WriteAllLines(summaryOutputPath, summaryOutput);

            //write classification level key
            List<string> key = new List<string>
            {
                "This file exists to explain what each of the proteoform ambiguity levels means.",
                "",
                "Classification Level\tDescription",
                "1\tNo sources of ambiguity",
                "2A\tAmbiguity in PTM localization",
                "2B\tAmbiguity in PTM identification",
                "2C\tAmbiguity in sequence",
                "2D\tAmbiguity in gene of origin",
                "3\tTwo sources of ambiguity",
                "4\tThree sources of ambiguity",
                "5\tFour sources of ambiguity"
            };
            File.WriteAllLines(Path.Combine(Path.GetDirectoryName(filenameWithoutExtension),"ClassificationLevel_Key.txt"), key);
        }

        public static void Notify(string s)
        {
            NotificationHandler?.Invoke(null, new StringEventArgs(s));
        }
    }
}