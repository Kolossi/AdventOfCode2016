using OneOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Runner
{
    class Day21 : Day
    {
        public interface IInstruction
        {
            char[] Process(char[] input);
            IInstruction Reverse();
        }

        public class PositionSwap:IInstruction
        {
            public int IndexX { get; set; }
            public int IndexY { get; set; }

            public char[] Process(char[] input)
            {
                return SwapPositions(input, IndexX, IndexY);
            }

            public static char[] SwapPositions(char[] input, int indexX, int indexY)
            {
                char t = input[indexX];
                input[indexX] = input[indexY];
                input[indexY] = t;
                return input;
            }

            public IInstruction Reverse()
            {
                return this;
            }
        }

        public class LetterSwap:IInstruction
        {
            public char X { get; set; }
            public char Y { get; set; }

            public char[] Process(char[] input)
            {
                var indexX = Array.IndexOf(input, X);
                var indexY = Array.IndexOf(input, Y);
                return PositionSwap.SwapPositions(input, indexX, indexY);
            }

            public IInstruction Reverse()
            {
                return this;
            }
        }

        public class Rotate:IInstruction
        {
            public int Count { get; set; }

            public char[] Process(char[] input)
            {
                return LeftRotate(input, Count);
            }

            public static char[] LeftRotate(char[] input, int count)
            {
                var length = input.Length;
                while (count < 0) count += length;
                var output = new char[length];
                for (int i = 0; i < length; i++)
                {
                    output[i] = input[(i + count) % length];
                }
                return output;
            }

            public IInstruction Reverse()
            {
                return new Rotate() { Count = -Count };
            }
        }

        public class RotatePos:IInstruction
        {
            public char X { get; set; }

            public char[] Process(char[] input)
            {
                var indexX = Array.IndexOf(input, X);
                var count = indexX + 1;
                if (indexX >= 4) count++;
                return Rotate.LeftRotate(input, -count);
            }

            public IInstruction Reverse()
            {
                return new UnRotatePos()
                {
                    X = X
                };
            }
        }

        public class UnRotatePos : IInstruction
        {
            public char X { get; set; }

            public char[] Process(char[] input)
            {
                var indexX = Array.IndexOf(input, X);

                var count = indexX == 0 ? 1 : ((indexX % 2) == 1) ? ((indexX - 1) / 2) + 1 : (indexX / 2) + 5;
                return Rotate.LeftRotate(input, count);
            }

            public IInstruction Reverse()
            {
                throw new NotImplementedException();
            }
        }


        public class Reverse : IInstruction
        {
            public int IndexX { get; set; }
            public int IndexY { get; set; }

            public char[] Process(char[] input)
            {
                var length = input.Length;
                var output = new char[length];
                for (int i = 0; i < length; i++)
                {
                    if (i >= IndexX && i <= IndexY)
                    {
                        output[i] = input[IndexY - (i - IndexX)];
                    }
                    else
                    {
                        output[i] = input[i];
                    }
                    
                }
                return output;
            }

            IInstruction IInstruction.Reverse()
            {
                return this;
            }
        }

        public class Move : IInstruction
        {
            public int IndexX { get; set; }
            public int IndexY { get; set; }

            public char[] Process(char[] input)
            {
                var length = input.Length;
                var output = new char[length];
                for (int i = 0; i < length; i++)
                {
                    if (i < Math.Min(IndexX,IndexY) || i > Math.Max(IndexX,IndexY))
                    {
                        output[i] = input[i];
                    }
                    else if (i==IndexY)
                    {
                        output[i] = input[IndexX];
                    }
                    else
                    {
                        output[i] = input[i-IndexX.CompareTo(IndexY)];
                    }

                }
                return output;

            }

            public IInstruction Reverse()
            {
                return new Move()
                {
                    IndexX = IndexY,
                    IndexY = IndexX
                };
            }
        }

        private static string ProcessPassword(List<IInstruction> instructions, char[] password)
        {
            
            
            foreach (var instruction in instructions)
            {
                password = instruction.Process(password);
                //Console.WriteLine(string.Join("", password));
            }

            return string.Join("", password);
        }

        private static List<IInstruction> GetInstructions(IEnumerable<string> instructionLines)
        {
            var instructions = new List<IInstruction>();
            IInstruction instruction;
            foreach (var instructionLine in instructionLines)
            {
                instruction = null;

                var parts = GetParts(instructionLine);

                if (parts[0] == "swap")
                {
                    if (parts[1] == "position")
                    {
                        instruction = new PositionSwap()
                        {
                            IndexX = int.Parse(parts[2]),
                            IndexY = int.Parse(parts[5])
                        };
                    }
                    else
                    {
                        instruction = new LetterSwap()
                        {
                            X = parts[2][0],
                            Y = parts[5][0]
                        };
                    }
                }
                else if (parts[0] == "rotate")
                {
                    if (parts[1] == "based")
                    {
                        instruction = new RotatePos()
                        {
                            X = parts[6][0]
                        };
                    }
                    else
                    {
                        instruction = new Rotate()
                        {
                            Count = int.Parse(parts[2]) * (parts[1] == "right" ? -1 : 1)
                        };
                    }

                }
                else if (parts[0] == "reverse")
                {
                    instruction = new Reverse()
                    {
                        IndexX = int.Parse(parts[2]),
                        IndexY = int.Parse(parts[4])
                    };
                }
                else if (parts[0] == "move")
                {
                    instruction = new Move()
                    {
                        IndexX = int.Parse(parts[2]),
                        IndexY = int.Parse(parts[5])
                    };
                }
                if (instruction == null) throw new InvalidOperationException();

                instructions.Add(instruction);

            }
            return instructions;
        }

        public static List<IInstruction> GetReverseInstructions(List<IInstruction> instructions)
        {
            var revInstructions = new List<IInstruction>();
            foreach (var instruction in instructions)
            {
                revInstructions.Insert(0, instruction.Reverse());
            }
            return revInstructions;
        }

        public override string FirstTest(string input)
        {
            var parts = input.Split(",");
            var password = parts[0].ToArray();
            var lines = new string[] { parts[1] };
            var instructions = GetInstructions(lines);
            return ProcessPassword(instructions, password);
        }

        public override string SecondTest(string input)
        {
            var parts = input.Split(",");
            var password = parts[0].ToArray();
            var lines = new string[] { parts[1] };
            var instructions = GetInstructions(lines);
            var revInstructions = GetReverseInstructions(instructions);
            return ProcessPassword(revInstructions, password);
        }

        public override string First(string input)
        {
            //return "";
            var lines = GetLines(input);
            var password = lines[0].ToArray();
            lines = lines.Skip(1).ToArray();
            var instructions = GetInstructions(lines);
            return ProcessPassword(instructions, password);
        }

        public override string Second(string input)
        {
            var lines = GetLines(input);
            var password = lines[0].ToArray();
            lines = lines.Skip(1).ToArray();
            var instructions = GetInstructions(lines);
            var revInstructions = GetReverseInstructions(instructions);
            return ProcessPassword(revInstructions, password);
        }
    }
}
