using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OneOf;

namespace Runner
{
    class Day10 : Day
    {
 
        public override string First(string input)
        {
            throw new NotImplementedException();
        }

        public override string FirstTest(string input)
        {
            var lines = GetLines(input);
            var parts = GetParts(lines[0]);
            var chip1 = int.Parse(parts[0]);
            var chip2 = int.Parse(parts[0]);
            lines = lines.Skip(1).ToArray();
        }

        public override string Second(string input)
        {
            throw new NotImplementedException();
        }
    }

    public class Factory
    {
        public List<Destination> Destinations { get; set; }
    }

    public class Instruction : OneOfBase<Instruction.GiveValue, Instruction.BotProgram>
    {
        public class GiveValue : Instruction
        {
            public int ChipValue { get; set; }
            public int BotNumber { get; set; }
        }

        public class BotProgram : Instruction
        {
            public int BotNumber { get; set; }
            public Destination LowDest { get; set; }
            public Destination HighDest { get; set; }
        }
    }

    public class Destination : OneOfBase<Destination.Bot, Destination.Bin>
    {
        public class Bot : Destination
        {
            public int Number { get; set; }
            public int LowChip { get; set; }
            public int HighChip { get; set; }
            public Destination LowDest { get; set; }
            public Destination HighDest { get; set; }
        }

        public class Bin : Destination
        {
            public int Number { get; set; }
            public int Chip { get; set; }
        }
    }

    public static class DestinationHelpers
    {
        public static Factory UpdateDestination(this Factory self, Destination destination)
        {
            var factory = new Factory()
            {
                Destinations = self.Destinations.Where(d => !d.Equals(destination)).Select(d =>
                    d.Match(bot => bot.Clone(), bin => bin.Clone())).ToList()
            };
            factory.Destinations.Add(destination);
            return factory;
        }

        public static Destination TakeChip(this Destination.Bot self, int chip)
        {
            if (self.HighChip > 0) throw new InvalidOperationException("Three chips");
            var newBot = new Destination.Bot()
            {
                Number = self.Number,
                LowDest = self.LowDest,
                HighDest = self.HighDest,
                LowChip = self.LowChip == 0 ? chip : Math.Min(chip, self.LowChip),
                HighChip = self.LowChip == 0 ? 0 : Math.Max(chip, self.LowChip)
            };
            return newBot;
        }


        public static Destination Clone(this Destination.Bot self)
        {
            return new Destination.Bot()
            {
                Number = self.Number,
                LowDest = self.LowDest,
                HighDest = self.HighDest,
                LowChip = self.LowChip,
                HighChip = self.HighChip
            };
        }

        public static Destination Clone(this Destination.Bin self)
        {
            return new Destination.Bin() { Number = self.Number, Chip = self.Chip };
        }

        public static Destination TakeChip(this Destination.Bin self, int chip)
        {
            var bin = new Destination.Bin { Number = self.Number, Chip = chip };
            return bin;
        }
    }
}
