using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Runner
{
    class Day14 : Day
    {
        public class Key
        {
            public string Hash { get; set; }
            public int Index { get; set; }
            public int QuintIndex { get; set; }
            public char TripletChar { get; set; }

            public override bool Equals(object obj)
            {
                return (obj as Key).Index == Index;
            }

            public override int GetHashCode()
            {
                return Index;
            }

            public override string ToString()
            {
                return string.Format("{0}-{1}:'{2}':{3}", Index, QuintIndex, TripletChar, Hash);
            }
        }

        public Regex HexTriplet = new Regex("([0-9a-f])\\1\\1");
        public Regex HexQuintet = new Regex("([0-9a-f])\\1\\1\\1\\1");

        public List<Key> GetKeys(string salt, int hashRepeat = 1)
        {
            int i = 0;
            int endIndex = int.MaxValue;
            var potentialKeys = new List<Key>();
            var keys = new List<Key>();
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                do
                {
                    string hash = string.Format("{0}{1}", salt, i);
                    
                    for (int h = 0; h < hashRepeat; h++)
                    {
                        byte[] hashBytes = System.Text.Encoding.ASCII.GetBytes(hash);
                        hashBytes = md5.ComputeHash(hashBytes);
                        hash = string.Join("", hashBytes.Select(b => b.ToString("x2")));
                        //if (hashRepeat>1) Console.WriteLine(hash);
                    }
                    var key = new Key()
                    {
                        Hash = hash,
                        Index = i
                    };

                    var triplets = HexTriplet.Matches(hash);
                    if (triplets.Count >= 1) key.TripletChar = triplets[0].Value[0];

                    //var tripletChar = GetTripletChar(hash);
                    //if (tripletChar != ' ') key.TripletChar = tripletChar;

                    potentialKeys.Add(key);
                    if (potentialKeys.Count > 1001)
                    {
                        potentialKeys.RemoveAt(0);
                    }
                    var quintets = HexQuintet.Matches(hash);
                    foreach (var quintetChar in quintets.Select(q => q.Value[0]))
                    {
                        var matchingKeys = potentialKeys.Where(k => k.TripletChar == quintetChar
                                                       && i > k.Index && i - k.Index <= 1000).ToArray();
                        foreach (var matchingKey in matchingKeys)
                        {
                            matchingKey.QuintIndex = i;
                            keys.Add(matchingKey);
                            potentialKeys.Remove(matchingKey);
                        }
                    }
                    //if ((i % 1000) == 0) Console.Write(".");
                    i++;
                    if (keys.Count >= 64 && endIndex == int.MaxValue) endIndex = keys.Last().Index + 1000;
                }
                while (i <= endIndex);
            }

            var result = keys.OrderBy(k=>k.Index).Take(64).ToList();
            return result;
        }

        //private char GetTripletChar(string hash)
        //{
        //    for (int i = 0; i < hash.Length-2; i++)
        //    {
        //        char c = hash[i];

        //        if ((i == 0 || hash[i - 1] != c) && hash[i + 1] == c
        //            && hash[i + 2] == c && (i == hash.Length - 3 || hash[i + 3] != c))
        //        {
        //            return c;
        //        }
        //    }
        //    return ' ';
        //}

        public override string First(string input)
        {
            var keys=GetKeys(input);
            return keys[63].Index.ToString();
        }

        public override string Second(string input)
        {
            var keys = GetKeys(input, hashRepeat:2017);
            return keys[63].Index.ToString();
        }
    }
}
