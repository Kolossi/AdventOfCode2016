using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    class Day18 : Day
    {
        public override string First(string input)
        {
            //4956 too high
            var parts = GetParts(input);

            var rowCount = int.Parse(parts[0]);
            var row = parts[1].Select(c => c == '^' ? true : false).ToArray();
            var rows = new bool[rowCount][];
            rows[0] = row;
            for (int r = 1; r < rowCount; r++)
            {
                row = Enumerable.Range(0, row.Length).Select(i => IsTrap(row, i)).ToArray();
                rows[r] = row;
            }

            // for first tests
            if (rowCount==2) return string.Join("", row.Select(b => b ? '^' : '.'));

            return rows.SelectMany(r => r).Count(b => !b).ToString();
        }

        private bool IsTrap(bool[] row, int i)
        {
            var left = i > 0 ? row[i - 1] : false;
            //var centre = row[i];
            var right = i < row.Length - 1 ? row[i + 1] : false;
            return (left ^ right);
        }

        public override string Second(string input)
        {
            return First(input);
        }
    }
}
