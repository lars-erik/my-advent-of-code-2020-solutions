using System;
using System.IO;
using System.Linq;
using AoCSharp.Common;
using NUnit.Framework;

namespace day3
{
    public class Counting_Slope_Trees
    {
        [Test]
        [TestCase("sample", 7)]
        [TestCase("input", 187)]
        public void For_Puzzle_Output_Finds_N_Trees(string path, int expectedTrees)
        {
            var trees = CountTreesOnSlope(input, 3, 1);

            Assert.AreEqual(expectedTrees, trees);
        }

        [Test]
        [TestCase("sample", 336)]
        [TestCase("input", 4723283400)]
        public void For_Many_Slopes_Find_Product(string path, long expectedProduct)
        {
            long a = CountTreesOnSlope(input, 1, 1);
            long b = CountTreesOnSlope(input, 3, 1);
            long c = CountTreesOnSlope(input, 5, 1);
            long d = CountTreesOnSlope(input, 7, 1);
            long e = CountTreesOnSlope(input, 1, 2);
            var product = a * b * c * d * e;

            Assert.AreEqual(expectedProduct, product);
        }

        private static int CountTreesOnSlope(string[] input, int right, int down)
        {
            return input
                .Where((x, i) => i % down == 0)
                .Select(TreeOnSlope(right))
                .Sum();
        }

        private static Func<string, int, int> TreeOnSlope(int right)
        {
            return (x, i) => TreeOnSlope(x, i, right);
        }

        private static int TreeOnSlope(string x, int i, int right)
        {
            return x[i * right % x.Length] == '#' ? 1 : 0;
        }

        string[] input;

        [SetUp]
        public void Initialize_Input()
        {
            input = Resources.GetResourceLines(this, $"day3.{TestContext.CurrentContext.Test.Arguments[0]}.txt");
        }
    }
}