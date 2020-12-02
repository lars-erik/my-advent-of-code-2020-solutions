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
            bool isValid = Validate(input, SledRentalPolicy);
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
        public void Counts_Valid_SledRental_Passwords()
        {
            var valid = input
                .Select(ValidateSledRentalPolicy)
                .Count(x => x);

            Assert.That(valid, Is.EqualTo(524));
        }

        [Test]
        public void Counts_Valid_ToboganCorporate_Passwords()
        {
            var valid = input
                .Select(ValidateToboganCorporatePolicy)
                .Count(x => x);

            Assert.That(valid, Is.EqualTo(485));
        }

        private static bool ValidateSledRentalPolicy(string record)
        {
            return Validate(record, SledRentalPolicy);
        }

        private static bool ValidateToboganCorporatePolicy(string record)
        {
            return Validate(record, ToboganCorporatePolicy);
        }

        public static bool Validate(string input, Func<(int[] range, char chr), string, bool> policy)
        {
            var set = input.Split(':').Select(x => x.Trim()).ToArray();
            var ruleSet = set[0].Split(' ');
            var rule = (
                range: ruleSet[0].Split('-').Select(x => Convert.ToInt32(x)).ToArray(),
                chr: ruleSet[1][0]
            );
            bool isValid = policy(rule, set[1]);
            return isValid;
        }

        private static bool ToboganCorporatePolicy((int[] range, char chr) rule, string password)
        {
            var isValid = (password[rule.range[0] - 1] == rule.chr) 
                        ^ (password[rule.range[1] - 1] == rule.chr);
            return isValid;
        }
        private static bool SledRentalPolicy((int[] range, char chr) rule, string password)
        {
            var chars = password.Count(x => x == rule.chr);
            var isValid = chars >= rule.range[0] && chars <= rule.range[1];
            return isValid;
        }
    }
}