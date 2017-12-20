using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    class Day18 : Day
    {
        //public override string FirstTest(string input)
        //{
        //    var row = input.Select(c => c == '^' ? true : false).ToArray();
        //    var next = Enumerable.Range(0, row.Length).Select(i => IsTrap(row, i)).ToArray();
        //    return string.Join("", next.Select(b => b ? '^' : '.'));
        //}

        public override string First(string input)
        {
            //4956 too high
            var row = input.Select(c => c == '^' ? true : false).ToArray();
            var rows = new bool[row.Length][];
            rows[0] = row;
            for (int r = 1; r < row.Length; r++)
            {
                row = Enumerable.Range(0, row.Length).Select(i => IsTrap(row, i)).ToArray();
                rows[r] = row;
            }
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
            throw new NotImplementedException();
        }
    }
}
