using System;
using System.Collections.Generic;
using System.Text;

namespace Runner
{
    class Day02 : Day
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

        private XY[] Directions = new XY[] {
                new XY(0, 1),
                new XY(1, 0),
                new XY(0, -1),
                new XY(-1, 0)
            };

        private string StepDirs = "DRUL";

        private static string[] complexKeyPad = new string[] { "  1  ", " 234 ", "56789", " ABC ", "  D  " };

        public override string First(string input)
        {
            var result = string.Empty;
            var lines = input.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var key = "5";
            foreach (var line in lines)
            {
                key = WalkPad(key, line, SimplePadMove, SimpleKeyToXY, SimpleXYToKey);
                result += key.ToString();
            }
            return result;
        }

        public override string Second(string input)
        {
            var result = string.Empty;
            var lines = input.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var key = "5";
            foreach (var line in lines)
            {
                key = WalkPad(key, line, ComplexPadMove, ComplexKeyToXY, ComplexXYToKey);
                result += key.ToString();
            }
            return result;
        }

        public override string FirstTest(string input)
        {
            var parts = input.Split(",");
            string start = parts[0];
            var steps = parts[1];

            return WalkPad(start, steps, SimplePadMove, SimpleKeyToXY, SimpleXYToKey);

        }

        public override string SecondTest(string input)
        {
            var parts = input.Split(",");
            string start = parts[0];
            var steps = parts[1];

            return WalkPad(start, steps, ComplexPadMove, ComplexKeyToXY, ComplexXYToKey);

        }
        public string WalkPad(string start, string steps, Func<XY, XY, XY> PadMove, Func<string, XY> KeyToXY, Func<XY, string> XYToKey)
        {
            var pos = KeyToXY(start);
            foreach (var step in steps.ToCharArray())
            {
                var dirXY = Directions[StepDirs.IndexOf(step)];
                pos = PadMove(pos, dirXY);
            }
            return XYToKey(pos);
        }

        public static XY SimplePadMove(XY pos, XY move)
        {
            return new XY(Math.Min(2, Math.Max(0, pos.X + move.X)), Math.Min(2, Math.Max(0, pos.Y + move.Y)));
        }

        public static XY SimpleKeyToXY(string key)
        {
            int keyNum = int.Parse(key);
            return new XY((keyNum - 1) % 3, (int)((keyNum - 1) / 3));
        }

        public static string SimpleXYToKey(XY xy)
        {
            return (xy.Y * 3 + xy.X + 1).ToString();
        }


        public static XY ComplexPadMove(XY pos, XY move)
        {
            var newPos = new XY(Math.Min(4, Math.Max(0, pos.X + move.X)), Math.Min(4, Math.Max(0, pos.Y + move.Y)));
            return ComplexXYToKey(newPos)==" " ? pos: newPos;
        }

        public static XY ComplexKeyToXY(string key)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    var xy = new XY(i, j);
                    if (ComplexXYToKey(xy) == key) return xy;
                }
            }
            return null;
        }

        public static string ComplexXYToKey(XY xy)
        {
            return complexKeyPad[xy.Y].Substring(xy.X,1);
        }
    }
}
