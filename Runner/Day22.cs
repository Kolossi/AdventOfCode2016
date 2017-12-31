using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    class Day22 : Day
    {
        class NodePair
        {
            public Node Node1 { get; set; }
            public Node Node2 { get; set; }

            public override int GetHashCode()
            {
                return Node1.GetHashCode() ^ Node2.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                var objPair = obj as NodePair;
                if (objPair == null) return false;
                return (objPair.Node1.Equals(Node1) && objPair.Node2.Equals(Node2))
                    || (objPair.Node2.Equals(Node1) && objPair.Node1.Equals(Node2));
            }

            public override string ToString()
            {
                return string.Format("{0} :: {1}", Node1, Node2);
            }
        }

        class Node
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Avail { get; set; }
            public int Used { get; set; }

            public override int GetHashCode()
            {
                return X ^ Y;
            }

            public override bool Equals(object obj)
            {
                var objNode = obj as Node;
                if (objNode == null) return false;
                return (objNode.X == X && objNode.Y == Y);
            }

            public override string ToString()
            {
                return string.Format("[{0},{1}] Used:{2}/Avail:{3}", X, Y, Used, Avail);
            }
        }

        class XY
        {
            public int X { get; set; }
            public int Y { get; set; }
            public XY(int x, int y)
            {
                X = x;
                Y = y;
            }

            public override string ToString()
            {
                return string.Format("({0},{1})", X, Y);
            }

            public override bool Equals(object obj)
            {
                var objXY = obj as XY;
                if (objXY == null) return false;
                return (objXY.X == X && objXY.Y == Y);
            }
        }

        private XY[] Directions = new XY[] {
                new XY(0, -1),
                new XY(-1, 0),
                new XY(1, 0),
                new XY(0, 1),
            };

        class State
        {
            public int[,] Used { get; set; }
            public XY DataPos { get; set; }
            public XY EmptyPos { get; set; }
            public int Steps { get; set; }

            public bool Equivalent(State right)
            {
                if (!right.EmptyPos.Equals(EmptyPos)) return false;
                if (!right.DataPos.Equals(DataPos)) return false;
                //for (int y = 0; y < Used.GetLength(1); y++)
                //{
                //    for (int x = 0; x < Used.GetLength(0); x++)
                //    {
                //        if (Used[x, y] != right.Used[x, y]) return false;
                //    }
                //}
                return true;
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                for (int y = 0; y < Used.GetLength(1); y++)
                {
                    for (int x = 0; x < Used.GetLength(0); x++)
                    {
                        sb.Append(string.Format("{1}{0}{1}", (EmptyPos.X == x && EmptyPos.Y == y) ? "_"
                            : ((DataPos.X == x && DataPos.Y == y) ? "G"
                                : ((Used[x, y] > 20) ? "#"
                                    : ".")),
                           x == 0 && y == 0 ? "|" : " "));
                    }
                    sb.AppendLine();
                }
                return sb.ToString();
            }
        }
    

        public override string First(string input)
        {
            var lines = GetLines(input).Skip(2);
            var nodes = new List<Node>();
            var pairs = new HashSet<NodePair>();
            foreach (var line in lines)
            {
                var parts = GetParts(line);
                var node = new Node()
                {
                    X = int.Parse(parts[0].Split("/")[3].Split("-")[1].Substring(1)),
                    Y = int.Parse(parts[0].Split("/")[3].Split("-")[2].Substring(1)),
                    Avail = int.Parse(parts[3].Replace("T", "")),
                    Used = int.Parse(parts[2].Replace("T", ""))
                };

                foreach (var nodepair in nodes.Where(n => n.Used > 0 && n.Used <= node.Avail)
                    .Select(n => new NodePair() { Node1 = n, Node2 = node }))
                {
                    pairs.Add(nodepair);
                }
                if (node.Used > 0)
                {
                    foreach (var nodepair in nodes.Where(n => node.Used <= n.Avail)
                        .Select(n => new NodePair() { Node1 = node, Node2 = n }))
                    {
                        pairs.Add(nodepair);
                    }
                }
                nodes.Add(node);
            }
            //Console.WriteLine(string.Join(Environment.NewLine, pairs.Select(p => p.ToString()).ToArray()));

            return pairs.Count().ToString();
        }

        public override string Second(string input)
        {
            var lines = GetLines(input).Skip(2);
            int[,] size, used;
            XY emptyPos;
            GetArrays(lines, out size, out used, out emptyPos);

            var count = Solve(size, used, emptyPos);

            return count.ToString();
        }

        private int Solve(int[,] size, int[,] used, XY emptyPos)
        {
            int? minSteps = null;
            var toProcess = new Queue<State>();
            int xSize = size.GetLength(0);
            int ySize = size.GetLength(1);
            var previous = new List<State>();

            var firstState = new State()
            {
                DataPos = new XY(xSize - 1, 0),
                Used = used,
                EmptyPos = emptyPos,
                Steps = 0
            };

            previous.Add(firstState);
            toProcess.Enqueue(firstState);

            do
            {
                var state = toProcess.Dequeue();
                var steps = state.Steps + 1;
                previous.Add(state);

                foreach (var dir in Directions)
                {

                    var toMove = new XY(state.EmptyPos.X + dir.X, state.EmptyPos.Y + dir.Y);
                    var dataPos = state.DataPos;
                    //Console.WriteLine("{3}{4}) Empty: {0}, Move: {1}, Data: {2}", state.EmptyPos, toMove, dataPos, new string(' ', steps), steps);
                    //Console.Write("{0}     ",new String(' ',steps));
                    if (toMove.X < 0)
                    {
                        //Console.WriteLine("x too small");
                        continue;
                    }
                    else if (toMove.X >= xSize)
                    {
                        //Console.WriteLine("x too large");
                        continue;
                    }
                    else if (toMove.Y < 0)
                    {
                        //Console.WriteLine("y too small");
                        continue;
                    }
                    else if (toMove.Y >= ySize)
                    {
                        //Console.WriteLine("y too large");
                        continue;
                    }
                    else if (minSteps.HasValue && steps > minSteps.Value)
                    {
                        //Console.WriteLine("steps too many");
                        continue;
                    }
                    else if (state.Used[toMove.X, toMove.Y] > size[state.EmptyPos.X, state.EmptyPos.Y])
                    {
                        //Console.WriteLine("empty too small");
                        continue;
                    }

                    if (toMove.Equals(dataPos))
                    {
                        //Console.WriteLine("moving data to {0}", state.EmptyPos);
                        dataPos = state.EmptyPos;
                        if (state.EmptyPos.X == 0 && state.EmptyPos.Y == 0)
                        {
                            if (!minSteps.HasValue || minSteps.Value > steps) minSteps = steps;
                            Console.WriteLine("****COMPLETE - {0} steps", steps);
                            continue;
                        }
                    }
                    var newUsed = used.Clone() as int[,];
                    newUsed[state.EmptyPos.X, state.EmptyPos.Y] = newUsed[toMove.X, toMove.Y];
                    newUsed[toMove.X, toMove.Y] = 0;
                    var newState = new State()
                    {
                        EmptyPos = toMove,
                        DataPos = dataPos,
                        Steps = steps,
                        Used = newUsed
                    };
                    //Console.WriteLine();
                    //Console.WriteLine(newState);
                    var match = previous.FirstOrDefault(p => p.Equivalent(newState) && p.Steps < steps);
                    if (match != null)
                    {
                        //Console.WriteLine("Seen before");
                        continue;
                    }
                    previous.Add(newState);
                    toProcess.Enqueue(newState);
                    //Console.WriteLine("{0}   Queued", new String(' ', steps));
                }
            } while (toProcess.Any());

            return minSteps.Value;
        }

        private static void GetArrays(IEnumerable<string> lines, out int[,] size, out int[,] used, out XY emptyPos)
        {
            emptyPos = null;
            var nodes = new List<Node>();
            foreach (var line in lines)
            {
                var parts = GetParts(line);
                var node = new Node()
                {
                    X = int.Parse(parts[0].Split("/")[3].Split("-")[1].Substring(1)),
                    Y = int.Parse(parts[0].Split("/")[3].Split("-")[2].Substring(1)),
                    Avail = int.Parse(parts[3].Replace("T", "")),
                    Used = int.Parse(parts[2].Replace("T", ""))
                };
                if (node.Used==0)
                {
                    emptyPos = new XY(node.X, node.Y);
                }
                nodes.Add(node);
            }

            int maxX = nodes.Max(n => n.X);
            int maxY = nodes.Max(n => n.Y);

            size = new int[maxX + 1, maxY + 1];
            used = new int[maxX + 1, maxY + 1];
            foreach (var node in nodes)
            {
                size[node.X, node.Y] = node.Used + node.Avail;
                used[node.X, node.Y] = node.Used;
            }
        }
    }
}
