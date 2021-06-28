using NUnit.Framework;
using System.IO;
using EngineLayer;
using System.Collections.Generic;

namespace Test
{
    public class VignetteTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ValidationTest()
        {
            string validationResults = Path.Combine(TestContext.CurrentContext.TestDirectory, "ValidationFiles", "Results.tsv");
            int returnCode = Validate.ValidateResults(validationResults);

            Assert.IsTrue(returnCode == 0);
        }

        [Test]
        public void ClassificationTest()
        {
            string validationResults = Path.Combine(TestContext.CurrentContext.TestDirectory, "ValidationFiles", "Results.tsv");
            int returnCode = Classifier.ClassifyResultFiles(new List<string> { validationResults }, false);

            Assert.IsTrue(returnCode == 0);
        }

        [Test] 
        public void Debug()
        {
            List<string> lines = new List<string>
            {
                "1653\tARTKQTARKSTGGKAPRKQLATKAARKSAPSTGGVKKPHRYRPGTVALREIRRYQK[UniProt:N6-glutaryllysine on K]STELLIRKLPFQRLVREIAQDFK[UniProt:N6-succinyllysine on K]TDLRFQSAAIGALQEASEAYLVGLFEDTNLC[Common Fixed:Carbamidomethyl on C]AIHAKRVTIMPKDIQLARRIRGERA|ARTKQTARKSTGGKAPRKQLATKAARKSAPSTGGVKKPHRYRPGTVALREIRRYQK[UniProt:N6-glutaryllysine on K]STELLIRKLPFQRLVREIAQDFK[UniProt:N6-succinyllysine on K]TDLRFQSAAIGALQEASEAYLVGLFEDTNLC[Common Fixed:Carbamidomethyl on C]AIHAKRVTIMPKDIQLARRIRGERA|ARTKQTARKSTGGKAPRKQLATKAARKSAPSTGGVKKPHRYRPGTVALREIRRYQK[UniProt:N6-succinyllysine on K]STELLIRKLPFQRLVREIAQDFK[UniProt:N6-glutaryllysine on K]TDLRFQSAAIGALQEASEAYLVGLFEDTNLC[Common Fixed:Carbamidomethyl on C]AIHAKRVTIMPKDIQLARRIRGERA|ARTKQTARKSTGGKAPRKQLATKAARKSAPSTGGVKKPHRYRPGTVALREIRRYQK[UniProt:N6-succinyllysine on K]STELLIRKLPFQRLVREIAQDFK[UniProt:N6-glutaryllysine on K]TDLRFQSAAIGALQEASEAYLVGLFEDTNLC[Common Fixed:Carbamidomethyl on C]AIHAKRVTIMPKDIQLARRIRGERA\tprimary:H3-3A, synonym:H3.3A, synonym:H3F3, synonym:H3F3A, ORF:PP781, primary:H3-3B, synonym:H3.3B, synonym:H3F3B"
            };
            string path = Path.Combine(TestContext.CurrentContext.TestDirectory, "debug.tsv");
            File.WriteAllLines(path, lines);
            ReadResults.ModifyHeader(false);
            List<PrSM> allPrSMs = ReadResults.ReadSingleFile(path);
            Assert.IsTrue(allPrSMs[0].Level.Equals("2A"));
        }
    }
}