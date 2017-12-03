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
                total += IsTriangle(sides[0],sides[1],sides[2]) ? 1 : 0;
            }

            return total.ToString();
        }

        public bool IsTriangle(int x, int y, int z)
        {
            return (x + y > z) && (y + z > x) && (x + z > y);
        }

        public override string Second(string input)
        {
            var lines = GetLines(input);
            int total = 0;
            for (int i = 0; i < lines.Length; i=i+3)
            {
                var a = new int[3];
                var b = new int[3];
                var c = new int[3];
                for (int j = 0; j < 3; j++)
                {
                    var sides = lines[i+j].Split((char[])null, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToArray();
                    a[j] = sides[0];
                    b[j] = sides[1];
                    c[j] = sides[2];
                }
                total += IsTriangle(a[0], a[1], a[2]) ? 1 : 0;
                total += IsTriangle(b[0], b[1], b[2]) ? 1 : 0;
                total += IsTriangle(c[0], c[1], c[2]) ? 1 : 0;
            }
            return total.ToString();
        }
    }
}
