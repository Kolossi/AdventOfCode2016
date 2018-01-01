using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using OneOf;

namespace Runner
{
    class Day25 : Day
    {

        public class Token : OneOfBase<Token.Register, Token.IntValue>
        {
            public static Token GetToken(string input)
            {
                int val;
                if (int.TryParse(input, out val))
                {
                    return new IntValue() { Value = val };
                }
                else
                {
                    return new Register() { RegName = input };
                }
            }

            public class Register : Token
            {
                public string RegName { get; set; }

                public override string ToString()
                {
                    return RegName;
                }
            }

            public class IntValue : Token
            {
                public int Value { get; set; }

                public override string ToString()
                {
                    return Value.ToString();
                }
            }

            public override string ToString()
            {
                return this.Match(reg => reg.ToString(), val => val.ToString());
            }
        }

        public class Instruction : OneOfBase<Instruction.Copy, Instruction.Increment, Instruction.Decrement,
            Instruction.Jump, Instruction.Toggle, Instruction.Output>
        {
            public static Instruction GetInstruction(string input)
            {
                var parts = GetParts(input);
                Instruction instruction;
                switch (parts[0])
                {
                    case "cpy":
                        instruction = new Instruction.Copy()
                        {
                            Source = Token.GetToken(parts[1]),
                            Destination = new Token.Register() { RegName = parts[2] }
                        };
                        break;
                    case "inc":
                        instruction =
                            new Instruction.Increment() { Register = new Token.Register() { RegName = parts[1] } };
                        break;
                    case "dec":
                        instruction =
                            new Instruction.Decrement() { Register = new Token.Register() { RegName = parts[1] } };
                        break;
                    case "jnz":
                        instruction = new Instruction.Jump()
                        {
                            Compare = Token.GetToken(parts[1]),
                            Offset = Token.GetToken(parts[2])
                        };
                        break;
                    case "tgl":
                        instruction = new Instruction.Toggle()
                        {
                            Offset = Token.GetToken(parts[1])
                        };
                        break;
                    case "out":
                        instruction = new Instruction.Output()
                        {
                            Value = Token.GetToken(parts[1])
                        };
                    break;
                    default:
                        throw new InvalidOperationException("Assembunny");
                }
                return instruction;
            }

            public class Copy : Instruction
            {
                public Token Source { get; set; }
                public Token.Register Destination { get; set; }

                public void ExecuteCopy(Alu alu)
                {
                    alu.SetRegValue(Destination,
                        Source.Match(reg => alu.GetRegValue(reg),
                                     val => val.Value));
                }
            }

            public class Increment : Instruction
            {
                public Token.Register Register { get; set; }

                public void ExecuteInc(Alu alu)
                {
                    alu.SetRegValue(Register, alu.GetRegValue(Register) + 1);
                }
            }

            public class Decrement : Instruction
            {
                public Token.Register Register { get; set; }

                public void ExecuteDec(Alu alu)
                {
                    alu.SetRegValue(Register, alu.GetRegValue(Register) - 1);
                }
            }

            public class Jump : Instruction
            {
                public Token Compare { get; set; }
                public Token Offset { get; set; }

                public void ExecuteJump(Alu alu)
                {
                    if (Compare.Match(reg => alu.GetRegValue(reg), val => val.Value) != 0)
                    {
                        var offset = Offset.Match(reg => alu.GetRegValue(reg), val => val.Value);
                        if (offset == 0) throw new StackOverflowException("Assembunny");
                        alu.AdjustPtr(offset - 1);
                    }
                }
            }

            public class Toggle : Instruction
            {
                public Token Offset { get; set; }

                public void ExecuteToggle(Alu alu)
                {
                    var offset = Offset.Match(reg => alu.GetRegValue(reg), val => val.Value);
                    alu.ToggleInstruction(offset);
                }
            }

            public class Output : Instruction
            {
                public Token Value { get; set; }

                public void ExecuteOutput(Alu alu)
                {
                    var value = Value.Match(reg => alu.GetRegValue(reg), val => val.Value);
                    alu.Output(value);

                }
            }

            public void Execute(Alu alu)
            {
                this.Switch(copy => copy.ExecuteCopy(alu),
                    inc => inc.ExecuteInc(alu),
                    dec => dec.ExecuteDec(alu),
                    jump => jump.ExecuteJump(alu),
                    toggle => toggle.ExecuteToggle(alu),
                    output => output.ExecuteOutput(alu));
            }

            public Instruction MakeToggled(Alu alu)
            {
                return this.Match(copy => new Jump() { Compare = copy.Source, Offset = copy.Destination },
                    inc => new Decrement() { Register = inc.Register },
                    dec => new Increment() { Register = dec.Register },
                    jump => jump.Offset.Match(reg => (Instruction)new Copy()
                                              {
                                                  Source = jump.Compare,
                                                  Destination = reg
                                              },
                                              val => (Instruction)new Jump()
                                              {
                                                  Compare = new Token.IntValue() { Value = 0 },
                                                  Offset = new Token.IntValue() { Value = 0 }
                                              }),
                    tgl => tgl.Offset.Match(reg => (Instruction)new Increment()
                                            {
                                                Register = reg
                                            },
                                            val => (Instruction)new Jump()
                                            {
                                                Compare = new Token.IntValue() { Value = 0 },
                                                Offset = new Token.IntValue() { Value = 0 }
                                            }),
                    output => output.Value.Match(reg => (Instruction) new Increment() { Register = reg },
                                                 val => (Instruction)new Jump()
                                                 {
                                                     Compare = new Token.IntValue() { Value = 0 },
                                                     Offset = new Token.IntValue() { Value = 0 }
                                                 }));
            }

        }

