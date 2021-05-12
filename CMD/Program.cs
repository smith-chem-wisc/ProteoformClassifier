using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CommandLine;
using CommandLine.Text;
using EngineLayer;

namespace CMD
{
    class Program
    {
        private static bool InProgress;
        private static System.CodeDom.Compiler.IndentedTextWriter MyWriter = new System.CodeDom.Compiler.IndentedTextWriter(Console.Out, "\t");
        private const string WelcomeMessage = "Welcome to The Proteoform Classifier!";
        private static CommandLineSettings CommandLineSettings;

        public static int Main(string[] args)
        {
            // an error code of 0 is returned if the program ran successfully.
            // otherwise, an error code of >0 is returned.
            // this makes it easier to determine via scripts when the program fails.
            int errorCode = 0;
            WriteOutput.NotificationHandler += NotificationHandler;

            var parser = new Parser(with => with.HelpWriter = null);
            var parserResult = parser.ParseArguments<CommandLineSettings>(args);

            parserResult
              .WithParsed<CommandLineSettings>(options => errorCode = Run(options))
              .WithNotParsed(errs => errorCode = DisplayHelp(parserResult, errs));

            return errorCode;
        }


        public static int DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errs)
        {
            Console.WriteLine(WelcomeMessage);

            int errorCode = 0;

            var helpText = HelpText.AutoBuild(result, h =>
            {
                h.AdditionalNewLineAfterOption = false;
                h.Copyright = "";
                return HelpText.DefaultParsingErrorsHandler(result, h);
            }, e => e);

            helpText.MaximumDisplayWidth = 300;

            helpText.AddPostOptionsLine("Example usage (Validation): ");
            helpText.AddPostOptionsLine("CMD.exe -r C:\\ValidationResults.tsv -v");
            helpText.AddPostOptionsLine(Environment.NewLine);

            helpText.AddPostOptionsLine("Example usage (Classification): ");
            helpText.AddPostOptionsLine("CMD.exe -r C:\\ResultsToClassify_1.tsv C:\\ResultsToClassify_1.tsv");
            helpText.AddPostOptionsLine(Environment.NewLine);

            Console.WriteLine(helpText);

            if (errs.Any())
            {
                errorCode = 1;
            }

            return errorCode;
        }

        private static int Run(CommandLineSettings settings)
        {
            Console.WriteLine(WelcomeMessage);

            //validate input
            try
            {
                settings.ValidateCommandLineSettings();
                CommandLineSettings = settings;
            }
            catch (Exception e)
            {
                Console.WriteLine(Environment.NewLine + e.Message);

                return 1;
            }

            //run
            if (CommandLineSettings.Validate)
            {
                return Validate.ValidateResults(CommandLineSettings.ResultFiles.First());
            }
            else //classify
            {
                return Classifier.ClassifyResultFiles(CommandLineSettings.ResultFiles.ToList(), CommandLineSettings.AggregateOutput);
            }
        }

        private static void NotificationHandler(object sender, StringEventArgs e)
        {
            string[] tokens = Regex.Split(e.S, @"\r?\n|\r");
            foreach (var str in tokens)
            {
                MyWriter.WriteLine(str);
            }
        }
    }
}
