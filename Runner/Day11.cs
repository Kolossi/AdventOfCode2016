using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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

        public enum Material
        {
            hydrogen,
            lithium,
            polonium,
            thulium,
            promethium,
            ruthenium,
            cobalt,
            elerium,
            dilithium
        }

        public class Facility
        {
            public int MovesMade { get; set; }
            public static List<Material> Materials;
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

            public static List<EquivalentState> Previous { get; set; }
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

            public override string ToString()
            {
                return string.Format("#{0}>{1}", MovesMade, Current);
            }

            public static bool StateSeenBefore(EquivalentState newState)
            {
                // make arequivalent(state1,state2) which for each material finds the int pairs of floors where its chip and gen are.
                // Then order these pairs by floor1,floor2
                // equivalence means the pairs are identical (including dups) - the actual material is not important but its pattern still is:
                //So these are equivalent:
                //
                //hydrogen - chip hydrogen - generator
                //lithium - chip
                //lithium - generator
                //
                //and
                //
                //lithium - chip lithium - generator
                //hydrogen - chip
                //hydrogen - generator
                //
                //however  if middle row in lower example above was cobalt-chip instead of hydrogen-chip it wouldn't be equivalent
                // because the lower 2 floors chip and gen are different materials now, whereas they were the same before
                if (Previous == null)
                {
                    Previous = new List<EquivalentState>();
                    return false;
                }
                return Previous.Any(s=>s.Equals(newState));
            }

            public Facility Clone()
            {
                return new Facility()
                {
                    MovesMade = MovesMade,
                    //Previous = new List<State>(Previous.Select(p => p.Clone())),
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
                if (Current != null)
                {
                    var currentEquivalent = new EquivalentState(Current, Materials);
                    if (!StateSeenBefore(currentEquivalent))
                    {
                        Previous.Add(currentEquivalent);
                    }
                }
            }

            public List<Move> GetValidMoves()
            {
                var validMoves = new List<Move>();
                var floor = Floors.First(f => f.FloorNum == Elevator.FloorNum);

                var possibleElevatorItems = GetFloorElevatorItems(floor);

                if (floor.FloorNum < Floors.Count)
                {
                    var destFloor = Floors.First(f => f.FloorNum == (floor.FloorNum + 1));
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

                if (floor.FloorNum > 1)
                {
                    var destFloor = Floors.First(f => f.FloorNum == (floor.FloorNum - 1));
                    //foreach (var elevatorItems in possibleElevatorItems.Where(e=>e.Count()==1 && e.Any(i=>i.ItemType==ItemType.microchip)))
                    foreach (var elevatorItems in possibleElevatorItems)
                    {
                            var move = new Move()
                        {
                            StartFloorNum = floor.FloorNum,
                            EndFloorNum = destFloor.FloorNum,
                            Items = elevatorItems
                        };

                        if (MoveResultIsValid(move)) validMoves.Add(move);
                        //if (MoveResultIsValid(move))
                        //{
                        //    validMoves.Add(move);
                        //    Facility.Previous.Add();
                        //}
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
                //only a single chip can go down
                //if (move.EndFloorNum < move.StartFloorNum
                //    && (move.Items.Count() > 1 || move.Items.Any(i => i.ItemType == ItemType.generator)))
                //{
                //    return false;
                //}

                //if (move.EndFloorNum > move.StartFloorNum 
                //    && Current.ChipsAway 
                //    && move.Items.Count() == 1
                //    && move.Items.Any(i => i.ItemType == ItemType.microchip))
                //{
                //    return false;
                //}

                var newState = Current.Clone();
                DoMove(newState, move);
                var startFloor = newState.Floors.First(f => f.FloorNum == move.StartFloorNum);
                var endFloor = newState.Floors.First(f => f.FloorNum == move.EndFloorNum);

                //var endChips = endFloor.Items.Where(i => i.ItemType == ItemType.microchip);
                //var endGenerators = endFloor.Items.Where(i => i.ItemType == ItemType.generator);
                //var endGeneratorlessChips = endChips.Where(c => !endGenerators.Any(g => g.Material == c.Material));

                //var startChips = startFloor.Items.Where(i => i.ItemType == ItemType.microchip);
                //var startGenerators = startFloor.Items.Where(i => i.ItemType == ItemType.generator);
                //var startGeneratorlessChips = startChips.Where(c => !startGenerators.Any(g => g.Material == c.Material));


                if (ChipFried(startFloor.Items) || ChipFried(endFloor.Items)
                    || Math.Abs(move.StartFloorNum - move.EndFloorNum) != 1
                    //|| Facility.StateSeenBefore(new EquivalentState(newState, Facility.Materials))
                    //|| Previous.Contains(newState)
                    //|| (startGenerators.Count() > 1 && endGeneratorlessChips.Count() > 2)
                    //|| (endGenerators.Count() > 1 && startGeneratorlessChips.Count() > 2)
                    )
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
                var moves = new List<List<Item>>();
                var currentFloorItems = floor.Items;

                //add same material pairs
                moves.AddRange(currentFloorItems
                    .SelectMany(i1 => currentFloorItems.Where(i => i.Material == i1.Material && i.ItemType < i1.ItemType)
                                                       .Select(i2 => new List<Item>(new Item[] { i1, i2 }))));

                //add same type pairs
                moves.AddRange(currentFloorItems.OrderBy(i=>i.ItemType) //generators first
                    .SelectMany(i1 => currentFloorItems.Where(i => i.ItemType == i1.ItemType && i.Material < i1.Material)
                                                       .Select(i2 => new List<Item>(new Item[] { i1, i2 }))));


                //add single items
                moves.AddRange(currentFloorItems.Select(i => new List<Item>(new Item[] { i })));

                moves = moves.OrderByDescending(m => m.Count(i => i.ItemType == ItemType.generator))
                    .ThenByDescending(m => m.Count(i => i.ItemType == ItemType.microchip))
                    .ThenBy(m=>m.Min(i=>i.Material))
                    .ToList();

                return moves;


                //var chips = floor.Items.Where(i => i.ItemType == ItemType.microchip);
                //var generators = floor.Items.Where(i => i.ItemType == ItemType.generator);
                //var generatorlessChips = chips.Where(c => !generators.Any(g => g.Material == c.Material));
                //var chiplessGenerators = generators.Where(g => !chips.Any(c => c.Material == g.Material));
                //var pairMaterials = chips.Where(c => generators.Any(g => g.Material == c.Material)).Select(c => c.Material); ;
                //// add each chipless generator pair
                //possibleElevatorItems.AddRange(chiplessGenerators.SelectMany(g1 => chiplessGenerators.Where(g2 => string.Compare(g1.Material, g2.Material) > 0).Select(g2 => new List<Item>(new Item[] { g1, g2 }))));

                //// add each individual chipless generator
                //possibleElevatorItems.AddRange(chiplessGenerators.Select(g => new List<Item>(new Item[] { g })));

                //// add each chip/generator pair
                //possibleElevatorItems.AddRange(pairMaterials.Select(m => new List<Item>(new Item[] { chips.First(c => c.Material == m), generators.First(g => g.Material == m) })));

                //if (chips.Count() <= 1 && generators.Any())
                //{
                //    // add each generator pair
                //    possibleElevatorItems.AddRange(generators.SelectMany(g1 => generators.Where(g2 => string.Compare(g1.Material, g2.Material) > 0).Select(g2 => new List<Item>(new Item[] { g1, g2 }))));
                //    // add each individual generator
                //    possibleElevatorItems.AddRange(generators.Select(g => new List<Item>(new Item[] { g })));
                //}

                //// add each chip pair
                //possibleElevatorItems.AddRange(chips.SelectMany(c1 => chips.Where(c2 => string.Compare(c1.Material, c2.Material) > 0)
                //                                                      .Select(c2 => new List<Item>(new Item[] { c1, c2 }))));
                //// add each individual chip
                //possibleElevatorItems.AddRange(chips.Select(c => new List<Item>(new Item[] { c })));



 
                //return possibleElevatorItems.Where(l1 => !possibleElevatorItems.Any(l2 => l2 != l1 && Item.AreSame(l2, l1))).ToList();
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
                //if (move.Items.Count() == 1 && move.Items.Any(i => i.ItemType == ItemType.microchip) && move.EndFloorNum < move.StartFloorNum)
                //{
                //    newState.ChipsAway = true;
                //}
                var startFloor = newState.Floors.First(f=>f.FloorNum==move.StartFloorNum);
                var endFloor = newState.Floors.First(f => f.FloorNum == move.EndFloorNum);
                startFloor.Items = startFloor.Items.Except(move.Items);
                endFloor.Items = endFloor.Items.Union(move.Items);
                newState.Elevator.FloorNum = move.EndFloorNum;
                //newState.Elevator.Items = new List<Item>();
                return newState;
            }
        }

        public class Move
        {
            public int StartFloorNum { get; set; }
            public int EndFloorNum { get; set; }
            private List<Item> _Items;
            public IEnumerable<Item> Items
            {
                get
                {
                    return _Items;
                }
                set
                {
                    _Items = new List<Item>(value.OrderBy(i => i.Material).ThenBy(i => i.ItemType));
                }
            }
            public override string ToString()
            {
                return string.Format("{0}->{1}:{2}", StartFloorNum, EndFloorNum, string.Join(",", Items.Select(i => i.ToString()).ToArray()));
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

        public class PairState
        {
            public int GeneratorFloorNum { get; set; }
            public int MicrochipFloorNum { get; set; }

            public override string ToString()
            {
                return string.Format("[{0},{1}]", GeneratorFloorNum, MicrochipFloorNum);
            }

            public override bool Equals(object obj)
            {
                var pairState = obj as PairState;
                if (pairState == null) return false;
                return (GeneratorFloorNum==pairState.GeneratorFloorNum && MicrochipFloorNum==pairState.MicrochipFloorNum);
            }
        }

        public class EquivalentState
        {
            public List<PairState> PairStates { get; set; }
            public int ElevatorFloor { get; set; }

            public EquivalentState(State state, List<Material> materials)
            {
                ElevatorFloor = state.Elevator.FloorNum;
                PairStates = materials.Select(m => new PairState()
                {
                    GeneratorFloorNum =
                        state.Floors.First(f => f.Items.Any(i => i.Material == m && i.ItemType == ItemType.generator))
                            .FloorNum,
                    MicrochipFloorNum =
                        state.Floors.First(f => f.Items.Any(i => i.Material == m && i.ItemType == ItemType.microchip))
                            .FloorNum
                }).OrderBy(ps=>ps.GeneratorFloorNum).ThenBy(ps=>ps.MicrochipFloorNum).ToList();
            }

            public override string ToString()
            {
                return string.Format("{0}::{1}", ElevatorFloor,
                    string.Join(",", PairStates.Select(ps => ps.ToString())));
            }

            public override bool Equals(object obj)
            {
                var equivalentState = obj as EquivalentState;
                if (equivalentState == null) return false;

                return (ElevatorFloor == equivalentState.ElevatorFloor && PairStates
                            .Select((p, i) => equivalentState.PairStates[i].Equals(p)).All(b => b == true));

            }
        }

        public class State
        {
            public List<Floor> Floors { get; set; }
            public Elevator Elevator { get; set; }
            //public bool ChipsAway { get; set; }

            public State Clone()
            {
                return new State()
                {
                    Floors = Floors.Select(f => f.Clone()).ToList(),
                    Elevator = Elevator.Clone(),
                    //ChipsAway = ChipsAway
                };
            }

            public override bool Equals(object obj)
            {
                var state = obj as State;
                if (state == null) return false;
                var elevatorSame = Elevator.Equals(state.Elevator);
                if (!elevatorSame) return false;

                var floorsSame = Floor.AreSame(Floors,state.Floors);
                return floorsSame;
            }

            public override int GetHashCode()
            {
                return 109 ^ Elevator.GetHashCode() ^ Floors.SelectMany(f=>f.Items).Select(i => i.GetHashCode()).Aggregate((a, b) => a ^ b);
            }

            public override string ToString()
            {
                return string.Format("{0}:::{1}", Elevator, string.Join("||", Floors.Select(f => f.ToString())));
            }
        }

        public class Item
        {
            public ItemType ItemType { get; set; }
            public Material Material { get; set; }

            public override string ToString()
            {
                return string.Format("{0}{1}", (int)Material, ItemType==ItemType.generator?"G":"C");
            }

            public override bool Equals(object obj)
            {
                var item = obj as Item;
                if (item == null) return false;
                return (ItemType == item.ItemType && Material==item.Material);
            }

            public override int GetHashCode()
            {
                return 97 ^ (int)ItemType ^ (int)Material;
            }

            public static bool AreSame(IEnumerable<Item> left,IEnumerable<Item> right)
            {
                if (left.Count() != right.Count()) return false;
                if (!left.Any()) return true;
                //var leftOrdered = left.OrderBy(i => i.Material).ThenBy(i => i.ItemType);
                //var rightOrdered = right.OrderBy(i => i.Material).ThenBy(i => i.ItemType);
                //var result = (left.Select(i => i.GetHashCode()).Aggregate((a, b) => a ^ b) == right.Select(i => i.GetHashCode()).Aggregate((a, b) => a ^ b)) ;
                var hashSame = (left.Select(i => i.GetHashCode()).Aggregate((a, b) => a ^ b) ==
                                right.Select(i => i.GetHashCode()).Aggregate((a, b) => a ^ b));

                if (!hashSame) return false;
                var areEqual = left.All(l => right.Any(r => r.Equals(l)));
                return areEqual;
            }
        }

        public class Floor
        {
            public int FloorNum { get; set; }

            private List<Item> _Items;
            public IEnumerable<Item> Items
            {
                get
                {
                    return _Items; 
                }
                set
                {
                    _Items = new List<Item>(value.OrderBy(i=>i.Material).ThenBy(i=>i.ItemType));
                }
            }

            public override string ToString()
            {
                return string.Format("{0}:{1}", FloorNum, string.Join(",", Items.Select(i => i.ToString())));
            }

            public override bool Equals(object obj)
            {
                var floor = obj as Floor;
                if (floor == null) return false;
                return (FloorNum == floor.FloorNum && Item.AreSame(Items,floor.Items));
            }

            public override int GetHashCode()
            {
                return 199 ^ FloorNum ^ (Items.Any()?Items.Select(i => i.GetHashCode()).Aggregate((a, b) => a ^ b):0);
            }

            public Floor Clone()
            {
                return new Floor()
                {
                    FloorNum = FloorNum,
                    _Items = new List<Item>(_Items)
                };
            }


            public static bool AreSame(IEnumerable<Floor> left, IEnumerable<Floor> right)
            {
                if (left.Count() != right.Count()) return false;
                if (!left.Any()) return true;
                //var leftOrdered = left.OrderBy(i => i.Material).ThenBy(i => i.ItemType);
                //var rightOrdered = right.OrderBy(i => i.Material).ThenBy(i => i.ItemType);
                //var hashSame = (left.Select(i => i.GetHashCode()).Aggregate((a, b) => a ^ b) ==
                //                right.Select(i => i.GetHashCode()).Aggregate((a, b) => a ^ b));
                //if (!hashSame) return false;
                var areEqual = left.All(l => right.Where(r=>r.FloorNum==l.FloorNum).Any(r => r.Equals(l)));
                return areEqual;
            }
        }

        public class Elevator
        {
            public int FloorNum { get; set; }
            //public List<Item> Items { get; set; }

            public Elevator Clone()
            {
                return new Elevator()
                {
                    FloorNum = FloorNum,
                    //Items = new List<Item>(Items)
                };
            }

            public override bool Equals(object obj)
            {
                var elevator = obj as Elevator;
                if (elevator == null) return false;
                return (FloorNum == elevator.FloorNum /*&& Item.AreSame(Items, elevator.Items)*/);
            }

            public override int GetHashCode()
            {
                return 73 ^ FloorNum /*^ Items.Select(i => i.GetHashCode()).Aggregate((a, b) => a ^ b)*/;
            }

            public override string ToString()
            {
                return string.Format("E{0}", FloorNum);
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
                    Items = Enumerable.Empty<Item>()
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
                    floor.Items=floor.Items.Union(new Item[]{item});
                }
            }

            Facility.Previous = null;
            var facility = new Facility() {
                Current = new State()
                {
                    Floors = floors,
                    Elevator = new Elevator()
                    {
                        FloorNum = 1,
                        //Items = new List<Item>()
                    }
                }
            };
            Facility.Previous = new List<EquivalentState>();
            Facility.Materials = floors.SelectMany(f => f.Items).Select(i => i.Material).Distinct().ToList();
            return facility;
        }

        public Facility MoveToTop(Facility rootFacility)
        {
            var toProcess = new Queue<Facility>();
            toProcess.Enqueue(rootFacility);
            Facility result = null;
            int maxMoves = 0;
            int batchMaxMoves = 0;
            int batchCount = 0;

            while (toProcess.Any())
            {
                var facility = toProcess.Dequeue();

                if (facility.MovesMade > maxMoves) maxMoves = facility.MovesMade;
                if (facility.MovesMade > batchMaxMoves) batchMaxMoves = facility.MovesMade;

                if (facility.MovesMade > 2980)
                {
                    Console.WriteLine("Burned");
                    continue;
                }

                if (batchCount++ % 1000 == 0)
                {
                    Console.WriteLine(string.Format("Combos={0}, batchMax={1}, max={2}, queueLength={3}", batchCount, batchMaxMoves,
                        maxMoves, toProcess.Count()));
                }

                if (facility.Floors.Where(f => f.FloorNum != 4).All(f => !f.Items.Any()))
                {
                    Console.WriteLine(string.Format(">>>=>{0}", facility.MovesMade));
                    if (result == null || (facility.MovesMade < result.MovesMade))
                    {
                        result = facility;
                        //                    return result;
                    }
                    return facility;
                }
                var moves = facility.GetValidMoves();
                if (!moves.Any())
                {
                    continue;
                }

                //foreach (var move in moves)

                //Facility parallelResult = null;
                //object myLock = new object();
                //Parallel.ForEach(moves, move =>
                foreach (var move in moves)
                {
                    var withMove = facility.Clone().DoMove(move);
                    //Console.WriteLine(string.Format("{0} : {1}{2}",
                    //    string.Join("|", withMove.Floors.Select(f => string.Format("{0},{1}", f.Items.Count(i => i.ItemType == ItemType.generator), f.Items.Count(i => i.ItemType == ItemType.microchip)))),
                    //    string.Join("", Enumerable.Repeat(" ", facility.MovesMade)),
                    //    move));

                    var equiv = new EquivalentState(withMove.Current, Facility.Materials);
                    if (!Facility.StateSeenBefore(equiv))
                    {
                        toProcess.Enqueue(withMove);
                        Facility.Previous.Add(equiv);
                    }



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
                } //);
            }
            return result;
        }

        public override string First(string input)
        {
            // to 16secs (4000+ combos) = 47
            return DoIt(input);
        }

        private string DoIt(string input)
        {
            var facility = GetFacility(input);
            Console.WriteLine(facility);
            facility = MoveToTop(facility);
            return facility.MovesMade.ToString();
        }

        public override string Second(string input)
        {
            // took 5min40s (15000+ combos) = 71
            return DoIt(input);
        }
    }
}
