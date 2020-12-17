using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AoCSharp.Common;
using NUnit.Framework;

namespace day17
{
    record Coord(int X, int Y, int Z);
    
    public class Evolving_Conway_Cubes
    {
        int minZ = 0,
            minX = 0,
            minY = 0,
            maxZ = 0,
            maxX = 2,
            maxY = 2;

        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void At_Cycle_6_Has_N_Active_Sample()
        {
            var input = new[]
            {
                ".#.",
                "..#",
                "###"
            };

            minZ = 0;
            minX = 0;
            minY = 0;
            maxZ = 0;
            maxX = 2;
            maxY = 2;

            var coords = InitialBoard(input);
            var output = CreateOutput(0, coords);
            Console.Write(output);

            for (var turn = 1; turn <= 6; turn++)
            {
                var nextSet = ExecuteTurn(coords);
                coords = nextSet;

                output = CreateOutput(turn, coords);
                Console.Write(output);
                
            }
            
            Assert.AreEqual(112, coords.Count);
        }

        [Test]
        public void At_Cycle_6_Has_N_Active_Input()
        {
            var input = Resources.GetResourceLines(this, "day17.input.txt");

            minZ = 0;
            minX = 0;
            minY = 0;
            maxZ = 0;
            maxX = 8;
            maxY = 8;

            var coords = InitialBoard(input);

            for (var turn = 1; turn <= 6; turn++)
            {
                var nextSet = ExecuteTurn(coords);
                coords = nextSet;
            }
            
            Assert.AreEqual(286, coords.Count);
        }

        private string CreateOutput(int turn, HashSet<Coord> coords)
        {
            var output = $"Turn {turn}\n";
            for (var z = minZ; z <= maxZ; z++)
            {
                output += $"Z: {z}\n";

                for (var y = minY; y <= maxY; y++)
                {
                    for (var x = minX; x <= maxX; x++)
                    {
                        output += coords.Contains(new Coord(x, y, z)) ? "#" : ".";
                    }

                    output += "\n";
                }

                output += "\n";
            }

            return output;
        }


        private HashSet<Coord> ExecuteTurn(HashSet<Coord> coords)
        {
            var nextSet = new HashSet<Coord>();
            for (var z = minZ - 1; z <= maxZ + 1; z++)
            {
                for (var y = minY - 1; y <= maxY + 1; y++)
                {
                    for (var x = minX - 1; x <= maxX + 1; x++)
                    {
                        var coord = new Coord(x, y, z);
                        var neighbours = CountNeighbors(coords, coord);
                        var isAlive = coords.Contains(coord);
                        if (
                            (isAlive && (neighbours == 2 || neighbours == 3)) || // stays alive
                            (!isAlive && neighbours == 3)
                        )
                        {
                            nextSet.Add(coord);
                            
                            minX = Math.Min(minX, x);
                            minY = Math.Min(minY, y);
                            minZ = Math.Min(minZ, z);
                            maxX = Math.Max(maxX, x);
                            maxY = Math.Max(maxY, y);
                            maxZ = Math.Max(maxZ, z);
                        }
                    }
                }
            }

            return nextSet;
        }

        private int CountNeighbors(HashSet<Coord> coords, Coord coord)
        {
            var neighbors = 0;
            for (var z = coord.Z - 1; z <= coord.Z + 1; z++)
            {
                for (var y = coord.Y - 1; y <= coord.Y + 1; y++)
                {
                    for (var x = coord.X - 1; x <= coord.X + 1; x++)
                    {
                        var neighbor = new Coord(x, y, z);
                        if (neighbor != coord && coords.Contains(neighbor))
                        {
                            neighbors++;
                        }
                    }
                }
            }

            return neighbors;
        }

        private static HashSet<Coord> InitialBoard(string[] input)
        {
            HashSet<Coord> coords = new HashSet<Coord>();

            for (var y = 0; y < input.Length; y++)
            {
                for (var x = 0; x < input[y].Length; x++)
                {
                    if (input[y][x] == '#')
                    {
                        coords.Add(new Coord(x, y, 0));
                    }
                }
            }

            return coords;
        }
    }
}