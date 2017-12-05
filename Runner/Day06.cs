using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    class Day06 :  Day
    {
        public override string First(string input)
        {
            var lines = GetLines(input);
            var length = lines[0].Length;

            return string.Join("",
                Enumerable.Range(0, length)
                    .Select(i => GetMostCommonChar(lines.Select(l => l[i]))));
        }

        public char GetMostCommonChar(IEnumerable<char> chars)
        {
            return chars.Distinct()
                .Select(c => new { c = c, count = chars.Count(n => n == c) })
                .OrderByDescending(x => x.count)
                .Select(x => x.c)
                .First();
        }

        public char GetLeastCommonChar(IEnumerable<char> chars)
        {
            return chars.Distinct()
                .Select(c => new { c = c, count = chars.Count(n => n == c) })
                .OrderBy(x => x.count)
                .Select(x => x.c)
                .First();
        }

        public override string Second(string input)
        {
            var lines = GetLines(input);
            var length = lines[0].Length;

            return string.Join("",
                Enumerable.Range(0, length)
                    .Select(i => GetLeastCommonChar(lines.Select(l => l[i]))));
        }
    }
}
