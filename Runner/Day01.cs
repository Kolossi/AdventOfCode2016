using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Runner
{
    class Day01 : Day
    {
        class XY
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

        private List<XY> Previous;

        private XY Position = new XY(0, 0);

        private int Direction = 0;

        public override string First(string input)
        {
            Position = new XY(0, 0);
            Direction = 0;

            Walk(input, JustWalk);

            return PositionDistance();
        }

        private string PositionDistance()
        {
            return Math.Abs(Position.X + Position.Y).ToString();
        }

        private string Walk(string input,Func<int,string> walkFunc)
        {
            var steps = input.Split(", ");
            foreach (var step in steps)
            {
                var dir = step.Substring(0, 1);
                Direction = (Direction + (dir == "R" ? 1 : -1) + 4) % 4;
                var dist = int.Parse(step.Substring(1));
                var result = walkFunc(dist);
                if (result != null) return result;
            }
            return null;
        }

        private string JustWalk(int dist)
        {
            Position.X += Directions[Direction].X * dist;
            Position.Y += Directions[Direction].Y * dist;
            return null;
        }

        public string WalkAndCheckPrevious(int distance)
        {
            for (int i = 0; i < distance; i++)
            {
                Position.X += Directions[Direction].X;
                Position.Y += Directions[Direction].Y;
                if (Previous.Any(p => p.X == Position.X && p.Y == Position.Y))
                {
                    return PositionDistance();
                }
                //Console.WriteLine("Visit : [{0},{1}]", Position.X, Position.Y);
                Previous.Add(new XY(Position.X,Position.Y));
            }
            return null;
        }

        public override string Second(string input)
        {
            Position = new XY(0, 0);
            Direction = 0;
            Previous = new List<XY>() { new XY(0,0) };

            return Walk(input, WalkAndCheckPrevious).ToString();
        }
    }
}
