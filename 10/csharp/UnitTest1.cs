using System;
using System.Collections.Generic;
using System.Linq;
using AoCSharp.Common;
using Newtonsoft.Json;
using NUnit.Framework;

namespace day16
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            visited = new List<int>();
            reaches = new Dictionary<int, long>();
        }

        private List<int> visited;

        private Dictionary<int, long> reaches;
        
        private int CountNext(Dictionary<int, int[]> withEdges, int current)
        {
            return 1;
        }

        private Dictionary<int, List<List<int>>> pathsFromHere = new Dictionary<int, List<List<int>>>();
        
        private List<List<int>> CreatePaths(Dictionary<int, int[]> withEdges, int current, IEnumerable<int> pathToHere)
        {
            if (withEdges[current].Length == 0)
            {
                return new List<List<int>> { pathToHere.ToList() };
            }

            if (pathsFromHere.ContainsKey(current))
            {
                return pathsFromHere[current];
            }
            
            var paths = withEdges[current].SelectMany(x =>
            {
                return CreatePaths(withEdges, x, pathToHere.Union(new[]{current}));
            }).ToList();
            
            pathsFromHere.Add(current, paths);

            return paths;
        }
        
        [Test]
        public void Sample1_Count()
        {
            var sampleInput = Resources.GetResourceLines(this, "day10.sample.txt", ", ").Select(x => Convert.ToInt32(x)).OrderBy(x => x);
            var input = sampleInput;
            var answer = FindPaths(input);

            Console.WriteLine(JsonConvert.SerializeObject(reaches));
            Console.WriteLine(answer);
            //var allPaths = CountNext(withEdges, 0);
            //Console.WriteLine(allPaths);
        }

        private long FindPaths(IOrderedEnumerable<int> input)
        {
            var max = input.Max() + 3;
            var allJolts = new [] {0}.Union(input).Union(new[] {max}).ToArray();
            // for(var i = 0; i<allJolts.Length; i++) {
            //     untested.Where(x => x <= 3)
            // }
            var withEdges = allJolts.Select((x, i) =>
                {
                    var next = allJolts.Skip(i + 1).Take(3).Where(y => y <= x + 3).ToArray();
                    return (x, next);
                })
                .ToDictionary(x => x.x, x => x.next);

            reaches = allJolts.ToDictionary(x => x, x => (long)0);
            reaches[0] = 1;
            for (var i = 0; i < allJolts.Length; i++)
            {
                var curJolt = allJolts[i];
                foreach (var j in withEdges[curJolt])
                {
                    reaches[j] += reaches[curJolt];
                }
            }

            var answer = reaches[max];
            return answer;
        }

        [Test]
        public void Sample1()
        {
            var sampleInput = Resources.GetResourceLines(this, "day10.sample.txt", ", ").Select(x => Convert.ToInt32(x)).OrderBy(x => x);
            var allJolts = new[] { 0 }.Union(sampleInput).Union(new []{sampleInput.Max() + 3}).ToArray();
            // for(var i = 0; i<allJolts.Length; i++) {
            //     untested.Where(x => x <= 3)
            // }
            var withEdges = allJolts.Select((x, i) => {
                var next = allJolts.Skip(i + 1).Take(3).Where(y => y <= x + 3).ToArray();
                return (x, next);
            })
                .ToDictionary(x => x.x, x => x.next);

            var allPaths = CreatePaths(withEdges, 0, new List<int>());
            Console.WriteLine(String.Join(Environment.NewLine, allPaths.Select(x => String.Join(", ", x))));
        }
        
        [Test]
        public void Sample2_Count()
        {
            var sampleInput = Resources.GetResourceLines(this, "day10.sample2.txt").Select(x => Convert.ToInt32(x)).OrderBy(x => x);
            var allJolts = new[] { 0 }.Union(sampleInput).Union(new []{sampleInput.Max() + 3}).ToArray();
            // for(var i = 0; i<allJolts.Length; i++) {
            //     untested.Where(x => x <= 3)
            // }
            var withEdges = allJolts.Select((x, i) => {
                var next = allJolts.Skip(i + 1).Take(3).Where(y => y <= x + 3).ToArray();
                return (x, next);
            })
                .ToDictionary(x => x.x, x => x.next);

            var answer = FindPaths(sampleInput);
            Console.WriteLine(answer);
        }

        [Test]
        public void Sample2()
        {
            var sampleInput = Resources.GetResourceLines(this, "day10.sample2.txt").Select(x => Convert.ToInt32(x)).OrderBy(x => x);
            var allJolts = new[] { 0 }.Union(sampleInput).Union(new []{sampleInput.Max() + 3}).ToArray();
            // for(var i = 0; i<allJolts.Length; i++) {
            //     untested.Where(x => x <= 3)
            // }
            var withEdges = allJolts.Select((x, i) => {
                var next = allJolts.Skip(i + 1).Take(3).Where(y => y <= x + 3).ToArray();
                return (x, next);
            })
                .ToDictionary(x => x.x, x => x.next);

            var allPaths = CreatePaths(withEdges, 0, new List<int>());
            Console.WriteLine(allPaths.Count);
        }
        
        [Test]
        public void Input()
        {
            var sampleInput = Resources.GetResourceLines(this, "day10.input.txt").Select(x => Convert.ToInt32(x)).OrderBy(x => x);
            var allJolts = new[] { 0 }.Union(sampleInput).Union(new []{sampleInput.Max() + 3}).ToArray();
            // for(var i = 0; i<allJolts.Length; i++) {
            //     untested.Where(x => x <= 3)
            // }
            var withEdges = allJolts.Select((x, i) => {
                var next = allJolts.Skip(i + 1).Take(3).Where(y => y <= x + 3).ToArray();
                return (x, next);
            })
                .ToDictionary(x => x.x, x => x.next);

            var answer = FindPaths(sampleInput);
            Console.WriteLine(answer);
        }
    }
}