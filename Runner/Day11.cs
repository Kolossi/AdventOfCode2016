using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner
{
    class Day11 : Day
    {
        public enum ItemType
        {
            generator,
            microchip
        }

        
        public class Facility
        {
            public int MovesMade { get; set; }
            private State _Current;
            public State Current {
                get
                {
                    return _Current;
                }
                set
                {
                    SaveState();
                    _Current = value;
                }
            }

            public List<State> Previous { get; set; }
            public List<Floor> Floors
            {
                get
                {
                    return Current.Floors;
                }
                set
                {
                    SaveState();
                    Current.Floors = value;
                }
            }

            public Facility Clone()
            {
                return new Facility()
                {
                    MovesMade = MovesMade,
                    Previous = new List<State>(Previous.Select(p => p.Clone())),
                    Current = Current.Clone(),
                    Floors = new List<Floor>(Floors.Select(f => f.Clone())),
                    Elevator = Elevator.Clone()
                };
            }

            public Elevator Elevator
            {
                get
                {
                    return Current.Elevator;
                }
                set
                {
                    SaveState();
                    Current.Elevator = value;
                }
            }

            public void SaveState()
            {
                if (Current != null) Previous.Add(Current.Clone());
            }

            public List<Move> GetValidMoves()
            {
                var validMoves = new List<Move>();
                var floor = Floors.First(f => f.FloorNum == Elevator.FloorNum);

                var possibleElevatorItems = GetFloorElevatorItems(floor);
                var destFloors = new List<Floor>();
                if (floor.FloorNum < Floors.Count) destFloors.Add(Floors.First(f => f.FloorNum == (floor.FloorNum + 1)));
                if (floor.FloorNum > 1) destFloors.Add(Floors.First(f => f.FloorNum == (floor.FloorNum - 1)));
                
                foreach (var destFloor in destFloors)
                {
                    foreach (var elevatorItems in possibleElevatorItems)
                    {
                        var move = new Move()
                        {
                            StartFloorNum = floor.FloorNum,
                            EndFloorNum = destFloor.FloorNum,
                            Items = elevatorItems
                        };

                        if (MoveResultIsValid(move)) validMoves.Add(move);
                    }
                }
                return validMoves;
            }

            private Floor CloneFloor(int floorNum)
            {
                return Floors.First(f => f.FloorNum == floorNum).Clone();
            }

            private bool MoveResultIsValid(Move move)
            {
                //%%%%%
                //only a single chip can go down
                if (move.EndFloorNum < move.StartFloorNum && (move.Items.Count() > 1 || move.Items.Any(i => i.ItemType == ItemType.generator)))
                    return false;

                // no gens down
                //if (move.EndFloorNum < move.StartFloorNum && move.Items.Any(i => i.ItemType == ItemType.generator)) return false;
                //%%%%%%

                var newState = Current.Clone();
                DoMove(newState, move);
                var startFloor = newState.Floors.First(f => f.FloorNum == move.StartFloorNum);
                var endFloor = newState.Floors.First(f => f.FloorNum == move.EndFloorNum);

                

                if (ChipFried(startFloor.Items) || ChipFried(endFloor.Items)
                    || Math.Abs(move.StartFloorNum - move.EndFloorNum) != 1
                    || Previous.Contains(newState))
                {
                    return false;
                }
                return true;
            }

            private static bool ChipFried(IEnumerable<Item> items)
            {
               return (items.Any(i => i.ItemType == ItemType.generator)
                    && items.Any(c => c.ItemType == ItemType.microchip && !items.Any(g => g.ItemType == ItemType.generator && g.Material == c.Material)));
            }

            private List<List<Item>> GetFloorElevatorItems(Floor floor)
            {
                var possibleElevatorItems = new List<List<Item>>();
                var chips = floor.Items.Where(i => i.ItemType == ItemType.microchip);
                var generators = floor.Items.Where(i => i.ItemType == ItemType.generator);
                var generatorlessChips = chips.Where(c => !generators.Any(g => g.Material == c.Material));
                var chiplessGenerators = generators.Where(g => !chips.Any(c => c.Material == g.Material));
                var pairMaterials = chips.Where(c => generators.Any(g => g.Material == c.Material)).Select(c => c.Material); ;
                // add each chipless generator pair
                possibleElevatorItems.AddRange(chiplessGenerators.SelectMany(g1 => chiplessGenerators.Where(g2 => string.Compare(g1.Material, g2.Material) > 0).Select(g2 => new List<Item>(new Item[] { g1, g2 }))));

                // add each individual chipless generator
                possibleElevatorItems.AddRange(chiplessGenerators.Select(g => new List<Item>(new Item[] { g })));

                // add each chip/generator pair
                possibleElevatorItems.AddRange(pairMaterials.Select(m => new List<Item>(new Item[] { chips.First(c => c.Material == m), generators.First(g => g.Material == m) })));

                if (chips.Count() <= 1 && generators.Any())
                {
                    // add each generator pair
                    possibleElevatorItems.AddRange(generators.SelectMany(g1 => generators.Where(g2 => string.Compare(g1.Material, g2.Material) > 0).Select(g2 => new List<Item>(new Item[] { g1, g2 }))));
                    // add each individual generator
                    possibleElevatorItems.AddRange(generators.Select(g => new List<Item>(new Item[] { g })));
                }

                // add each chip pair
                possibleElevatorItems.AddRange(chips.SelectMany(c1 => chips.Where(c2 => string.Compare(c1.Material, c2.Material) > 0)
                                                                      .Select(c2 => new List<Item>(new Item[] { c1, c2 }))));
                // add each individual chip
                possibleElevatorItems.AddRange(chips.Select(c => new List<Item>(new Item[] { c })));



 
                return possibleElevatorItems.Where(l1 => !possibleElevatorItems.Any(l2 => l2 != l1 && Item.AreSame(l2, l1))).ToList();
            }

            public Facility DoMove(Move move)
            {
                var newState = Current.Clone();
                DoMove(newState, move);
                Current = newState;
                MovesMade++;
                return this;
            }

            public static State DoMove(State newState, Move move)
            { 
                var startFloor = newState.Floors.First(f=>f.FloorNum==move.StartFloorNum);
                var endFloor = newState.Floors.First(f => f.FloorNum == move.EndFloorNum);
                startFloor.Items = new List<Item>(startFloor.Items.Except(move.Items));
                endFloor.Items = new List<Item>(endFloor.Items.Union(move.Items));
                newState.Elevator.FloorNum = move.EndFloorNum;
                newState.Elevator.Items = new List<Item>();
                return newState;
            }
        }

        public class Move
        {
            public int StartFloorNum { get; set; }
            public int EndFloorNum { get; set; }
            public List<Item> Items { get; set; }
            public override string ToString()
            {
                return string.Format("{0}->{1}:{2}", StartFloorNum, EndFloorNum, string.Join(", ", Items.Select(i => i.ToString()).ToArray()));
            }

            public override bool Equals(object obj)
            {
                var move = obj as Move;
                if (move == null) return false;
                return (StartFloorNum == move.StartFloorNum && EndFloorNum == move.EndFloorNum && Item.AreSame(Items, move.Items));
            }

            public override int GetHashCode()
            {
                return 19 ^ StartFloorNum ^ EndFloorNum ^ Items.Select(i=>i.GetHashCode()).Aggregate((a, b) => a ^ b);
            }
        }

        public class State
        {
            public List<Floor> Floors { get; set; }
            public Elevator Elevator { get; set; }

            public State Clone()
            {
                return new State()
                {
                    Floors = Floors.Select(f => f.Clone()).ToList(),
                    Elevator = Elevator.Clone()
                };
            }

            public override bool Equals(object obj)
            {
                var state = obj as State;
                if (state == null) return false;
                return (Elevator.Equals(state.Elevator) && Floors.SequenceEqual(state.Floors));
            }

            public override int GetHashCode()
            {
                return 109 ^ Elevator.GetHashCode() ^ Floors.SelectMany(f=>f.Items).Select(i => i.GetHashCode()).Aggregate((a, b) => a ^ b);
            }
        }

        public class Item
        {
            public ItemType ItemType { get; set; }
            public string Material { get; set; }

            public override string ToString()
            {
                return string.Format("{0} {1:G}", Material, ItemType);
            }

            public override bool Equals(object obj)
            {
                var item = obj as Item;
                if (item == null) return false;
                return (ItemType == item.ItemType && Material.Equals(item.Material));
            }

            public override int GetHashCode()
            {
                return 97 ^ (int)ItemType ^ Material.GetHashCode();
            }

            public static bool AreSame(IEnumerable<Item> left,IEnumerable<Item> right)
            {
                if (left.Count() != right.Count()) return false;
                if (!left.Any()) return true;
                //var leftOrdered = left.OrderBy(i => i.Material).ThenBy(i => i.ItemType);
                //var rightOrdered = right.OrderBy(i => i.Material).ThenBy(i => i.ItemType);
                var result = (left.Select(i => i.GetHashCode()).Aggregate((a, b) => a ^ b) == right.Select(i => i.GetHashCode()).Aggregate((a, b) => a ^ b));
                //var result = !left.Any(l=>!right.Any(r=>r.Equals(l)));
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
                return (FloorNum == floor.FloorNum && Item.AreSame(Items,floor.Items));
            }

            public override int GetHashCode()
            {
                return 199 ^ FloorNum ^ Items.Select(i => i.GetHashCode()).Aggregate((a, b) => a ^ b);
            }

            public Floor Clone()
            {
                return new Floor()
                {
                    FloorNum = FloorNum,
                    Items = new List<Item>(Items)
                };
            }
        }

        public class Elevator
        {
            public int FloorNum { get; set; }
            public List<Item> Items { get; set; }

            public Elevator Clone()
            {
                return new Elevator()
                {
                    FloorNum = FloorNum,
                    Items = new List<Item>(Items)
                };
            }

            public override bool Equals(object obj)
            {
                var elevator = obj as Elevator;
                if (elevator == null) return false;
                return (FloorNum == elevator.FloorNum && Item.AreSame(Items, elevator.Items));
            }

            public override int GetHashCode()
            {
                return 73 ^ FloorNum ^ Items.Select(i => i.GetHashCode()).Aggregate((a, b) => a ^ b);
            }
        }

        public Dictionary<string, string> NumberDict = new Dictionary<string, string>() {
            { "first", "1" }, { "second", "2" }, { "third", "3" }, { "fourth", "4" }
        };

        private Facility GetFacility(string input)
        {
            var floors=new List<Floor>();

            var lines = GetLines(input);
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i]
                    .Replace(", and a ", ", ")
                    .Replace(" and a ", ", ")
                    .Replace(" a ", " ")
                    .Replace(".","")
                    .Replace("-compatible","");
                foreach (var key in NumberDict.Keys)
                {
                    line = line.Replace(key, NumberDict[key]);
                }

                var floor = new Floor() {
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
                        Material = words[0],
                        ItemType = (ItemType)Enum.Parse(typeof(ItemType), words[1])
                    };
                    floor.Items.Add(item);
                }
            }

            return new Facility() {
                Current = new State()
                {
                    Floors = floors,
                    Elevator = new Elevator()
                    {
                        FloorNum = 1,
                        Items = new List<Item>()
                    }
                },
                Previous = new List<State>()
            };
        }

        public Facility MoveToTop(Facility facility)
        {
            if (facility.Floors.Where(f => f.FloorNum != 4).All(f => !f.Items.Any()))
            {
                Console.WriteLine(string.Format(">>>=>{0}", facility.MovesMade));
                return facility;
            }
            var moves = facility.GetValidMoves();
            if (!moves.Any())
            {
                return null;
            }

            //foreach (var move in moves)

            Facility parallelResult = null;
            object myLock = new object();
            //Parallel.ForEach(moves, move =>
            foreach (var move in moves)
            {
                var withMove = facility.Clone().DoMove(move);
                Console.WriteLine(string.Format("{0} : {1}{2}",
                    string.Join(",", withMove.Floors.Select(f => f.Items.Count(i => i.ItemType == ItemType.generator).ToString())),
                    string.Join("",Enumerable.Repeat(" ",facility.MovesMade)),
                    move));
                var result = MoveToTop(withMove);
                if (result != null) return result;

                //lock (myLock)
                // {
                     
                //     if (result != null)
                //     {
                //         Console.WriteLine(string.Format(">>>=>{0}", result.MovesMade));

                //         parallelResult = result;
                //         return parallelResult;
                //         break;
                //     }
                // }
             }//);
            return parallelResult;
        }

        public override string First(string input)
        {
            var facility = GetFacility(input);
            facility = MoveToTop(facility);
            Console.WriteLine("WAITING");
            Console.ReadLine();
            return facility.MovesMade.ToString();


            //var facility = GetFacility(input);

            //var moves = facility.GetValidMoves();
            //Console.WriteLine();
            //Console.WriteLine(">>>===>MOVES\r\n" + string.Join("\r\n", moves.Select(m => m.ToString()).ToArray()));
            //facility.DoMove(moves.First());
            //moves = facility.GetValidMoves();
            //Console.WriteLine();
            //Console.WriteLine(">>>===>MOVES\r\n" + string.Join("\r\n", moves.Select(m => m.ToString()).ToArray()));
            //facility.DoMove(moves.First());
            //moves = facility.GetValidMoves();
            //Console.WriteLine();
            //Console.WriteLine(">>>===>MOVES\r\n" + string.Join("\r\n", moves.Select(m => m.ToString()).ToArray()));
            //return string.Join("\r\n", facility.Floors.Select(f => f.ToString()).ToArray());
            
        }

        public override string Second(string input)
        {
            throw new NotImplementedException();
        }
    }
}
