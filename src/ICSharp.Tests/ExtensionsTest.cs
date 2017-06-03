using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ICSharp.Kernel;

namespace ICSharp.Tests
{
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }

    [TestClass]
    public class ExtensionsTest
    {
        [TestMethod]
        public void TableExtensionTest()
        {
            List<Person> persons = new List<Person>();

            persons.Add(new Person() { FirstName = "John", LastName = "Doe", Age = 34 });
            persons.Add(new Person() { FirstName = "Jane", LastName = "Doe", Age = 26 });

            var output = persons.AsTable();

            Assert.IsInstanceOfType(output, typeof(TableOutput));

            TableOutput table = output as TableOutput;

            Assert.AreEqual(table.Columns.Count, 3);
            Assert.AreEqual(table.Columns[0], "FirstName");
            Assert.AreEqual(table.Columns[1], "LastName");
            Assert.AreEqual(table.Columns[2], "Age");

            Assert.AreEqual(table.Rows.Count, 2);

            Assert.AreEqual(table.Rows[0][0], "John");
            Assert.AreEqual(table.Rows[0][1], "Doe");
            Assert.AreEqual(table.Rows[0][2], "34");

            Assert.AreEqual(table.Rows[1][0], "Jane");
            Assert.AreEqual(table.Rows[1][1], "Doe");
            Assert.AreEqual(table.Rows[1][2], "26");
        }

    }
}
