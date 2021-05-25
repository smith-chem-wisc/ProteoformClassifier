using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using EngineLayer;

namespace CMD
{
    public class CommandLineSettings
    {
        [Option('r', HelpText = "Proteoform result files (.tsv, .txt); space-delimited")]
        public IEnumerable<string> ResultFiles { get; set; }

        [Option('a', HelpText = "[Optional] Aggregate output when multiple files are used")]
        public bool AggregateOutput { get; set; }

        [Option('c', HelpText = "[Optional] Column delimiter")]
        public char ColumnDelimiter { get; set; }

        [Option('h', HelpText = "[Optional] Use if a header row is not present in the result file(s)")]
        public bool NoHeader {get;set;}

        [Option('n', HelpText = "[Optional] Accepts 'p' for parenthetical or 'm' for multiple rows. Pipe format expected if p or m are not specified.  Pipe format: XM[Oxidation]AMX|XMAM[Oxidation]X. Parenthetical format: X(MAM)[Oxidation]X. Multiple row format gives each possible proteoform a separate row with the same scan number."
        public char AmbiguityNotation { get; set; }

        [Option('s', HelpText = "[Optional] Proteoform sequence/Gene delimiter")]
        public char SequenceDelimiter { get; set; }

        [Option('v', HelpText = "[Optional] Validate vignette if specified, otherwise classify the provided files")]
        public bool Validate { get; set; }


        public void ValidateCommandLineSettings()
        {
            //test if there are files
            if (ResultFiles.Count() < 1)
            {
                throw new Exception("No result files detected. Use the --help parameter to view all parameters (e.g., \"CMD.exe --help\")");
            }
        }

        public void ImplementSettings()
        {
            if (ColumnDelimiter != 0)
            {
                ReadResults.ModifyColumnDelimiter(ColumnDelimiter);
            }
            if (NoHeader)
            {
                ReadResults.ModifyHeader(false);
            }
            char format = char.ToLower(AmbiguityNotation);
            if (format == 'p')
            {
                ReadResults.ModifyProteoformFormat(ProteoformFormat.Parenthetical);
            }
            else if (format == 'm')
            {
                ReadResults.ModifyProteoformFormat(ProteoformFormat.MultipleRows);
            }
            if (SequenceDelimiter != 0)
            {
                ReadResults.ModifySequenceAndGeneDelimiter(SequenceDelimiter);
            }
        }
    }
}
