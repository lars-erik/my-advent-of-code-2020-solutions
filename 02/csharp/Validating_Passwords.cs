using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace day2 {
    [TestFixture]
    public class Validating_Passwords
    {
        [Test]
        [TestCase("1-3 a: abcde", true)]
        [TestCase("1-3 b: cdefg", false)]
        [TestCase("2-9 c: ccccccccc", true)]
        public void Enforces_Policy(string input, bool expectedValidity)
        {
            bool isValid = Validate(input);
            Assert.That(isValid, Is.EqualTo(expectedValidity));
        }

        string[] input;

        [SetUp]
        public void Initialize_Input()
        {
            using var inputStream = GetType()
                .Assembly
                .GetManifestResourceStream("day2.input.txt");
            input = new StreamReader(inputStream)
                .ReadToEnd()
                .Split(Environment.NewLine)
                .ToArray();
        }

        [Test]
        public void Counts_Valid_Passwords()
        {
            var valid = input
                .Select(Validate)
                .Count(x => x);
            Assert.That(valid, Is.EqualTo(524));
        }

        public static bool Validate(string input)
        {
            var set = input.Split(':').Select(x => x.Trim()).ToArray();
            var ruleSet = set[0].Split(' ');
            var rule = (
                range: ruleSet[0].Split('-').Select(x => Convert.ToInt32(x)).ToArray(),
                chr: ruleSet[1][0]
            );
            bool isValid = SledRentalPolicy(rule, set[1]);
            return isValid;
        }

        private static bool SledRentalPolicy((int[] range, char chr) rule, string password)
        {
            var chars = password.Where(x => x == rule.chr).Count();
            var isValid = chars >= rule.range[0] && chars <= rule.range[1];
            return isValid;
        }
    }
}