using System;
using System.Diagnostics;
using System.Linq;
using AoCSharp.Common;
using NUnit.Framework;

namespace day5
{
    public class Parsing_Boarding_Passes
    {
        [Test]
        [TestCase("FBFBBFFRLR", 357)]
        [TestCase("BFFFBBFRRR", 567)]
        [TestCase("FFFBBBFRRR", 119)]
        [TestCase("BBFFBBFRLL", 820)]
        public void Yields_Seat_Numbers(string boardingpass, int expectedSeat)
        {
            var seat = SeatNumber(boardingpass);

            Assert.That(seat, Is.EqualTo(expectedSeat));
        }

        [Test]
        public void Finds_Highest_Reserved_Seat_Number()
        {
            var maxSeat = input
                .Select(SeatNumber)
                .Max();

            Assert.That(maxSeat, Is.EqualTo(0));
        }

        [Test]
        public void Finds_Last_Free_Seat()
        {
            var ordered = input
                .Select(SeatNumber)
                .OrderBy(x => x)
                .ToArray();

            var seat = ordered
                .Where((x, i) => ordered[i + 1] > x + 1)
                .First() + 1;

            Assert.That(seat, Is.EqualTo(5));
        }

        private static int SeatNumber(string boardingpass)
        {
            var row = FindPosition(boardingpass.Substring(0, 7), 'F');
            var column = FindPosition(boardingpass.Substring(7, 3), 'L');
            var seat = row * 8 + column;
            return seat;
        }

        private static int FindPosition(string instructions, char lower)
        {
            var length = instructions.Length;
            var size = (int)Math.Pow(2, length);
            var range = (l: 0, u: size);
            for (int i = 0; i < length; i++, size /= 2)
            {
                range = instructions[i] switch
                {
                    'F' or 'L' => (range.l, range.u - size / 2),
                    _ => (range.l + size / 2, range.u)
                };
            }
            return range.l;
        }

        private string[] input;

        [SetUp]
        public void Setup()
        {
            input = Resources.GetResourceLines(this, "day5.input.txt");
        }
    }
}