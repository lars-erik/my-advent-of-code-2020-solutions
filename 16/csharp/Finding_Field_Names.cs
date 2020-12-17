using System;
using System.Linq;
using AoCSharp.Common;
using NUnit.Framework;

namespace day16
{
    public class Finding_Field_Names
    {
        private string[] inputLines;
        
        [SetUp]
        public void Setup()
        {
            inputLines = Resources.GetResourceLines(this, "day16.input.txt");
        }

        [Test]
        public void Test1()
        {
            inputLines.Take(50).ToList().ForEach(Console.WriteLine);
            
            Assert.Fail();
        }
    }
}