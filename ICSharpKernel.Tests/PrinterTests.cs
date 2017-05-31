using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ICSharp.Kernel;

namespace ICSharpKernel.Tests
{
    [TestClass]
    public class PrinterTests
    {
        [TestMethod]
        public void TestPrintingSimpleList()
        {
            List<int> numbers = new List<int>() { 1, 2, 3, 4, 5, 6};
            var result = Printers.PrintVariable(numbers);

            Assert.IsInstanceOfType(result, typeof(BinaryOutput));
            Assert.Equals(result.ContentType, "text/plain");
        }
    }
}
