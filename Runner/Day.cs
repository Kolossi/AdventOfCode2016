using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Runner
{
    public abstract class Day
    {
        public abstract string First(string input);
        public abstract string Second(string input);

        public string Solve(string set, Func<string,string> solver)
        {
            string input = string.Empty;
            try
            {
                input = GetInput(set);
            }
            catch (FileNotFoundException)
            {
            }

            if (string.IsNullOrWhiteSpace(input)) return "INPUT MISSING";

            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                var result =  solver(input);
                sw.Stop();
                Console.Write("(Took : {0}) ",sw.Elapsed);
                return result;
            }
            catch (NotImplementedException)
            {
                return "NOT IMPLEMENTED";
            }
        }

        public string SolveFirst()
        {
            return Solve("First", this.First);
        }

        public string SolveSecond()
        {
            return Solve("Second", this.Second);
        }

        public virtual string FirstTest(string input)
        {
            return this.First(input);
        }

        public virtual string SecondTest(string input)
        {
            return this.Second(input);
        }

        public bool TestFirst()
        {
            return Test("First", this.FirstTest);
        }

        public bool TestSecond()
        {
            return Test("Second", this.SecondTest);
        }

        public bool Test(string set, Func<string, string> solver)
        {
            bool result = true;
            string input = string.Empty;

            Console.WriteLine(string.Format("    {0} Tests", set));
            try
            {
                input = GetInput(string.Format("{0}Tests", set));
            }
            catch (FileNotFoundException)
            {
            }

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("      TEST INPUT MISSING");
                return false;
            }

            var lines = input.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {

                var parts = line.Split(":");
                var testInput = parts[0];
                var expectedOutput = parts[1];
                string output = string.Empty;
                try
                {
                    output = solver(testInput);
                    if (output != expectedOutput)
                    {
                        result = false;
                        Console.WriteLine(string.Format("    {0} : FAILED", line));
                        Console.WriteLine(string.Format("      Input : {0}, Expected : {1}, Got : {2}", testInput,
                            expectedOutput, output));
                    }
                    else
                    {
                        Console.WriteLine(string.Format("    {0} : OK", line));
                    }
                }
                catch (NotImplementedException)
                {
                    Console.WriteLine(string.Format("    {0} : NOT IMPLEMENTED", line));
                    result = false;
                }
            }
            return result;
        }

        public string GetInput(string set)
        {
            string filename = string.Format("Inputs/{0}{1}.txt",this.GetType().Name, set);
            string input = System.IO.File.ReadAllText(filename);
            return input;
        }

        public static string[] GetLines(string input)
        {
            return input.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] GetParts(string input)
        {
            return input.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
