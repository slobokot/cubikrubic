using System;
using System.Collections.Generic;

namespace cubikrubic
{
    class Program
    {
        static void Main(string[] args)
        {
            int depth = 10;
            var eqFunc = EqFuncs.SwapEdgeNeighborsEqFunc();
            var resultFile = "c:\cubikrubik.txt";

            if (args.Length > 0 && args[0] == "--child")
            {
                List<string> moves = new List<string>();
                string x = args[1];
                while(x.Length > 0)
                {
                    moves.Add(x.Substring(0, 4));
                    x = x.Substring(4);
                }

                new SolutionFinder3(
                    eqFunc,
                    moves.ToArray(),
                    depth,
                    resultFile)
                    .FindSolutionWithTimer();
            }
            else
            {
                int threads = args.Length > 0 ? int.Parse(args[0]) : -1;
                threads = threads < 1 ? Math.Max(1, Environment.ProcessorCount - 1) : threads;
                Console.WriteLine($"Using {threads} threads");

                new SolutionFinder3(
                    eqFunc,
                    threads,
                    depth,
                    resultFile)
                    .FindSolutionWithTimer();
            }
        }
    }
}
