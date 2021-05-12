using Proteomics;
using Proteomics.ProteolyticDigestion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineLayer
{
    public class PrSM
    {
        public string Scan { get; private set; }
        public string[] Proteoforms {get; private set;}
        public string[] Genes { get; private set; }
        public string Level { get; private set; }
        public string Line { get; private set; }
        public bool A { get; set; }
        public bool B { get; set; }
        public bool C { get; set; }
        public bool D { get; set; }


        public PrSM(string line, string scan, string[] proteoforms, string[] genes)
        {
            Line = line;
            Scan = scan;           
            Proteoforms = UnifyPtmAnnotation(proteoforms);
            Genes = genes;
            Level = ClassifyPrSM(string.Join('|', Proteoforms), string.Join('|', Genes));
        }

        public string[] UnifyPtmAnnotation(string[] proteoforms)
        {
            //modify ptms for MM so that they're considered the same if they're the same
            //For example, these sequences are currently different, even though they're both acetylation.
            //"[Common Biological:Acetylation on X]SLRKQTPSDFLK"
            //"SLRK[Common Biological:Acetylation on K]QTPSDFLK"
            //"[UniProt:N-acetylserine on S]SLRKQTPSDFLK"
            List<string> substringsToSearch = new List<string> {
                "cetyl",
                "xidation",
                "hospho"
            };
            List<string> stringsToReplace = new List<string> {
            "Acetylation",
            "Oxidation",
            "Phosphorylation"
            };
            for (int p = 0; p < proteoforms.Length; p++)
            {
                //DEBUG
                //starts with /
                if (proteoforms[p][0].Equals('\\'))
                { }
                if (proteoforms[p][0].Equals('"'))
                { }
                if (proteoforms[p][0].Equals('\"'))
                { }
                //different number of brackets?
                int numstart = proteoforms[p].Count(x => x.Equals('['));
                int numend = proteoforms[p].Count(x => x.Equals(']'));
                if (numstart != numend)
                { }
                else
                {
                    string[] split = proteoforms[p].Split('['); //separate by ptms
                    string builder = split[0];
                    for (int ptmIndex = 1; ptmIndex < split.Length; ptmIndex++)
                    {
                        string[] twoSplit = split[ptmIndex].Split(']');
                        string ptm = twoSplit[0];
                        for (int i = 0; i < substringsToSearch.Count; i++)
                        {
                            if (ptm.Contains(substringsToSearch[i]))
                            {
                                ptm = stringsToReplace[i];
                                break;
                            }
                        }
                        builder += "[" + ptm + "]" + twoSplit[1];
                    }
                    proteoforms[p] = builder;
                }
            }
            return proteoforms;
        }

        public string ClassifyPrSM(string fullSequenceString, string geneString)
        {
            //separate delimited input
            string[] sequences = fullSequenceString.Split('|');
            string[] genes = geneString.Split('|');


            //determine sequence ambiguity
            string firstBaseSequence = PeptideWithSetModifications.GetBaseSequenceFromFullSequence(sequences[0]).ToUpper(); //get first sequence with modifications removed
            bool sequenceIdentified = true; //check if there are any ambiguous amino acids (i.e. B, J, X, Z)
            //for every other sequence reported
            if (sequenceIdentified) //if there weren't any unknown amino acids reported.
            {
                for (int i = 1; i < sequences.Length; i++)
                {
                    //if the unmodified sequences don't match, then there's sequence ambiguity
                    if (!firstBaseSequence.Equals(PeptideWithSetModifications.GetBaseSequenceFromFullSequence(sequences[i]).ToUpper()))
                    {
                        sequenceIdentified = false;
                        break;
                    }
                }
            }


            //determine PTM localization and identification
            List<(int index, string ptm)> firstPTMsSortedByIndex = GetPTMs(sequences[0]); //get ptms from the first sequence reported
            List<string> firstPTMsSortedByPTM = firstPTMsSortedByIndex.Select(x => x.ptm).OrderBy(x => x).ToList(); //sort ptms alphabetically
            //check if there are unknown mass shifts
            bool ptmsIdentified = !PtmsContainUnknownMassShifts(firstPTMsSortedByPTM);
            bool ptmsLocalized = true; //assume these are localized unless we determine otherwise
            //for every other sequence reported
            for (int seqIndex = 1; seqIndex < sequences.Length; seqIndex++)
            {
                List<(int index, string ptm)> currentPTMsSortedByIndex = GetPTMs(sequences[seqIndex]); //get ptms from this sequence
                List<string> currentPTMsSortedByPTM = currentPTMsSortedByIndex.Select(x => x.ptm).OrderBy(x => x).ToList(); //sort ptms alphabetically

                //are number of PTMs the same?
                if (firstPTMsSortedByIndex.Count == currentPTMsSortedByIndex.Count)
                {
                    //check localization (are indexes conserved?)
                    for (int i = 0; i < firstPTMsSortedByIndex.Count; i++)
                    {
                        if (firstPTMsSortedByIndex[i].index != currentPTMsSortedByIndex[i].index)
                        {
                            ptmsLocalized = false;
                            break;
                        }
                    }
                    //check PTMs
                    for (int i = 0; i < firstPTMsSortedByPTM.Count; i++)
                    {
                        if (!firstPTMsSortedByPTM[i].Equals(currentPTMsSortedByPTM[i]))
                        {
                            ptmsIdentified = false;
                            break;
                        }
                    }
                }
                else
                {
                    ptmsIdentified = false;
                    ptmsLocalized = false;
                }
            }


            //determine gene ambiguity
            bool geneIdentified = genes.Length == 1;

            A = ptmsLocalized;
            B = ptmsIdentified;
            C = sequenceIdentified;
            D = geneIdentified;
            return ProteoformLevelClassifier.GetProteoformClassification(ptmsLocalized, ptmsIdentified, sequenceIdentified, geneIdentified);
        }

        /// <summary>
        /// Given a proteoform sequence (contains ptms), returns a list of all ptms and their one based index in order from N-terminus to C-terminus
        /// </summary>
        /// <param name="fullSequence"></param>
        /// <returns></returns>
        private static List<(int, string)> GetPTMs(string fullSequence)
        {
            List<(int, string)> ptmsToReturn = new List<(int, string)>();
            StringBuilder currentPTM = new StringBuilder();
            int currentIndex = 0;
            int numLeftBrackets = 0; //PTMs are annotated with brackets. This object keeps track of how many brackets deep we are

            //iterate through the sequence
            foreach (char c in fullSequence)
            {
                //if we found a right bracket
                if (c == ']')
                {
                    //record that we're stepping out of brackets
                    numLeftBrackets--;
                    //if we've finished the ptm
                    if (numLeftBrackets == 0)
                    {
                        //Add the ptm and clear the record
                        currentIndex--; //move back an index because we added one when we entered the bracket 
                        ptmsToReturn.Add((currentIndex, currentPTM.ToString()));
                        currentPTM.Clear();
                    }
                }
                else //if not a right bracket...
                {
                    //if we're already in a PTM, record it
                    if (numLeftBrackets > 0)
                    {
                        currentPTM.Append(c);
                    }
                    else //we're not in a PTM, so update where we are in the proteoform
                    {
                        currentIndex++; //this operation occurs when entering a PTM, so we need to substract when exiting the PTM
                    }
                    //if we're entering a PTM or a nested bracket, record it
                    if (c == '[')
                    {
                        numLeftBrackets++;
                    }
                }
            }

            return ptmsToReturn;
        }


        /// <summary>
        /// See if any of the reported PTMs are mass shifts, (e.g. [+15.99] or [-17.99]) or contain "?"
        /// </summary>
        /// <param name="ptms"></param>
        /// <returns></returns>
        private static bool PtmsContainUnknownMassShifts(List<string> ptms)
        {
            foreach (string ptm in ptms)
            {
                if (ptm.Length > 1) //check length is appropriate
                {
                    //remove sign with substring and try to parse into double. If it's a mass, tryparse returns true
                    if (double.TryParse(ptm.Substring(1), out double mass))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
