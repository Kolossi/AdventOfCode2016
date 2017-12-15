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
        }

        private XY[] Directions = new XY[] {
            new XY(0, 1),
            new XY(1, 0),
            new XY(0, -1),
            new XY(-1, 0)
        };

        public override string First(string input)
        {
            Map = new Dictionary<XY, int>();
            var favourite = int.Parse(input);
            var sb = new StringBuilder();
            for (int y = 0; y < 6; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    sb.Append(IsWall(new XY(x, y), favourite) ? "#" : ".");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public override string Second(string input)
        {
            throw new NotImplementedException();
        }

        public bool IsWall(XY xy, int favourite)
        {
            int mapValue;
            if (Map.TryGetValue(xy, out mapValue))
            {
                if (mapValue < 0) return true;
            }

            if (xy.X < 0 || xy.Y < 0) return true;
            UInt64 x = (UInt64)xy.X, y = (UInt64)xy.Y;
            

            UInt64 result = x * x + 3UL * x + 2UL * x * y + y + y * y + (UInt64)favourite;
            var isWall =  (Convert.ToString((int)result, 2).Count(c => c == '1') % 2) == 1;
            Map[xy] = isWall ? -1 : 0;
            return isWall;
        }
    }
}
