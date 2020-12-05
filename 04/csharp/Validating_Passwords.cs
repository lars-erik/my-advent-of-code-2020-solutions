using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using NUnit.Framework;
using static System.Convert;
using static System.Environment;
using static AoCSharp.Common.Resources;

namespace day4
{
    public class Validating_Passwords
    {
        [Test]
        [TestCase("sample", 2)]
        [TestCase("input", 196)]
        public void With_NorthPole_Special_Requirements(string inputName, int expectedValid)
        {
            var requiredKeys = new[]
            {
                "byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid"
            };

            var valid = ParsePassports()
                .Count(x => requiredKeys.All(x.ContainsKey))
                ;

            Assert.That(valid, Is.EqualTo(expectedValid));
        }

        [Test]
        [TestCase("input", 114)]
        public void With_NorthPole_Special_Validated_Rules(string inputName, int expectedValid)
        {
            var requiredKeys = new[]
            {
                "byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid"
            };
            var nineDigit = new Regex(@"^\d{9}$");

            var valid = ParsePassports()
                .Where(x => requiredKeys.All(x.ContainsKey))
                .Count(x =>
                {
                    return ToInt32(x["byr"]) >= 1920 && ToInt32(x["byr"]) <= 2002 &&
                           ToInt32(x["iyr"]) >= 2010 && ToInt32(x["iyr"]) <= 2020 &&
                           ToInt32(x["eyr"]) >= 2020 && ToInt32(x["eyr"]) <= 2030 &&
                           ValidateHeight(x["hgt"]) &&
                           ValidateHairColor(x["hcl"]) &&
                           ValidateEyeColor(x["ecl"]) &&
                           nineDigit.IsMatch(x["pid"]);
                })
                ;

            Assert.That(valid, Is.EqualTo(expectedValid));
        }

        private bool ValidateEyeColor(string s)
        {
            var keys = new[] {"amb", "blu", "brn", "gry", "grn", "hzl", "oth"};
            return keys.Contains(s);
        }

        private bool ValidateHairColor(string s)
        {
            Regex ex = new Regex(@"^#[0-9a-f]{6}$");
            return ex.IsMatch(s);
        }

        private bool ValidateHeight(string s)
        {
            Regex ex = new Regex(@"(\d+)(cm|in)");
            var match = ex.Match(s);
            switch (match.Groups[2].Value)
            {
                case "cm": return ToInt32(match.Groups[1].Value) >= 150 && ToInt32(match.Groups[1].Value) <= 193;
                case "in": return ToInt32(match.Groups[1].Value) >= 59 && ToInt32(match.Groups[1].Value) <= 76;
            }

            return false;
        }

        private IEnumerable<Dictionary<string, string>> ParsePassports()
        {
            return input
                .Select(x =>
                    x.Replace(NewLine, " ")
                        .Split(" ")
                        .Select(e => e.Split(':'))
                        .ToDictionary(e => e[0], e => e[1])
                );
        }

        string[] input;

        [SetUp]
        public void Initialize_Input()
        {
            input = GetResourceLines(this, $"day4.{TestContext.CurrentContext.Test.Arguments[0]}.txt", NewLine + NewLine);
        }
    }
}