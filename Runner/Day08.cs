using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using OneOf;

namespace Runner
{
    class Day08 :  Day
    {
        Regex RectRE = new Regex(@"rect ([0-9]+)x([0-9]+)");
        Regex RotRowRE = new Regex(@"rotate row y=([0-9]+) by ([0-9]+)");
        Regex RotColRE = new Regex(@"rotate column x=([0-9]+) by ([0-9]+)");

        public class Pixel
        {
            public int X { get; set; }
            public int Y { get; set; }
            public bool On { get; set; }
            //public override bool Equals(object obj)
            //{
            //    var other = obj as Pixel;
            //    if (other == null) return false;
            //    return ();
            //}
        }

        public class Operation : OneOfBase<Operation.Rect, Operation.RotateRow, Operation.RotateColumn>
        {
            public class Rect
            {
                public int A { get; set; }
                public int B { get; set; }

                public override string ToString()
                {
                    return string.Format("Rect {0}x{1}", A, B);
                }
            }

            public class RotateRow
            {
                public int Row { get; set; }
                public int Pixels { get; set; }

                public override string ToString()
                {
                    return string.Format("Row {0}>{1}", Row, Pixels);
                }
            }

            public class RotateColumn
            {
                public int Column { get; set; }
                public int Pixels { get; set; }

                public override string ToString()
                {
                    return string.Format("Col {0}V{1}", Column, Pixels);
                }
            }

            public override string ToString()
            {
                return this.Match(rect => rect.ToString(), rotRow => rotRow.ToString(), rotCol => rotCol.ToString());
            }
        };

        
        public override string First(string input)
        {
            var operations = GetOperations(input);
            return CountPixels(PerformOperations(operations,50,6)).ToString();
        }

        public override string FirstTest(string input)
        {
            var operations = GetOperations(input);
            var result = PerformOperations(operations,7,3);
            //return Display(result);
            return CountPixels(result).ToString();
        }

        private int CountPixels(bool[,]pixels)
        {
            int count = 0;
            for (int y = 0; y < pixels.GetLength(1); y++)
            {
                for (int x = 0; x < pixels.GetLength(0); x++)
                {

                    if (pixels[x, y]) count++;
                }
            }
            return count;
        }
        private string Display(bool[,] pixels)
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            for (int y = 0; y < pixels.GetLength(1); y++)
            {
                for (int x = 0; x < pixels.GetLength(0); x++)
                {

                    sb.Append(pixels[x, y] ? "#" : ".");
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        private bool[,] PerformOperations(List<OneOf<Operation.Rect, Operation.RotateRow, Operation.RotateColumn>> operations, int screenX, int screenY)
        {
            var pixels = new bool[screenX, screenY];

            foreach (var operation in operations)
            {
                pixels = operation.Match(rect => SetOrCreate(pixels, rect.A, rect.B),
                    rotRow => RotateRow(pixels, rotRow.Row, rotRow.Pixels),
                    rotCol => RotateColumn(pixels, rotCol.Column, rotCol.Pixels));
            }

            return pixels;
        }

        private bool[,] RotateRow(bool[,] pixelsIn, int row, int pixelsToRotate)
        {
            var pixels = (bool[,]) pixelsIn.Clone();
            for (int x = 0; x < pixels.GetLength(0); x++)
            {
                pixels[x, row] = pixelsIn[(x - pixelsToRotate + pixels.GetLength(0)) % pixels.GetLength(0), row];
            }

            return pixels;
        }

        private bool[,] RotateColumn(bool[,] pixelsIn, int column, int pixelsToRotate)
        {
            var pixels = (bool[,])pixelsIn.Clone();
            for (int y = 0; y < pixels.GetLength(1); y++)
            {
                pixels[column, y] = pixelsIn[column, (y - pixelsToRotate + pixels.GetLength(1)) % pixels.GetLength(1)];
            }

            return pixels;
        }

        private bool[,] SetOrCreate(bool[,] pixelsIn, int A, int B)
        {
            var pixels = (bool[,]) pixelsIn.Clone();
            for (int x = 0; x < A; x++)
            {
                for (int y = 0; y < B; y++)
                {
                    pixels[x, y] = true;
                }
            }
            return pixels;
        }

        public List<OneOf<Operation.Rect, Operation.RotateRow, Operation.RotateColumn>> GetOperations(string input)
        {
            var lines = GetLines(input);

            var operations = new List<OneOf<Operation.Rect, Operation.RotateRow, Operation.RotateColumn>>();

            //var operations = new List<Operation>();

            foreach (var line in lines)
            {
                var rectMatch = RectRE.Match(line);
                if (rectMatch.Success)
                {
                    operations.Add(new Operation.Rect()
                    {
                        A = int.Parse(rectMatch.Groups[1].Value),
                        B = int.Parse(rectMatch.Groups[2].Value)
                    });
                    continue;
                }
                var rotColMatch = RotColRE.Match(line);
                if (rotColMatch.Success)
                {
                    operations.Add(new Operation.RotateColumn()
                    {
                        Column = int.Parse(rotColMatch.Groups[1].Value),
                        Pixels = int.Parse(rotColMatch.Groups[2].Value)
                    });
                    continue;
                }
                var rotRowMatch = RotRowRE.Match(line);
                if (rotRowMatch.Success)
                {
                    operations.Add(new Operation.RotateRow()
                    {
                        Row = int.Parse(rotRowMatch.Groups[1].Value),
                        Pixels = int.Parse(rotRowMatch.Groups[2].Value)
                    });
                    continue;
                }
            }
            return operations;
        }

        public override string Second(string input)
        {
            var operations = GetOperations(input);
            return Display(PerformOperations(operations, 50, 6));
        }
    }
}
