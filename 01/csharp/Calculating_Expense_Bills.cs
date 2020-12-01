using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace day1
{
    public class Calculating_Expense_Bills
    {
        int[] input;

        [SetUp]
        public void Initialize_Input()
        {
            var inputStream = GetType()
                .Assembly
                .GetManifestResourceStream("day1.input.txt");
            input = new StreamReader(inputStream)
                .ReadToEnd()
                .Split(Environment.NewLine)
                .Select<object, int>(Convert.ToInt32)
                .ToArray();
        }

        [Test]
        public void Need_Product_Of_2020_Pairs()
        {
            var output = input
                .Join(input, x => true, x => true, (x, y) => (x, y))
                .Where(xy => xy.x + xy.y == 2020)
                .Select(xy => xy.x * xy.y)
                .First();
            
            Assert.That(output, Is.EqualTo(319531));
        }
        
        [Test]
        public void Need_Product_Of_2020_Triplets()
        {
            var output = input
                .Join(input, x => true, x => true, (x, y) => (x, y))
                .Join(input, x => true, x => true, (xy, z) => new[]{xy.x, xy.y, z})
                .Where(xyz => xyz.Sum() == 2020)
                .Select(xyz => xyz.Aggregate((p, n) => p * n))
                .First();
            
            Assert.That(output, Is.EqualTo(244300320));
        }
    }
}