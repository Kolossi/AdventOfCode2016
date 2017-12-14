using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using OneOf;

namespace Runner
{
    class Day12 : Day
    {
        public class Token : OneOfBase<Token.Register, Token.IntValue>
        {
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
            public class Copy : Instruction
            {
                public int LineNum { get; set; }
                public Token Source { get; set; }
                public Token.Register Destination { get; set; }

                public void Execute(Alu alu)
                {
                    
                }
            }

            public class Increment : Instruction
            {
                public int LineNum { get; set; }
                public Token.Register Register { get; set; }
            }

            public class Decrement : Instruction
            {
                public int LineNum { get; set; }
                public Token.Register Register { get; set; }
            }

            public class Jump : Instruction
            {
                public int LineNum { get; set; }
                public Token Compare { get; set; }
                public int Offset { get; set; }
            }

        }

        public class Alu
        {
            private Dictionary<int,Instruction> Instructions { get; set; }
            public Dictionary<string,int> Registers { get; set; }
            public int Ptr { get; set; }

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
                
            }
        }

        public override string First(string input)
        {
            throw new NotImplementedException();
        }

        public override string Second(string input)
        {
            throw new NotImplementedException();
        }
    }
}
