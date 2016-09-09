using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PentahoDataIntegration.Tests
{
    /// <summary>
    /// Contains tests for that will run pentaho transforms that are configured to test transform logic.
    /// The tests pass if the transform file runs successfully. The output of the tests can then be run through a test result xml handler.
    /// </summary>
    [TestFixture]
    public class PentahoTransformTests
    {
        /// <summary>
        /// The path to the pan.bat file on the system.
        /// </summary>
        private string panPath;

        /// <summary>
        /// Setups this instance. 
        /// </summary>
        [OneTimeSetUp]
        public void Setup()
        {
            this.panPath = ConfigurationManager.AppSettings["panPath"];
        }

        /// <summary>
        /// Runs the pentaho test transform and asserts that it exited without error.
        /// </summary>
        /// <param name="transformFile">The transform file.</param>
        [Test, TestCaseSource("GetTestCases")]
        public void TransformSuccedes(string transformFile)
        {
            // Arrange
            var processInfo = new ProcessStartInfo(this.panPath, " -file=\"" + transformFile + "\"");
            processInfo.WindowStyle = ProcessWindowStyle.Hidden;

            // Act
            var process = new Process();
            process.StartInfo = processInfo;
            process.EnableRaisingEvents = true;
         
            process.Start();

            // Assert
            process.WaitForExit(120 * 1000);

            Assert.AreEqual(0, process.ExitCode);
        }

        /// <summary>
        /// Gets the test cases.
        /// </summary>
        /// <returns>Test cases</returns>
        private static TestCaseData[] GetTestCases()
        {
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var checkDirectory = Path.Combine(currentDirectory, "..\\");
            var files = Directory.GetFiles(checkDirectory, "Test*.ktr", SearchOption.AllDirectories);
            var paths = files.Select(i => Path.GetFullPath(i)).ToArray();
            var testCases = paths.Select(i => new TestCaseData(i).SetName(i.Split('\\').Last()));
            return testCases.ToArray();
        }
    }
}
