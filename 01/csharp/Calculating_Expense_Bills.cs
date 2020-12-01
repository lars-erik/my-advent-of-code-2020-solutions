using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;

namespace day1
{
    public class Calculating_Expense_Bills
    {
        int[] input;

        [SetUp]
        public void Initialize_Input()
        {
            using var inputStream = GetType()
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
                .Join(input, x => true, y => true, (x, y) => (x, y))
                .Where(xy => xy.x + xy.y == 2020)
                .Select(xy => xy.x * xy.y)
                .First();
            
            Assert.That(output, Is.EqualTo(319531));
        }
        
        [Test]
        public void Need_Product_Of_2020_Join()
        {
            var output = input
                .Join(input, x => true, y => true, (x, y) => (x, y))
                .Join(input, x => true, y => true, (xy, z) => (xy.x, xy.y, z))
                .Where(xyz => xyz.x + xyz.y + xyz.z == 2020)
                .Select(xyz => xyz.x * xyz.y * xyz.z)
                .First();
            
            Assert.That(output, Is.EqualTo(244300320));
        }
        
        [Test]
        public void Need_Product_Of_2020_SelectMany()
        {
            var output = input.SelectMany(
                    x => input.SelectMany(
                    y => input.Select(
                    z => (x, y, z)
                )))
                .Where(xyz => xyz.x + xyz.y + xyz.z == 2020)
                .Select(xyz => xyz.x * xyz.y * xyz.z)
                .First();
            
            Assert.That(output, Is.EqualTo(244300320));
        }
        
        [Test]
        public void Need_Product_Of_2020_LINQ()
        {
            var output = (
                from x in input
                from y in input
                from z in input
                where
                    x + y + z == 2020
                select
                    x * y * z
                )
                .First();

            Assert.That(output, Is.EqualTo(244300320));
        }

        [Test]
        public void Need_Product_Of_2020_Triplets_Loop()
        {
            Func<int> findTriplet = () => {
                foreach(var x in input)
                    foreach(var y in input)
                        foreach(var z in input)
                            if (x + y + z == 2020)
                                return x * y * z;
                return -1;
            };
            var output = findTriplet();
            
            Assert.That(output, Is.EqualTo(244300320));
        }
    }
}