        public class Alu
        {
            private Instruction[] Instructions { get; set; }
            public Dictionary<string, int> Registers { get; set; }
            public int Ptr { get; set; }
            public List<int> Outputs { get; set; }
            public Alu(string input)
            {
                Ptr = 0;
                var lines = GetLines(input);
                Instructions = new Instruction[lines.Length];
                foreach (var line in lines)
                {
                    Instructions[Ptr++] = Instruction.GetInstruction(line);
                }
                Reset();
            }

            public void Reset()
            {
                Registers = new Dictionary<string, int>();
                Outputs = new List<int>();
                Ptr = 0;
            }

            public int GetRegValue(Token.Register reg)
            {
                if (!Registers.ContainsKey(reg.RegName))
                {
                    Registers[reg.RegName] = 0;
                    return 0;
                }
                return Registers[reg.RegName];
            }

            public void SetRegValue(Token.Register reg, int value)
            {
                Registers[reg.RegName] = value;
            }

            public void ToggleInstruction(int offset)
            {
                var togglePtr = Ptr + offset;
                if (togglePtr >= 0 && togglePtr < Instructions.Length)
                {
                    var instruction = Instructions[Ptr + offset];
                    Instructions[Ptr + offset] = instruction.MakeToggled(this);
                }
            }

            public void AdjustPtr(int offset)
            {
                Ptr += offset;
            }

            public void ExecuteProgram()
            {
                Ptr = 0;
                while (Ptr >= 0 && Ptr < Instructions.Length)
                {
                    Instructions[Ptr].Execute(this);
                    Ptr++;
                }
            }

            public void Output(int value)
            {
                if (value == (Outputs.Count % 2))
                {
                    Outputs.Add(value);
                    if (Outputs.Count < 500) return;
                }
                else
                {
                    Outputs = new List<int>();
                }
                //abort program
                Ptr = Instructions.Length + 1;
            }
        }

        public override string First(string input)
        {
            var alu = new Alu(input);
            for (int a = 0; a < 100000; a++)
            {
                alu.Reset();
                alu.Registers["a"] = a;
                alu.ExecuteProgram();
                if (alu.Outputs.Count > 0) return a.ToString();
            }
            return "NOPE";
        }

        public override string Second(string input)
        {
            throw new NotImplementedException();
        }
    }
}
