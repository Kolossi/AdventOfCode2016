using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    class Day20 : Day
    {
        class Range
        {
            public long From { get; set; }
            public long To { get; set; }

            public override int GetHashCode()
            {
                return (int)From ^ (int)To;
            }

            public override bool Equals(object obj)
            {
                if (obj == null) return false;
                var rangeObj = obj as Range;
                if (rangeObj == null) return false;
                return (rangeObj.From == From && rangeObj.To == To);
            }

            public override string ToString()
            {
                return string.Format("{0}-{1}", From, To);
            }
        }

        private List<Range> GetRanges(string[] lines)
        {
            var ranges = new List<Range>();

            foreach (var line in lines)
            {
                var parts = line.Split(new char[] { '-' });
                var range = new Range()
                {
                    From = long.Parse(parts[0]),
                    To = long.Parse(parts[1])
                };
                ranges = MergeInRange(ranges, range);
            }

            return ranges;
        }

        private List<Range> MergeInRange(List<Range> ranges, Range range)
        {
            var crossedRanges = ranges.Where(r => (r.To >= range.To && r.From <= range.From) || (r.To>=range.From-1 && r.To<=range.To) || (r.From>=range.From && r.From<=range.To+1));

            if (crossedRanges.Any())
            {
                crossedRanges = crossedRanges.Append(range);
                range = new Range()
                {
                    From = crossedRanges.Min(r => r.From),
                    To = crossedRanges.Max(r => r.To)
                };
            }

            return ranges.Except(crossedRanges).Append(range).OrderBy(r => r.From).ToList();
        }

        public override string First(string input)
        {
            var lines = GetLines(input);
            var ranges = GetRanges(lines);
            return (ranges.Min(r => r.To) + 1).ToString();
        }

        public override string Second(string input)
        {
            var lines = GetLines(input);
            var ranges = GetRanges(lines);
            long total = 0;
            var prev = ranges[0];
            foreach (var range in ranges.Skip(1))
            {
                total += range.From - prev.To - 1L;
                prev = range;
            }
            total += 4294967295L - prev.To;
            return total.ToString();
        }
    }
}
