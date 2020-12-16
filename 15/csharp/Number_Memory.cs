using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace day15
{
    public class Number_Memory
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase("0,3,6", 436, 2020)]
        [TestCase("1,3,2", 1, 2020)]
        [TestCase("2,1,3", 10, 2020)]
        [TestCase("0,3,1,6,7,5", 852, 2020)]
        [TestCase("0,3,1,6,7,5", 6007666, 30000000)]
        public void Each_Round_Mentions_Age_Of_Previous_Number(string inputData, int expectedAnswer, int iterations)
        {
            var input = inputData
                .Split(",")
                .Select(x => Convert.ToInt32(x));
            var history = input.ToList();
            var data = history
                .Select((x, i) => (x: x, indexes: new List<int>{i}))
                .ToDictionary(x => x.x, x => x.indexes);
            
            for (var i = history.Count; i < iterations; i++)
            {
                var prevNum = history[i - 1];
                var indexes = data[prevNum];
                var diff = indexes.Count == 1 ? 0 : indexes[^1] - indexes[^2];
                history.Add(diff);
                if (data.ContainsKey(diff))
                {
                    data[diff].Add(i);
                }
                else
                {
                    data.Add(diff, new List<int>{i});
                }
            }
            
            //Console.WriteLine(JsonConvert.SerializeObject(history));
            //Console.WriteLine(JsonConvert.SerializeObject(data, Formatting.Indented));

            var answer = history.Last();
            
            Assert.AreEqual(expectedAnswer, answer);
        }
    }
}