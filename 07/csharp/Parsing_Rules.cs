using AoCSharp.Common;
using ApprovalTests;
using ApprovalTests.Reporters;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;
using static System.StringSplitOptions;
using static Newtonsoft.Json.JsonConvert;

namespace csharp
{
    [UseReporter(typeof(VisualStudioReporter))]
    public class Parsing_Rules
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Parses_Rule_With_No_Content()
        {
            var input = Resources.GetResourceLines(this, "day7.sample.txt");

            var rawRules = input.Skip(7).Take(1).ToArray();
            var rules = ParseRules(rawRules);

            Approvals.VerifyJson(SerializeObject(rules));
        }

        [Test]
        public void Parses_Sets_Of_Rules()
        {
            var input = Resources.GetResourceLines(this, "day7.sample.txt");
            var rules = ParseRules(input);

            Approvals.VerifyJson(SerializeObject(rules));
        }

        [Test]
        [TestCase("sample", 32)]
        [TestCase("anothersample", 126)]
        [TestCase("input", 14177)]
        public void Counts_Mandatory_Bags(string inputFile, int expected)
        {
            var input = Resources.GetResourceLines(this, $"day7.{inputFile}.txt");
            var rules = ParseRules(input).ToDictionary(x => x.Container.Color, x => x.Contents);

            const string omniContainer = "shiny gold";
            var container = rules[omniContainer];
            var totalContained = container.CalculateBagCount(rules);

            Assert.That(totalContained, Is.EqualTo(expected));
        }

        private static List<BagRule> ParseRules(string[] rawRules)
        {
            return new SimpleParser(rawRules).Parse();
        }
    }

    public class SimpleParser
    {
        private readonly string[] input;

        public SimpleParser(string[] input)
        {
            this.input = input;
        }

        public List<BagRule> Parse()
        {
            return input.Select(Parse).ToList();
        }

        private BagRule Parse(string ruleText)
        {
            var ruleParts = ruleText.Split("contain");
            var bagPart = ruleParts[0].Split(' ', RemoveEmptyEntries);
            var unparsedContents = ruleParts[1].Split(new[] { ',', '.' }, RemoveEmptyEntries);
            var bag = new Bag(bagPart[0] + " " + bagPart[1]);
            var contents = unparsedContents.Select(contentText =>
            {
                var contentParts = contentText.Split(' ', RemoveEmptyEntries);
                if (contentParts[0] == "no")
                {
                    return new ContentRule(0, null);
                }

                return new ContentRule(Convert.ToInt32(contentParts[0]),
                    new Bag(contentParts[1] + " " + contentParts[2]));
            })
            .Where(x => x.Bag != null)
            .ToArray();
            return new BagRule(bag, new ContentSet(contents));
        }
    }

    public record Bag(string Color);
    public record ContentRule(int Amount, Bag Bag);
    public record ContentSet(ContentRule[] Rules)
    {
        public int CalculateBagCount(Dictionary<string, ContentSet> rules)
        {
            return Rules.Select(rule =>
                {
                    var count = rule.Amount + rule.Amount * rules[rule.Bag.Color].CalculateBagCount(rules);
                    return count;
                }
            ).Sum();
        }
    }

    public record BagRule(Bag Container, ContentSet Contents);
}