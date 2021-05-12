using System;
using System.Collections.Generic;

namespace EngineLayer
{
    public static class Classifier
    {
        public static int ClassifyResultFiles(List<string> resultFiles, bool aggregateOutput)
        {
            try
            {
                WriteOutput.Notify("Starting classification.");

                //read in results
                WriteOutput.Notify("Reading results.");
                List<PrSM>[] allPrSMs = ReadResults.ReadAllFiles(resultFiles);

                //output classified results
                WriteOutput.Notify("Writing output.");
                WriteOutput.WriteAllOutput(resultFiles, allPrSMs, aggregateOutput);
                WriteOutput.Notify("Done!");
                return 0;
            }
            catch (Exception e)
            {
                WriteOutput.Notify("Error: " + e.Message);
                WriteOutput.Notify("The run has been canceled.");
                return 3;
            }
        }
    }
}
