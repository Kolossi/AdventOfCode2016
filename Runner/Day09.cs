using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Runner
{
    class Day09 : Day
    {
        public override string First(string input)
        {
            return Decompress(input).Length.ToString();
        }

        public override string FirstTest(string input)
        {
            return Decompress(input);
        }

        private string Decompress(string input)
        {
            var sb=new StringBuilder();
            int i=0;
            do
            {
                int bracket = input.IndexOf('(', i);
                if (bracket < 0)
                {
                    sb.Append(input.Substring(i, input.Length - i));
                    i = input.Length;
                }
                else
                {
                    sb.Append(input.Substring(i, bracket - i));
                    i = bracket + 1;
                    int endBracket = input.IndexOf(')', i);
                    var parts = input.Substring(i, endBracket - i).Split('x');
                    var chars = int.Parse(parts[0]);
                    var repeats = int.Parse(parts[1]);
                    sb.Append(string.Join("", Enumerable.Repeat(input.Substring(endBracket + 1, chars), repeats)));
                    i = endBracket + 1 + chars;
                }
            } while (i<input.Length);

            return sb.ToString();
        }

        public override string Second(string input)
        {
            return DecompressV2Length(input).ToString();
        }

        public long DecompressV2Length(string input)
        {
            long length = 0;
            int i = 0;
            do
            {
                int bracket = input.IndexOf('(', i);
                if (bracket < 0)
                {
                    length += input.Length - i;
                    i = input.Length;
                }
                else
                {
                    length += bracket - i;
                    i = bracket + 1;
                    int endBracket = input.IndexOf(')', i);
                    var parts = input.Substring(i, endBracket - i).Split('x');
                    var chars = int.Parse(parts[0]);
                    var repeats = int.Parse(parts[1]);
                    length += DecompressV2Length(input.Substring(endBracket + 1, chars)) * repeats;
                    i = endBracket + 1 + chars;
                }
            } while (i < input.Length);

            return length;
        }
    }
}
