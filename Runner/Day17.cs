using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Runner
{
    class Day17 : Day
    {
        public class XY
        {
            public int X { get; set; }
            public int Y { get; set; }
            public XY(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        private static Dictionary<char, XY> Directions = new Dictionary<char, XY>
        {
            {'U', new XY(0, -1)},
            {'D', new XY(0, 1)},
            {'L', new XY(-1, 0)},
            {'R', new XY(1, 0)},
        };

        private static char[] DirKeys = new char[] {'U', 'D', 'L', 'R'};

        public class Walk
        {
            private static System.Security.Cryptography.MD5 _md5=null;
            public XY Pos { get; set; }
            public string Route { get; set; }

            public List<char> openDoor = new List<char>(new char[] {'b', 'c', 'd', 'e', 'f'});
            public Walk[] GetNextWalks()
            {
                if (_md5==null) _md5 = new MD5CryptoServiceProvider();
                byte[] msgBytes = System.Text.Encoding.ASCII.GetBytes(Salt + Route);
                byte[] hashBytes = _md5.ComputeHash(msgBytes);
                string hashKey = string.Format("{0:x2}{1:x2}", hashBytes[0], hashBytes[1]);
                var walks = Enumerable.Range(0, 4).Where(i => openDoor.Contains(hashKey[i]))
                    .Select(i => new Walk()
                    {
                        Pos = new XY(Pos.X+Directions[DirKeys[i]].X, Pos.Y + Directions[DirKeys[i]].Y),
                        Route = Route + DirKeys[i]
                    })
                    .ToArray();
                return walks;
            }
        }

        public static string Salt;

        public override string First(string input)
        {
            Salt = input;
            var walkQueue = new Queue<Walk>();
            walkQueue.Enqueue(new Walk() {Pos = new XY(0, 0), Route = ""});
            Walk walk = null;
            do
            {
                walk = walkQueue.Dequeue();
                if (walk.Pos.X == 3 && walk.Pos.Y == 3) break;
                var walksInRange = walk.GetNextWalks()
                    .Where(w => w.Pos.X >= 0 && w.Pos.X < 4 && w.Pos.Y >= 0 && w.Pos.Y < 4);

                foreach (var newWalk in walksInRange)
                {
                    walkQueue.Enqueue(newWalk);
                }
            } while (true);
            return walk.Route;
        }

        public override string Second(string input)
        {
            Salt = input;
            var walkQueue = new Queue<Walk>();
            walkQueue.Enqueue(new Walk() { Pos = new XY(0, 0), Route = "" });
            Walk walk = null;
            Walk longestWalk = null;
            do
            {
                walk = walkQueue.Dequeue();
                if (walk.Pos.X == 3 && walk.Pos.Y == 3)
                {
                    if (longestWalk == null || walk.Route.Length > longestWalk.Route.Length)
                    {
                        longestWalk = walk;
                    }
                    continue;
                }
                var walksInRange = walk.GetNextWalks()
                    .Where(w => w.Pos.X >= 0 && w.Pos.X < 4 && w.Pos.Y >= 0 && w.Pos.Y < 4);

                foreach (var newWalk in walksInRange)
                {
                    walkQueue.Enqueue(newWalk);
                }
            } while (walkQueue.Any());
            return longestWalk.Route.Length.ToString();
        }
    }
}
