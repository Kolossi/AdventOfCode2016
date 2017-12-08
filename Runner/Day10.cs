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
            var lines = GetLines(input);
            return ProcessFirst(lines, 61, 17);
        }

        public override string FirstTest(string input)
        {
            var lines = GetLines(input);
            var parts = GetParts(lines[0]);
            var chip1 = int.Parse(parts[0]);
            var chip2 = int.Parse(parts[1]);
            lines = lines.Skip(1).ToArray();
            return ProcessFirst(lines, chip1, chip2);
        }

        private string ProcessFirst(string[] lines, int chip1, int chip2)
        {
            if (chip2 < chip1)
            {
                var tmp = chip1;
                chip1 = chip2;
                chip2 = tmp;
            }
            var factory = GetFactory(lines);
            return factory.Destinations
                .Where(d => d.Match(
                    bot => (bot.LowChip == chip1 && bot.HighChip == chip2),
                    bin => false))
                .First()
                .Match(
                    bot => bot.Number.ToString(),
                    bin => "");
        }

        public Factory GetFactory(string[] lines)
        {
            var factory = new Factory();
            foreach (var line in lines)
            {
                var parts = GetParts(line);
                if (parts[0] == "value")
                {
                    var value = int.Parse(parts[1]);
                    if (parts[4]=="bot")
                    {
                        var botnum = int.Parse(parts[5]);
                        factory.GetOrMakeBot(botnum).TakeChip(value);
                    }
                    else
                    {
                        throw new InvalidOperationException("give value straight to bin?");
                    }
                }
                else if (parts[0]=="bot")
                {
                    var botNum = int.Parse(parts[1]);
                    var lowDestNum = int.Parse(parts[6]);
                    var highDestNum = int.Parse(parts[11]);
                    var bot = factory.GetOrMakeBot(botNum);
                    bot.LowDest = factory.GetOrMakeDestinationFromInput(parts[5], parts[6]);
                    bot.HighDest = factory.GetOrMakeDestinationFromInput(parts[10], parts[11]);
                }
                else
                {
                    throw new InvalidOperationException("unrecognised line");
                }
            }
            return factory;
        }

        public override string Second(string input)
        {
            throw new NotImplementedException();
        }
    }

    public class Factory
    {
        public List<Destination> Destinations { get; set; }
        public Factory()
        {
            Destinations = new List<Destination>();
        }
    }

    //public class Instruction : OneOfBase<Instruction.GiveValue, Instruction.BotProgram>
    //{
    //    public class GiveValue : Instruction
    //    {
    //        public int ChipValue { get; set; }
    //        public Destination.Bot Bot { get; set; }
    //    }

    //    public class BotProgram : Instruction
    //    {
    //        public Destination.Bot Bot { get; set; }
    //        public Destination LowDest { get; set; }
    //        public Destination HighDest { get; set; }
    //    }
    //}

    public class Destination : OneOfBase<Destination.Bot, Destination.Bin>
    {
        public class Bot : Destination
        {
            public int Number { get; set; }
            public int? LowChip { get; set; }
            public int? HighChip { get; set; }
            public Destination LowDest { get; set; }
            public Destination HighDest { get; set; }

            public override string ToString()
            {
                return string.Format("Bot {0} : {1}->{2} {3}->{4}",
                    Number,
                    LowChip,
                    LowDest == null ? "?" : LowDest.Match(bot => string.Format("bot {0}", bot.Number), bin => string.Format("bin {0}", bin.Number)),
                    HighChip,
                    HighDest == null ? "?" : HighDest.Match(bot => string.Format("bot {0}", bot.Number), bin => string.Format("bin {0}", bin.Number))
                    );
            }
        }

        public class Bin : Destination
        {
            public int Number { get; set; }
            public int? Chip { get; set; }

            public override string ToString()
            {
                return string.Format("Bin {0} : {1}",
                    Number,
                    Chip);
            }
        }

        public override string ToString()
        {
            return this.Match(bot => bot.ToString(), bin => bin.ToString());
        }
    }

    public static class DestinationHelpers
    {
        public static Destination GetOrMakeDestinationFromInput(this Factory self, string type, string number)
        {
            var num = int.Parse(number);
            if (type == "bot")
            {
                return self.GetOrMakeBot(num);
            }
            else if (type=="output")
            {
                return self.GetOrMakeBin(num);
            }
            else
            {
                throw new InvalidOperationException("unknown input type");
            }
        }

        public static Destination.Bot GetOrMakeBot(this Factory self, int botNum)
        {
            var existing = self.Destinations.Any(d => d.Match(bot => bot.Number == botNum, bin => false))
                ? self.Destinations.First(d => d.Match(bot => bot.Number == botNum, bin => false)) as Destination.Bot
                : null;
            if (existing != null) return existing;
            var newBot = new Destination.Bot() { Number = botNum };
            self.Destinations.Add(newBot);
            return newBot;
        }

        public static Destination.Bin GetOrMakeBin(this Factory self, int binNum)
        {
            var existing = self.Destinations.Any(d => d.Match(bot => false, bin => bin.Number == binNum))
                ? self.Destinations.First(d => d.Match(bot => false, bin => bin.Number == binNum)) as Destination.Bin
                : null;
            if (existing != null) return existing;
            var newBin = new Destination.Bin() { Number = binNum };
            self.Destinations.Add(newBin);
            return newBin;
        }

        public static Destination TakeChip(this Destination self, int chip)
        {
            return self.Match(bot => bot.TakeChip(chip), bin => bin.TakeChip(chip));
        }

        public static Destination TakeChip(this Destination.Bot self, int chip)
        {
            if (self.HighChip > 0) throw new InvalidOperationException("Three chips");

            self.HighChip = !self.LowChip.HasValue  ? 0 : Math.Max(chip, self.LowChip.Value);
            self.LowChip = !self.LowChip.HasValue ? chip : Math.Min(chip, self.LowChip.Value);
            

            return self;
        }

        public static Destination TakeChip(this Destination.Bin self, int chip)
        {
            if (self.Chip > 0) throw new InvalidOperationException("Already has a chip");

            self.Chip = chip;

            return self;
        }
        
        public static Destination GiveChips(this Destination.Bot self)
        {
            if (!self.LowChip.HasValue || !self.HighChip.HasValue) throw new InvalidOperationException("Chips not full");
            self.LowDest.TakeChip(self.LowChip.Value);
            self.HighDest.TakeChip(self.HighChip.Value);
            return self;
        }
    }
}
