using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Runner
{
    class Day04 :  Day
    {
        private Regex itemRegex = new Regex(@"([a-z\-]*)-([0-9]*)\[([a-z]*)\]");

        public class Items
        {
            public string Name { get; set; }
            public int Sector { get; set; }
            public string Checksum { get; set; }
        }
        public override string First(string input)
        {
            return GetLines(input)
                .Select(l => GetItems(l))
                .Where(i => i.Checksum == FirstChecksum(i.Name))
                .Sum(i => i.Sector)
                .ToString();
        }

        public override string FirstTest(string input)
        {
            var items = GetItems(input);
            return ((FirstChecksum(items.Name) == items.Checksum) ? 1 : 0).ToString();
        }

        public override string SecondTest(string input)
        {
            var items = GetItems(input);
            return Decrypt(items);
        }

        private string Decrypt(Items items)
        {
            return string.Join("", items.Name.ToCharArray().Select(c => DecryptChar(c, items.Sector)));
        }

        int a = (int)'a';
        private char DecryptChar(char c, int sector)
        {
            if (c == '-') return ' ';
            return (char)(((((int)c)-a+sector)%26)+a);
        }

        public string FirstChecksum(string name)
        {
            var nameChars = new List<char>(name.Replace("-", "").ToCharArray());
            var distinctOrderedChars = nameChars.Distinct().OrderBy(c=>c);
            var result = string.Join("", distinctOrderedChars
                .Select(c => new { c = c, count = nameChars.Count(n => n == c) })
                .OrderByDescending(x => x.count)
                .Select(x=>x.c)
                .Take(5));
            return result;
        }

        private Items GetItems(string input)
        {
            var match = itemRegex.Match(input);
            return new Items()
            {
                Name = match.Groups[1].Value,
                Sector = int.Parse(match.Groups[2].Value),
                Checksum = match.Groups[3].Value
            };
        }

        public override string Second(string input)
        {
            //return string.Join(Environment.NewLine, GetLines(input)
            //    .Select(l => GetItems(l))
            //    .Where(i => i.Checksum == FirstChecksum(i.Name))
            //    .Select(i=>Decrypt(i)))
            //    ;

            return GetLines(input)
                .Select(l => GetItems(l))
                .Where(i => i.Checksum == FirstChecksum(i.Name))
                .Select(i => new { n = Decrypt(i), s = i.Sector })
                .First(x => x.n.Contains("north")).s.ToString();

        }
    }
}
