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
    }
}