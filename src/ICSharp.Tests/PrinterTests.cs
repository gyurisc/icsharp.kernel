using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ICSharp.Kernel;

namespace ICSharp.Tests
{
    [TestClass]
    public class PrinterTests
    {
        [TestMethod]
        public void TestPrintingSimpleList()
        {
            List<int> numbers = new List<int>() { 1, 2, 3, 4, 5, 6 };
            var result = Printers.PrintVariable(numbers);

            Assert.IsInstanceOfType(result, typeof(BinaryOutput));
            Assert.AreEqual(result.ContentType, "text/plain");
            Assert.AreEqual("List<int>(6) { 1, 2, 3, 4, 5, 6 }", result.Data);
        }

        [TestMethod]
        public void TesPrintingInt()
        {
            var number = 8;
            var result = Printers.PrintVariable(number);

            Assert.IsInstanceOfType(result, typeof(BinaryOutput));
            Assert.AreEqual(result.ContentType, "text/plain");
            Assert.AreEqual("8", result.Data);
        }

        [TestMethod]
        public void TesPrintingString()
        {
            string text = "Hello";
            var result = Printers.PrintVariable(text);

            Assert.IsInstanceOfType(result, typeof(BinaryOutput));
            Assert.AreEqual(result.ContentType, "text/plain");
            Assert.AreEqual("\"Hello\"", result.Data);
        }


        public class SimpleClass
        {
            public string TextField { get; set; }
        }

        [TestMethod]
        public void TestRemovalOfSubmission()
        {
            SimpleClass s = new SimpleClass() { TextField = "Simple" };

            var result = Printers.PrintVariable(s);
            Assert.IsInstanceOfType(result, typeof(BinaryOutput));
            Assert.AreEqual(result.ContentType, "text/plain");
            Assert.AreEqual("PrinterTests.SimpleClass { TextField=\"Simple\" }", result.Data);
        }   
    }
}
