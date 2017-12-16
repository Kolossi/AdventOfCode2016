using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    class Day13 : Day
    {
        public Dictionary<XY, int> Map;

        public class XY
        {
            public int X { get; set; }
            public int Y { get; set; }
            public XY(int x, int y)
            {
                X = x;
                Y = y;
            }

            public override bool Equals(object obj)
            {
                var xy = obj as XY;
                if (xy == null) return false;

                return xy.X == X && xy.Y == Y;
            }

            public override int GetHashCode()
            {
                return 39 ^ X ^ Y;
            }

            public XY Clone()
            {
                return new XY(X, Y);
            }

            public override string ToString()
            {
                return string.Format("{0},{1}", X, Y);
            }
        }

        public class Walk
        {
            public List<XY> Previous { get; set; }
            public XY XY { get; set; }
            public int Dist { get; set; }
            public Walk Clone()
            {
                var walk = new Walk()
                {
                    Previous = new List<XY>(Previous.ToArray()),
                    XY = XY.Clone(),
                    Dist = Dist+1,
                };
                Previous.Add(XY.Clone());
                return walk;
            }

            public List<Walk> GetNextWalks()
            {
                var newWalks = new List<Walk>();
                foreach (var dirXY in Directions)
                {
                    var walk = this.Clone();
                    walk.XY.X += dirXY.X;
                    walk.XY.Y += dirXY.Y;
                    if (!walk.BeenHere()) newWalks.Add(walk);
                }
                return newWalks;
            }

            public bool BeenHere()
            {
                foreach (var prev in Previous)
                {
                    if (prev.Equals(XY)) return true;
                }
                return false;
            }
        }

        private static XY[] Directions = new XY[] {
            new XY(0, 1),
            new XY(1, 0),
            new XY(0, -1),
            new XY(-1, 0),
        };

        public int WalkIt(XY start, XY destination, int favourite, int walkLimit=int.MaxValue)
        {
            int minDistance = walkLimit;
            var firstWalk = new Walk()
            {
                Dist = 0,
                Previous = new List<XY>(),
                XY = start
            };

            var toWalk = new Queue<Walk>();
            toWalk.Enqueue(firstWalk);

            while (toWalk.Any())
            {
                var walk = toWalk.Dequeue();
                if (IsWall(walk.XY, favourite)) continue;

                Map[walk.XY] = Math.Min(Map[walk.XY], walk.Dist);
                if (minDistance < int.MaxValue && walk.Dist > minDistance) continue;

                if (walk.XY.Equals(destination))
                {
                    Console.WriteLine(string.Format("Made it in {0}", walk.Dist));
                    minDistance = Math.Min(minDistance, walk.Dist);
                    continue;
                }

                var newWalks = walk.GetNextWalks();
                foreach (var newWalk in newWalks)
                {
                    toWalk.Enqueue(newWalk);
                }
            }

            return minDistance;
        }

        public bool IsWall(XY xy, int favourite)
        {
            int mapValue;
            if (Map.TryGetValue(xy, out mapValue))
            {
                return (mapValue < 0);
            }

            if (xy.X < 0 || xy.Y < 0) return true;
            UInt64 x = (UInt64)xy.X, y = (UInt64)xy.Y;
            

            UInt64 result = x * x + 3UL * x + 2UL * x * y + y + y * y + (UInt64)favourite;
            var isWall =  (Convert.ToString((int)result, 2).Count(c => c == '1') % 2) == 1;
            Map[xy] = isWall ? -1 : int.MaxValue;
            return isWall;
        }

        public override string First(string input)
        {
            Map = new Dictionary<XY, int>();
            var parts = input.Split(",");
            var destination = new XY(int.Parse(parts[0]), int.Parse(parts[1]));
            var favourite = int.Parse(parts[2]);
            return WalkIt(new XY(1, 1), destination, favourite).ToString();
        }

        public override string Second(string input)
        {
            Map = new Dictionary<XY, int>();
            var parts = input.Split(",");
            var destination = new XY(int.Parse(parts[0]), int.Parse(parts[1]));
            var favourite = int.Parse(parts[2]);
            WalkIt(new XY(1, 1), destination, favourite, 50).ToString();
            return Map.Values.Count(v => v >= 0 && v <=50).ToString();
        }
    }
}
