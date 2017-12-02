using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Runner
{
    class Day03 :  Day
    {
        public override string First(string input)
        {
            var lines = GetLines(input);
            int total = 0;
            foreach (var line in lines)
            {
                var sides = line.Split((char[])null,StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToArray();
                total += (sides[0] + sides[1] > sides[2])
                    && (sides[1] + sides[2] > sides[0])
                    && (sides[0] + sides[2] > sides[1]) 
                    ? 1 : 0;


            }

            return total.ToString();
        }

        public override string Second(string input)
        {
            throw new NotImplementedException();
        }
    }
}
