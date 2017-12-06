using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Runner
{
    class Day07 :  Day
    {
        Regex ipv7RE = new Regex(@"([a-z]+)(?:\[([a-z]+)\])?");

        public override string First(string input)
        {
            return GetLines(input).Count(l => SupportsTLS(l)).ToString();
        }

        public override string FirstTest(string input)
        {
            return SupportsTLS(input) ? "1" : "0";
        }

        bool SupportsTLS(string input)
        {
            bool haveABBA = false;

            var matches = ipv7RE.Matches(input);


            foreach (Match match in matches)
            {
                if (HasABBA(match.Groups[1].Value))
                {
                    haveABBA = true;
                }
                var hyperNet = match.Groups[2].Value;
                if (!string.IsNullOrEmpty(hyperNet) && HasABBA(hyperNet)) return false;
            }
            return haveABBA;
            
        }

        bool HasABBA(string input)
        {
            var length = input.Length;
            if (length < 4) return false;
            for (int i = 0; i < length-3; i++)
            {
                if (IsABBA(input.Substring(i, 4))) return true;
            }
            return false;
        }

        bool IsABBA(string input)
        {
            if (input.Length != 4) return false;
            return (input[0] == input[3] && input[1] == input[2] && input[0] != input[1]);
        }

        bool SupportsSSL(string input)
        {
            var matches = ipv7RE.Matches(input);

            var hyperNets = matches.Select(m=>m.Groups[2].Value).Where(h => !string.IsNullOrWhiteSpace(h));

            var otherNets = matches.Select(m => m.Groups[1].Value);

            return (otherNets.SelectMany(n => GetABAs(n))).Any(a => hyperNets.Any(h => h.Contains(ToBAB(a))));
        }

        string[] GetABAs(string input)
        {
            var length = input.Length;
            if (length < 3) return new string[]{};
            return Enumerable.Range(0, length - 2).Select(i=>input.Substring(i, 3)).Where(x => IsABA(x)).ToArray();
        }

        bool IsABA(string input)
        {
            if (input.Length != 3) return false;
            return (input[0] == input[2] && input[0] != input[1]);
        }

        string ToBAB(string aba)
        {
            return string.Join("", new Char[] {aba[1], aba[0], aba[1]});
        }

        public override string Second(string input)
        {
            return GetLines(input).Count(l => SupportsSSL(l)).ToString();
        }

        public override string SecondTest(string input)
        {
            return SupportsSSL(input) ? "1" : "0";
        }
    }
}
