using System;

namespace cubikrubic
{
    class Program
    {
        static void Main(string[] args)
        {
            int threads = args.Length > 0 ? int.Parse(args[0]) : -1;
            threads = threads < 1 ? Math.Max(1, Environment.ProcessorCount - 1) : threads;
            Console.WriteLine($"Using {threads} threads");

            new SolutionFinder3(
                EqFuncs.SwapEdgeNeighborsEqFunc(), 
                threads)
                .FindSolutionWithTimer();
        }
    }
}
