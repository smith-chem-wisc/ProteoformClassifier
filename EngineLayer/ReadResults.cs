using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineLayer
{
    public static class ReadResults
    {

        private static char ColumnDelimiter = '\t';
        private static char SequenceAndGeneDelimiter = '|';
        private static ProteoformFormat Format = ProteoformFormat.Delimited;

        public static List<PrSM>[] ReadAllFiles(List<string> resultFiles)
        {
            List<PrSM>[] allPrSMs = new List<PrSM>[resultFiles.Count];

            for (int fileIndex = 0; fileIndex < resultFiles.Count; fileIndex++)
            {
                string currentFile = resultFiles[fileIndex];
                WriteOutput.Notify("Reading "+ currentFile);

                allPrSMs[fileIndex] = ReadSingleFile(currentFile);
            }
            return allPrSMs;
        }

        public static List<PrSM> ReadSingleFile(string file)
        {
            List<PrSM> prsmsToReturn = new List<PrSM>();

            string[] lines = File.ReadAllLines(file);
            foreach (string l in lines)
            {
                string[] line = l.Split(ColumnDelimiter);
                if (line.Length < 3)
                {
                    throw new Exception("The line " + l + " from the file " + file + " has fewer than 3 columns (scan#, sequence, gene) when using the delimiter '" + ColumnDelimiter + "'. Update your delimiter or check your result file.");
                }

                //remove any quotes... unclear what causes them during reading.
                line[1] = line[1].Replace("\"", "");

                //parse proteoforms
                if (Format == ProteoformFormat.Delimited)
                {
                    prsmsToReturn.Add(new PrSM(l, line[0], line[1].Split(SequenceAndGeneDelimiter), line[2].Split(SequenceAndGeneDelimiter)));
                }
                else //if parenthetical
                {
                    //look for parenthesis
                    string[] splitP = line[1].Split(')');

                    List<string> seqs = new List<string>();
                    for (int p = 0; p < splitP.Length - 1; p++)
                    {
                        int currentSeqs = seqs.Count;

                        //find how many AA are possible
                        string[] splitSplit = splitP[p].Split('(');
                        if (splitSplit.Length != 2)
                        {
                            throw new Exception("The line " + l + " from the file " + file + " has an unmatched ')' or nested parenthesis in its proteoform sequence '" + line[1] + ". Please check your input and try again.");
                        }
                        else
                        {
                            //splitsplit is length 2, with unambiguous aa on index 0 and ambiguous on index 1
                            string possibleAAs = splitSplit[1];
                            List<string> possibilities = new List<string>();
                            //there should be a mod after each closing parenthesis... check that there is
                            string[] ptmSplit = splitP[p + 1].Split(']');
                            string ptm = ptmSplit[0] + "]";
                            if (ptm[0] == ('[')) //check that ptm is valid
                            {
                                string[] splitSplitSplit = splitSplit[0].Split('['); 
                                string builder = splitSplitSplit[splitSplitSplit.Length-1];//get sequence without modification
                                for (int aa =0; aa<possibleAAs.Length; aa++)
                                {
                                    string builderAA = "";

                                    for (int aaa=0; aaa<aa+1; aaa++)
                                    {
                                        builderAA += possibleAAs[aaa];
                                    }
                                    builderAA += ptm;
                                    for (int aaa = aa+1; aaa < possibleAAs.Length; aaa++)
                                    {
                                        builderAA += possibleAAs[aaa];
                                    }
                                    //finished this, can add it to the rest of the builder.
                                    if(currentSeqs == 0) //if this is the first mod in the sequence
                                    {
                                        seqs.Add(builder + builderAA);
                                    }
                                    else
                                    {
                                        for(int s=0; s<seqs.Count; s++)
                                        {
                                            seqs[s] = seqs[s] + builder + builderAA;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                throw new Exception("The line " + l + " from the file " + file + " has a ')' that is not followed by a bracketed modification in its proteoform sequence '" + line[1] + ". Please check your input and try again.");
                            }
                        }
                    }
                    //add last bit of sequence to each seq
                    string[] finishSeqs = splitP[splitP.Length - 1].Split(']');
                    string finishSeq = finishSeqs[finishSeqs.Length - 1];
                    if(seqs.Count==0)
                    {
                        seqs.Add(finishSeq);
                    }
                    else
                    {
                        for (int s = 0; s < seqs.Count; s++)
                        {
                            seqs[s] = seqs[s] + finishSeq;
                        }
                    }
                    prsmsToReturn.Add(new PrSM(l, line[0], seqs.ToArray(), line[2].Split(SequenceAndGeneDelimiter)));
                }
            }

            return prsmsToReturn;
        }

        public static void ModifyColumnDelimiter(char c)
        {
            ColumnDelimiter = c;
        }
        public static void ModifySequenceAndGeneDelimiter(char c)
        {
            SequenceAndGeneDelimiter = c;
        }
        public static void ModifyProteoformFormat(ProteoformFormat pf)
        {
            Format = pf;
        }
        public static ProteoformFormat GetProteoformFormat()
        {
            return Format;
        }

        public static char GetColumnDelimiter()
        {
            return ColumnDelimiter;
        }
        public static char GetProteoformDelimiter()
        {
            return SequenceAndGeneDelimiter;
        }
    }
}
