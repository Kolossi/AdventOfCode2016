using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    class Day19 : Day
    {
        public class Elf
        {
            public int Seat { get; set; }
            public bool Presents { get; set; }
            public Elf Next { get; set; }
            public Elf Prev { get; set; }
        }

        public override string First(string input)
        {
            int elfCount = int.Parse(input);
            var elves = Enumerable.Range(1, elfCount).Select(i => new Elf() { Seat = i, Presents = true }).ToArray();
            do
            {
                var i = 0;
                while (i < elfCount - 1)
                {
                    Steal(elves, i, i + 1);
                    i += 2;
                }
                if (i == elfCount - 1)
                {
                    Steal(elves, i, 0);
                }
                elves = elves.Where(e => e.Presents).ToArray();
                elfCount = elves.Length;

            }
            while (elves.Count() > 1);

            return elves.First().Seat.ToString();
        }

        private static void Steal(Elf[] elves, int taker, int from)
        {
            elves[from].Presents = false;
        }

        private static void Delete(Elf elf)
        {
            elf.Prev.Next = elf.Next;
            elf.Next.Prev = elf.Prev;
        }

        public override string Second(string input)
        {
            int elfCount = int.Parse(input);
            var elves = Enumerable.Range(1, elfCount).Select(i => new Elf() { Seat = i, Presents = true }).ToArray();
            for (int i = 0; i < elfCount; i++)
            {
                elves[i].Next = elves[(i + 1) % elfCount];
                elves[i].Prev = elves[(elfCount + i - 1) % elfCount];
            }

            var elf = elves[0];
            var firstElf = elf;
            var stealFrom = elves[elfCount / 2];

            do
            {
                Delete(stealFrom);
                if (stealFrom == firstElf) firstElf = firstElf.Next;
                elfCount--;
                stealFrom = stealFrom.Next;
                if (elfCount % 2 == 0) stealFrom = stealFrom.Next;
                elf = elf.Next;
                
            }
            while (elfCount > 1);
            return elf.Seat.ToString();
        }
    }
}
