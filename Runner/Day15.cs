using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    class Day15 : Day
    {
        class Disc
        {
            public int Positions { get; set; }
            public int StartPos { get; set; }
        }

        private static int FindDelay(Disc[] discs)
        {
            int delay = 0;
            do
            {
                bool caught = false;
                for (int time = 1; time <= discs.Length; time++)
                {
                    var disc = discs[time - 1];
                    var position = (disc.StartPos + time + delay) % disc.Positions;
                    if (position != 0)
                    {
                        caught = true;
                        break;
                    }
                }
                if (!caught) return delay;
                delay++;
            } while (true);
        }

        private Disc[] GetDiscs(string input)
        {
            var discs = new List<Disc>();
            foreach (var line in GetLines(input))
            {
                var parts = GetParts(line);
                discs.Add(new Disc()
                {
                    Positions = int.Parse(parts[3]),
                    StartPos = int.Parse(parts[11].Replace(".", ""))
                });
            }
            return discs.ToArray();
        }

        public override string First(string input)
        {
            var discs = GetDiscs(input);
            return FindDelay(discs).ToString();
        }

        public override string Second(string input)
        {
            var discs = GetDiscs(input);
            discs = (new List<Disc>(discs)).Union(new Disc[] { new Disc() { Positions = 11, StartPos = 0 } }).ToArray();
            return FindDelay(discs).ToString();
        }
    }
}
