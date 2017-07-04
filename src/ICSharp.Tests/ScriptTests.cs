using ICSharp.Kernel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharp.Tests
{
    [TestClass]
    public class ScriptTests
    {
        [TestMethod]
        public void TestSimpleExpression()
        {
            string add = "5 + 7";
            Evaluation.EvalInteraction(add);

            string result = Evaluation.sbOut.ToString();

            Assert.AreEqual(result, "12");
        }
    }

}
