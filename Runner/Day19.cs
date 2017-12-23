using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    class Day19 : Day
    {
        //public class Elf
        //{
        //    public int Seat { get; set; }
        //    public int Presents { get; set; }
        //}

        //public override string First(string input)
        //{
        //    int elfCount = int.Parse(input);
        //    var elves = Enumerable.Range(1, elfCount).Select(i => new Elf() { Seat = i, Presents = 1 }).ToArray();
        //    do
        //    {
        //        var i = 0;
        //        while (i < elfCount - 1)
        //        {
        //            Steal(elves, i, i + 1);
        //            i += 2;
        //        }
        //        if (i == elfCount - 1)
        //        {
        //            Steal(elves, i, 0);
        //        }
        //        elves = elves.Where(e => e.Presents > 0).ToArray();
        //        elfCount = elves.Length;

        //    }
        //    while (elves.Count() > 1);

        //    return elves.First().Seat.ToString();
        //}

        //private static void Steal(Elf[] elves, int taker, int from)
        //{
        //    var elf = elves[taker];
        //    var nextElf = elves[from];
        //    elves[taker].Presents += elves[from].Presents;
        //    elves[from].Presents = 0;
        //}

        public class Elf
        {
            public int Seat { get; set; }
            public bool Presents { get; set; }
        }

        private static void GameElves(int elfCount, Dictionary<int, Elf> elves, Func<Dictionary<int,Elf>,int,int,int> stealFromFn)
        {
            int maxElf = elfCount;
            do
            {
                var current = 0;
                while (current < maxElf && elves.Keys.Count() > 1)
                {
                    current = GetNextElf(elves, current + 1, maxElf);
                    var stealFrom = stealFromFn(elves, current, maxElf);
                    Steal(elves, current, stealFrom);
                }
                maxElf = elves.Keys.Max();
            }
            while (elves.Keys.Count() > 1);
        }

        private static int GetNextElf(Dictionary<int, Elf> elves, int i, int maxElf)
        {
            if (!elves.Any()) throw new InvalidOperationException();
            while (!elves.ContainsKey(i))
            {
                i++;
                if (i > maxElf) i = 1;
            }
            return i;
        }

        private static void Steal(Dictionary<int, Elf> elves, int taker, int from)
        {
            var elf = elves[taker];
            var nextElf = elves[from];
            elves.Remove(from);
        }

        //private static int GetOppositeElf(Dictionary<int, Elf> elves, int current, int maxElf)
        //{
        //    int[] seatIndexes = elves.Keys.OrderBy(seat => seat).ToArray();
        //    var currentIndex = Array.IndexOf(seatIndexes, current);
        //    var oppositeIndex = (currentIndex + (seatIndexes.Length / 2)) % seatIndexes.Length;
        //    return seatIndexes[oppositeIndex];
        //}

        private static int GetOppositeElf(Dictionary<int, Elf> elves, int current, int maxElf)
        {
            var skipForward = elves.Count() / 2;
            var next = current;
            for (int i = 0; i < skipForward; i++)
            {
                next = GetNextElf(elves, next + 1, maxElf);
            }
            return next;
        }

        public override string First(string input)
        {
            int elfCount = int.Parse(input);
            var elves = Enumerable.Range(1, elfCount).Select(i => new Elf() { Seat = i, Presents = true }).ToDictionary(e => e.Seat);
            GameElves(elfCount, elves, (e, current, maxElf) => GetNextElf(e, current + 1, maxElf));

            return elves.Values.First().Seat.ToString();
        }

        public override string Second(string input)
        {
            int elfCount = int.Parse(input);
            var elves = Enumerable.Range(1, elfCount).Select(i => new Elf() { Seat = i, Presents = true }).ToDictionary(e => e.Seat);
            GameElves(elfCount, elves, (e, current, maxElf) => GetOppositeElf(e, current, maxElf));

            return elves.Values.First().Seat.ToString();
        }
    }
}
