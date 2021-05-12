using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CMD
{
    public class CommandLineSettings
    {
        [Option('r', HelpText = "Proteoform result files (.csv, .tsv, .txt); space-delimited")]
        public IEnumerable<string> ResultFiles { get; set; }

        [Option('a', HelpText = "[Optional] Aggregate output when multiple files are used")]
        public bool AggregateOutput { get; set; }

        [Option('c', HelpText = "[Optional] Column delimiter")]
        public char ColumnDelimiter { get; set; }

        [Option('p', HelpText = "[Optional] Proteoform sequence/Gene delimiter")]
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
    }
}
