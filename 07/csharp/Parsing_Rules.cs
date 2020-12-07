using AoCSharp.Common;
using ApprovalTests;
using ApprovalTests.Reporters;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;
using static System.String;
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
        public void Tokenizes_Rules()
        {
            var input = Resources.GetResourceLines(this, "day7.sample.txt");

            var tokens = TokenizeRules(input.Take(1).ToArray());

            Approvals.VerifyJson(SerializeObject(tokens));
        }

        [Test]
        public void Parses_Tokenized_Rules()
        {
            var input = Resources.GetResourceLines(this, "day7.sample.txt");

            var parser = new Parser(TokenizeRules(input.Take(1).ToArray())[0]);
            var rules = parser.Parse();

            Approvals.VerifyJson(SerializeObject(rules));
        }

        [Test]
        public void Parses_Rule_With_No_Content()
        {
            var input = Resources.GetResourceLines(this, "day7.sample.txt");

            var parser = new Parser(TokenizeRules(input.Skip(7).Take(1).ToArray())[0]);
            var rules = parser.Parse();

            Approvals.VerifyJson(SerializeObject(rules));
        }

        [Test]
        public void Parses_Sets_Of_Rules()
        {
            var input = Resources.GetResourceLines(this, "day7.sample.txt");
            var rules = input.Select(TokenizeRule).Select(ParseRule);

            Approvals.VerifyJson(SerializeObject(rules));
        }

        [Test]
        [TestCase("sample", 32)]
        [TestCase("anothersample", 126)]
        [TestCase("input", -1)]
        public void Counts_Mandatory_Bags(string inputFile, int expected)
        {
            var input = Resources.GetResourceLines(this, $"day7.{inputFile}.txt");
            var rules = input.Select(TokenizeRule).Select(ParseRule).OfType<BagRule>().ToDictionary(x => x.Container.Color, x => x.Contents);

            const string omniContainer = "shiny gold";
            var container = rules[omniContainer];
            var totalContained = container.CalculateBagCount(1, rules);
            
            Assert.That(totalContained, Is.EqualTo(expected));
        }

        private static Node ParseRule(List<Token> tokenizedRule)
        {
            return new Parser(tokenizedRule).Parse();
        }

        private static List<List<Token>> TokenizeRules(string[] input)
        {
            return input.Select(TokenizeRule).ToList();
        }

        private static List<Token> TokenizeRule(string x)
        {
            return new Tokenizer(x).Tokenize();
        }
    }


    public record Token();
    public record Separator() : Token;
    public record Literal(string Value) : Token;
    public record Declarator(string Type) : Token;
    public record Operator(string Op) : Token;
    public record Number(int Value) : Token;
    public record ErrorToken(int Index, string Message) : Token;
    public record TokenizeResult(int Consumed, Token Token);

    public record Node();
    public record Bag(string Color) : Node;
    public record ContentRule(int Amount, Bag Bag) : Node;
    public record ContentSet(ContentRule[] Rules) : Node
    {
        public int CalculateBagCount(int multiplier, Dictionary<string, ContentSet> rules)
        {
            return Rules.Select(rule =>
                {
                    var count = rule.Amount + rule.Amount * rules[rule.Bag.Color].CalculateBagCount(rule.Amount, rules);
                    return count;
                }
            ).Sum();
        }
    }

    public record BagRule(Bag Container, ContentSet Contents) : Node;
    public record ErrorNode(int Index, string Message) : Node;

    public record ParseResult(int Consumed, Node Node);

    public class Parser
    {
        private readonly List<Token> tokens;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        public Node Parse()
        {
            return Rule(tokens).Node;
        }

        private ParseResult Rule(IEnumerable<Token> arg)
        {
            if (arg.Skip(3).FirstOrDefault() is Operator)
            {
                var bagResult = Bag(0, arg);
                var contentResult = Contents(4, arg.Skip(4));
                if (bagResult.Node is ErrorNode)
                    return bagResult;
                if (contentResult.Node is ErrorNode)
                    return contentResult;
                return new ParseResult(
                    arg.Count(),
                    new BagRule(
                        bagResult.Node as Bag,
                        contentResult.Node as ContentSet
                    )
                );
            }

            return new ParseResult(arg.Count(), new ErrorNode(0, "Tree is not a rule"));
        }

        private ParseResult Bag(int i, IEnumerable<Token> tokens)
        {
            if (tokens.Skip(2).FirstOrDefault() is Declarator)
            {
                return new ParseResult(
                    3,
                    new Bag(
                        Join(" ", tokens.Cast<Literal>().Select(x => x.Value).Take(2))
                    )
                );
            }

            return new ParseResult(
                tokens.Count(),
                new ErrorNode(i, $"{i}: {tokens.FirstOrDefault()} is not a bag.")
                );
        }

        private ParseResult Contents(int startIndex, IEnumerable<Token> tokens)
        {
            var rest = tokens.ToArray();
            List<ContentRule> rules = new List<ContentRule>();
            for (var i = 0; i < rest.Length; i++)
            {
                if (rest[i] is Separator)
                {
                    continue;
                }
                
                var result = ContentRule(i, rest);
                if (result.Node is ContentRule contentRule)
                {
                    if (contentRule.Bag != null)
                    { 
                        rules.Add(result.Node as ContentRule);
                    }
                    i += result.Consumed;
                }
                else
                {
                    return new ParseResult(rest.Length, new ErrorNode(startIndex + i, "Expected separator or content rule"));
                }
            }

            return new ParseResult(rest.Length, new ContentSet(rules.ToArray()));
        }

        private ParseResult ContentRule(int i, Token[] rest)
        {
            if (rest[i] is Literal {Value: "no"})
            {
                return new ParseResult(rest.Length, new ContentRule(0, null));
            }
            
            var amtResult = rest[i] as Number;
            if (amtResult == null)
            {
                return new ParseResult(rest.Length, new ErrorNode(i, "Expected amount"));
            }

            var bagResult = Bag(i + 1, rest.Skip(i + 1));
            if (bagResult.Node is ErrorNode)
            {
                return new ParseResult(rest.Length, new ErrorNode(i, "Expected bag"));
            }

            return new ParseResult(4, new ContentRule(amtResult.Value, (Bag) bagResult.Node));
        }
    }

    public class Tokenizer
    {
        private readonly string ruleText;
        private int index;
        private Func<TokenizeResult>[] tokenizers;

        public Tokenizer(string ruleText)
        {
            this.ruleText = ruleText;
            index = 0;

            tokenizers = new Func<TokenizeResult>[]
            {
                WhiteSpace,
                Number,
                Separator,
                Declarator,
                Operator,
                Literal
            };
        }

        public List<Token> Tokenize()
        {
            var tokens = new List<Token>();
            while (index < ruleText.Length)
            {
                var tokenized = false;
                foreach (var tokenizer in tokenizers)
                {
                    var result = tokenizer();
                    if (result.Consumed > 0)
                    {
                        index += result.Consumed;
                        if (result.Token != null)
                        {
                            tokens.Add(result.Token);
                        }

                        tokenized = true;
                        break;
                    }
                }

                if (!tokenized)
                {
                    tokens.Add(new ErrorToken(index, $"Dunno what to do at {index} {ruleText[index]}"));
                    return tokens;
                }
            }

            return tokens;
        }

        private TokenizeResult WhiteSpace()
        {
            var i = index;
            if (IsWhiteSpace(i))
                return new TokenizeResult(1, null);

            return new TokenizeResult(0, null);
        }

        private TokenizeResult Separator()
        {
            if (ruleText[index] == ',')
                return new TokenizeResult(1, new Separator());

            return new TokenizeResult(0, null);
        }

        private TokenizeResult Declarator()
        {
            var (consumed, token) = Literal();
            if (
                consumed > 0 &&
                token is Literal lit &&
                (
                    lit.Value == "bag" ||
                    lit.Value == "bags"
                )
            )
            {
                return new TokenizeResult(consumed, new Declarator(lit.Value));
            }

            return new TokenizeResult(0, null);
        }

        private TokenizeResult Operator()
        {
            var (consumed, token) = Literal();
            if (
                consumed > 0 &&
                token is Literal lit &&
                lit.Value == "contain"
            )
            {
                return new TokenizeResult(consumed, new Operator(lit.Value));
            }

            return new TokenizeResult(0, null);
        }

        private TokenizeResult Number()
        {
            if (IsDigit(index))
            {
                var current = "";
                for (var i = index; i < ruleText.Length; i++)
                {
                    current += ruleText[i];
                    if (IsEoF(i) || !IsDigit(i + 1))
                    {
                        return new TokenizeResult(i - index + 1, new Number(Convert.ToInt32(current)));
                    }
                }
            }
            return new TokenizeResult(0, null);
        }

        private TokenizeResult Literal()
        {
            if (IsLetter(index))
            {
                var current = "";
                for (var i = index; i < ruleText.Length; i++)
                {
                    current += ruleText[i];
                    if (IsEoF(i) || !IsLetter(i + 1))
                    {
                        return new TokenizeResult(i - index + 1, new Literal(current));
                    }
                }
            }
            return new TokenizeResult(0, null);
        }

        private bool IsEoF(int i)
        {
            return i == ruleText.Length - 1;
        }

        private bool IsWhiteSpace(int i)
        {
            return ruleText[i] == ' ' || ruleText[i] == '.';
        }

        private bool IsLetter(int i)
        {
            return ruleText[i] >= 'a' && ruleText[i] <= 'z';
        }

        private bool IsDigit(int i)
        {
            return ruleText[i] >= '0' && ruleText[i] <= '9';
        }
    }
}