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
            // /dev/grid/node-x0-y29
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
                
                foreach (var nodepair in nodes.Where(n => n.Used>0 && n.Used <= node.Avail)
                    .Select(n=> new NodePair() { Node1 = n, Node2 = node }))
                {
                    pairs.Add(nodepair);
                }
                if (node.Used>0)
                {
                    foreach (var nodepair in nodes.Where(n => node.Used <= n.Avail)
                        .Select(n=> new NodePair() { Node1 = node, Node2 = n }))
                    {
                        pairs.Add(nodepair);
                    }
                }
                nodes.Add(node);
            }
            Console.WriteLine(string.Join(Environment.NewLine, pairs.Select(p => p.ToString()).ToArray()));

            return pairs.Count().ToString();
        }

        public override string Second(string input)
        {
            throw new NotImplementedException();
        }
    }
}
