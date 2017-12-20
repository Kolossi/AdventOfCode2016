using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    class Day16 : Day
    {
        public static string DragonIt(string a)
        {
            var bChars = a.ToCharArray().Select(c => c == '0' ? '1' : '0').ToArray();
            Array.Reverse(bChars);

            return string.Format("{0}0{1}", a, string.Join("", bChars));
        }

        public static string GetData(string a, int length)
        {
            do
            {
                a = DragonIt(a);
            } while (a.Length < length);
            return a.Substring(0,length);
        }

        public static string GetChecksum(string a)
        {
            do
            {
                var checksum = new List<char>();

                for (int i = 0; i < a.Length; i += 2)
                {
                    var c1 = a[i];
                    var c2 = a[i + 1];
                    checksum.Add(c1 == c2 ? '1' : '0');
                }
                a = string.Join("", checksum);
            } while (a.Length % 2 == 0);
            return a;
        }

        public override string First(string input)
        {
            var parts = GetParts(input);
            var a = parts[0];
            var length = int.Parse(parts[1]);
            var data = GetData(a, length);
            return GetChecksum(data);
        }

        public override string Second(string input)
        {
            return First(input);
        }
    }
}
