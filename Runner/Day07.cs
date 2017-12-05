using System;
using System.Collections.Generic;
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

        public override string Second(string input)
        {
            throw new NotImplementedException();
        }
    }
}
