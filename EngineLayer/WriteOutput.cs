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
            if(aggregateOutput) //combine all files into a single output
            {
                List<PrSM> aggregatedPrSMs = allPrSMs.SelectMany(x => x.Select(y => y)).ToList();
                string firstResultFile = resultFiles.First();
                string filenameWithoutExtension = Path.Combine(Path.GetDirectoryName(firstResultFile), "AllPrSMs");
                string extension = Path.GetExtension(firstResultFile);
                WriteOutputForOneFile(aggregatedPrSMs, filenameWithoutExtension, extension);
            }
            else //write each output file individually
            {
                for(int fileIndex = 0; fileIndex<resultFiles.Count; fileIndex++)
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
            foreach(string level in classificationLevels)
            {
                summaryOutput.Add(level + columnDelimiter + prsmsForThisFile.Count(x => x.Level.Equals(level)).ToString());
            }
            //DEBUG CODE FOR MANUSCRIPT
            //List<PrSM> lv3 = prsmsForThisFile.Where(x => x.Level.Equals("3")).ToList();
            //int A =lv3.Count(x => !x.A);
            //int B = lv3.Count(x => !x.B);
            //int C = lv3.Count(x => !x.C);
            //int D = lv3.Count(x => !x.D);
            //int AB = lv3.Count(x => !x.A && !x.B);
            //int AC = lv3.Count(x => !x.A && !x.C);
            //int AD = lv3.Count(x => !x.A && !x.D);
            //int BC = lv3.Count(x => !x.B && !x.C);
            //int BD = lv3.Count(x => !x.B && !x.D);
            //int CD = lv3.Count(x => !x.C && !x.D);
            //int sum = A + B + C + D;
            //int divide = sum / 2;
            //int sum2 = AB + AC + AD + BC + BD + CD;
            //Notify("A " + lv3.Count(x => !x.A).ToString());
            //Notify("B " + lv3.Count(x => !x.B).ToString());
            //Notify("C " + lv3.Count(x => !x.C).ToString());
            //Notify("D " + lv3.Count(x => !x.D).ToString());
            //Notify("A+B " + lv3.Count(x => !x.A && !x.B).ToString());
            //Notify("A+C " + lv3.Count(x => !x.A && !x.C).ToString());
            //Notify("A+D " + lv3.Count(x => !x.A && !x.D).ToString());
            //Notify("B+C " + lv3.Count(x => !x.B && !x.C).ToString());
            //Notify("B+D " + lv3.Count(x => !x.B && !x.D).ToString());
            //Notify("C+D " + lv3.Count(x => !x.C && !x.D).ToString());
            //summaryOutput.Add(columnDelimiter.ToString());
            //Dictionary<string, int> ptmsForLv3 = new Dictionary<string, int>();
            //foreach(PrSM prsm in lv3.Where(x=>!x.B))
            //{
            //    List<string> ptms = new List<string>();
            //    foreach(string proteoform in prsm.Proteoforms)
            //    {
            //        string[] split = proteoform.Split('[');
            //        for(int i=1; i<split.Length; i++)
            //        {
            //            string ptm = split[i].Split(']')[0];
            //            if(!ptms.Contains(ptm))
            //            {
            //                ptms.Add(ptm);
            //            }
            //        }
            //    }
            //    foreach(string ptm in ptms)
            //    {
            //        if (ptmsForLv3.ContainsKey(ptm))
            //        {
            //            ptmsForLv3[ptm]++;
            //        }
            //        else
            //        {
            //            ptmsForLv3[ptm] = 1;
            //        }
            //    }
            //}
            //var kvps = ptmsForLv3.Select(x => x).OrderByDescending(x => x.Value).ToList();

            //List<PrSM> geneAmbiguities = prsmsForThisFile.Where(x => !x.D).ToList();
            //Dictionary<string, int> geneDict = new Dictionary<string, int>();
            //foreach(PrSM prsm in geneAmbiguities)
            //{
            //    List<string> genes = new List<string>();
            //    foreach (string gene in prsm.Genes)
            //    {
            //        if(!genes.Contains(gene))
            //        {
            //            genes.Add(gene);
            //        }
            //    }
            //    foreach (string gene in genes)
            //    {
            //        if (geneDict.ContainsKey(gene))
            //        {
            //            geneDict[gene]++;
            //        }
            //        else
            //        {
            //            geneDict[gene] = 1;
            //        }
            //    }
            //}
            //var geneKvps = geneDict.Select(x => x).OrderByDescending(x => x.Value).ToList();


            int prsmsWithAmbiguity = prsmsForThisFile.Count(x => !x.Level.Equals("1"));
            summaryOutput.Add("PrSMs with Ambiguity" + columnDelimiter + prsmsWithAmbiguity.ToString());
            summaryOutput.Add("Total PrSMs" + columnDelimiter + prsmsForThisFile.Count().ToString());
            summaryOutput.Add("Fraction of PrSMs with Ambiguity" + columnDelimiter + (prsmsWithAmbiguity * 1d / prsmsForThisFile.Count()).ToString());
            File.WriteAllLines(summaryOutputPath, summaryOutput);
        }

        public static void Notify(string s)
        {
            NotificationHandler?.Invoke(null, new StringEventArgs(s));
        }
    }
}
