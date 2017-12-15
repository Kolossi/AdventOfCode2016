using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using OneOf;

namespace Runner
{
    class Day12 : Day
    {
        public class Token : OneOfBase<Token.Register, Token.IntValue>
        {
            public static Token GetToken(string input)
            {
                int val;
                if (int.TryParse(input, out val))
                {
                    return new IntValue() {Value = val};
                }
                else
                {
                    return new Register() {RegName = input};
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
            Instruction.Jump>
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
                            Destination = new Token.Register() { RegName = parts[2]}
                        };
                        break;
                    case "inc":
                        instruction =
                            new Instruction.Increment() {Register = new Token.Register() {RegName = parts[1]}};
                        break;
                    case "dec":
                        instruction =
                            new Instruction.Decrement() { Register = new Token.Register() { RegName = parts[1] } };
                        break;
                    case "jnz":
                        instruction = new Instruction.Jump()
                        {
                            Compare = Token.GetToken(parts[1]),
                            Offset = int.Parse(parts[2])
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
                public int Offset { get; set; }

                public void ExecuteJump(Alu alu)
                {
                    if (Compare.Match(reg => alu.GetRegValue(reg), val => val.Value) != 0)
                    {
                        if (Offset == 0) throw new StackOverflowException("Assembunny");
                        alu.AdjustPtr(Offset - 1);
                    }
                }
            }

            public void Execute(Alu alu)
            {
                this.Switch(copy => copy.ExecuteCopy(alu), 
                    inc => inc.ExecuteInc(alu),
                    dec => dec.ExecuteDec(alu),
                    jump => jump.ExecuteJump(alu));
            }

        }

        public class Alu
        {
            private Instruction[] Instructions { get; set; }
            public Dictionary<string,int> Registers { get; set; }
            public int Ptr { get; set; }

            public Alu(string input)
            {
                Registers = new Dictionary<string, int>();
                Ptr = 0;
                var lines = GetLines(input);
                Instructions = new Instruction[lines.Length];
                foreach (var line in lines)
                {
                    Instructions[Ptr++] = Instruction.GetInstruction(line);
                }
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
        }

        public override string First(string input)
        {
            var alu = new Alu(input);
            alu.ExecuteProgram();
            return alu.Registers["a"].ToString();
        }

        public override string Second(string input)
        {
            var alu = new Alu(input);
            alu.Registers["c"] = 1;
            alu.ExecuteProgram();
            return alu.Registers["a"].ToString();
        }
    }
}
