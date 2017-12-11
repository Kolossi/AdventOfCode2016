using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    class Day11_v2 : Day
    {
        public enum Material
        {
            hydrogen,
            lithium,
            polonium,
            thulium,
            promethium,
            ruthenium,
            cobalt
        }

        public enum ItemType
        {
            microchip,
            generator
        }

        public class Item
        {
            public ItemType ItemType { get; set; }
            public Material Material { get; set; }

            public override string ToString()
            {
                return string.Format("{0:G} {1:G}", Material, ItemType);
            }

            public override bool Equals(object obj)
            {
                var item = obj as Item;
                if (item == null) return false;
                return (ItemType == item.ItemType && Material == item.Material);
            }

            public override int GetHashCode()
            {
                return 97 ^ (int)ItemType ^ (int)Material;
            }

            public static bool AreSame(IEnumerable<Item> left, IEnumerable<Item> right)
            {
                if (left.Count() != right.Count()) return false;
                if (!left.Any()) return true;
                var result = left.Select(i => i.GetHashCode())
                    .Aggregate((a, b) => a ^ b) == right.Select(i => i.GetHashCode()).Aggregate((a, b) => a ^ b);
                return result;
            }

        }

        public class Floor
        {
            public int FloorNum { get; set; }
            public List<Item> Items { get; set; }

            public override string ToString()
            {
                return string.Format("{0} {1}", FloorNum, string.Join(",", Items.Select(i => i.ToString()).ToArray()));
            }

            public override bool Equals(object obj)
            {
                var floor = obj as Floor;
                if (floor == null) return false;
                return (FloorNum == floor.FloorNum && Item.AreSame(Items, floor.Items));
            }

            public override int GetHashCode()
            {
                return 199 ^ FloorNum ^ Items.Select(i => i.GetHashCode()).Aggregate((a, b) => a ^ b);
            }
        }

        public class Facility
        {
            public int ElevatorFloor { get; set; }
            public List<Floor> Floors { get; set; }
            public int Moves { get; set; }

            public Dictionary<string, string> NumberDict = new Dictionary<string, string>() {
                { "first", "1" }, { "second", "2" }, { "third", "3" }, { "fourth", "4" }
            };

            public Facility(string input)
            {
                var floors = new List<Floor>();

                var lines = GetLines(input);
                for (int i = 0; i < lines.Length; i++)
                {
                    var line = lines[i]
                        .Replace(", and a ", ", ")
                        .Replace(" and a ", ", ")
                        .Replace(" a ", " ")
                        .Replace(".", "")
                        .Replace("-compatible", "");
                    foreach (var key in NumberDict.Keys)
                    {
                        line = line.Replace(key, NumberDict[key]);
                    }

                    var floor = new Floor()
                    {
                        FloorNum = int.Parse(GetParts(line)[1]),
                        Items = new List<Item>()
                    };

                    floors.Add(floor);

                    var itemsString = line.Substring(line.IndexOf("contains ") + 9);
                    var itemParts = itemsString.Split(", ");
                    foreach (var itemString in itemParts)
                    {
                        if (itemString == "nothing relevant") continue;
                        var words = GetParts(itemString);
                        var item = new Item()
                        {
                            Material = (Material)Enum.Parse(typeof(Material), words[0]),
                            ItemType = (ItemType)Enum.Parse(typeof(ItemType), words[1])
                        };
                        floor.Items.Add(item);
                    }
                }

                Floors = floors;
                ElevatorFloor = 1;
            }

            public List<Move> GetMoves()
            {
                var moves = new List<Move>();

                var currentFloorItems = Floors.First(f => f.FloorNum == ElevatorFloor).Items;

                if (ElevatorFloor<Floors.Count())
                {
                    //add pairs
                    moves.AddRange(currentFloorItems
                        .SelectMany(i1 => currentFloorItems.Where(i => !i1.Equals(i))
                        .Select(i2 => new Move()
                        {
                            FromFloorNum = ElevatorFloor,
                            ToFloorNum = ElevatorFloor + 1,
                            Items = new List<Item>(new Item[] { i1, i2 })
                        })));

                    //add single items
                    moves.AddRange(currentFloorItems.Select(i => new Move() {
                        FromFloorNum = ElevatorFloor,
                        ToFloorNum = ElevatorFloor + 1,
                        Items = new List<Item>(new Item[] { i })
                    }));

                }
                if (ElevatorFloor>1)
                {
                    //single chips
                    moves.AddRange(currentFloorItems
                        .Where(i => i.ItemType == ItemType.microchip)
                        .Select(i => new Move()
                        {
                            FromFloorNum = ElevatorFloor,
                            ToFloorNum = ElevatorFloor - 1,
                            Items = new List<Item>(new Item[] { i })
                        }));
                }

                moves = moves.OrderByDescending(m => m.Items.Count(i => i.ItemType == ItemType.generator))
                    .OrderByDescending(m => m.Items.Count(i => i.ItemType == ItemType.microchip)).ToList();

                return moves;
            }
        }

        public class Move
        {
            public int FromFloorNum { get; set; }
            public int ToFloorNum { get; set; }
            public List<Item> Items { get; set; }
        }

        public override string First(string input)
        {
            var facility = new Facility(input);
            return facility.Moves.ToString();
        }

        public override string Second(string input)
        {
            throw new NotImplementedException();
        }
    }
}
