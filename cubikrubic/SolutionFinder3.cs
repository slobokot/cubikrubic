using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cubikrubic
{
    class SolutionFinder3
    {
        readonly Func<Cube3, Cube3, bool> eqFunc;
        readonly int threads;
        readonly int depth;
        readonly string resultFile;
        readonly string[] startMoves;
        readonly object printLock = new object();
        bool running;

        public SolutionFinder3(Func<Cube3,Cube3,bool> eqFunc, int threads, int depth, string resultFile)
        {
            this.eqFunc = eqFunc;
            this.threads = threads;
            this.depth = depth;
            this.resultFile = resultFile;
            this.startMoves = null;
            if (threads > 1)
                GetResultFileWaitHandle();
        }

        public SolutionFinder3(Func<Cube3, Cube3, bool> eqFunc, string[] startMoves, int depth, string resultFile)
        {
            this.eqFunc = eqFunc;
            this.threads = -1;
            this.depth = depth;
            this.startMoves = startMoves;
            this.resultFile = resultFile;
        }

        public void FindSolutionWithTimer()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            FindSolution();

            stopwatch.Stop();
            Console.WriteLine("Time required: " + stopwatch.Elapsed);
        }


        Cube3 PrepareCube()
        {
            var cube = new Cube3();
            cube.Rotate("a2", 1);
            cube.Rotate("b2", 1);
            cube.Rotate("c2", 1);
            cube.Rotate("a1", 1);
            cube.Rotate("b1", 1);
            cube.Rotate("c1", 1);
            return cube;
        }

        Cube3 PrepareCubeAndPrint()
        {
            var cube = PrepareCube();
            Console.WriteLine(cube.ToStringWithPieces());
            return cube;
        }


        void FindSolution()
        {
            var refCube = PrepareCubeAndPrint();
            var finder = new RecursiveSolutionFinder3(refCube, (x,y)=>PrintSolution(x,y), eqFunc, depth, threads, startMoves);

            StartPerformanceCounterThread(finder);

            finder.Run();

            Console.WriteLine("================= FINISHED ============================");
            Console.WriteLine("Checked " + finder.Combinations + " combinations");            
        }

        void StartPerformanceCounterThread(RecursiveSolutionFinder3 finder)
        {
            if (threads > 1)
                return;

            new Thread(() =>
            {
                long combinations = 0;
                for (int i = 0; i < 3; i++)
                {
                    int seconds = 10;
                    Thread.Sleep(TimeSpan.FromSeconds(seconds));
                    var newcombinations = finder.Combinations;
                    lock (printLock)
                    {
                        Console.WriteLine((newcombinations - combinations) / seconds + " per sec");
                    }
                    combinations = newcombinations;
                }
            }).Start();
        }

        void PrintSolution(Cube3 cube, IEnumerable<string> result)
        {
            var writer = new StringWriter();
            if (result != null && result.Any())
            {
                writer.WriteLine("Moves:");
                PrintMoves(result, writer);
                writer.WriteLine();
                cube = new Cube3(cube);
                cube.Rotate(result);

                writer.WriteLine(cube.ToStringWithPieces());
            }
            else
            {
                writer.WriteLine("No moves found :(");
            }

            while (true)
            {
                try
                {
                    using (var file = new StreamWriter(File.Open(resultFile, FileMode.Append, FileAccess.Write, FileShare.Read)))
                    {
                        file.Write(writer.ToString());
                    }
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Thread.Sleep(10);
                }
            }
        }

        public static void PrintMoves(IEnumerable<string> moves)
        {
            PrintMoves(moves, Console.Out);
        }

        public static void PrintMoves(IEnumerable<string> moves, TextWriter writer)
        {
            int i = 1;
            foreach (var move in moves)
            {
                if (move == null)
                    break;
                writer.Write((i > 1 ? ", " : "") + i + ". " + move);
                i++;
            }

            writer.WriteLine();
        }

        EventWaitHandle GetResultFileWaitHandle()
        {
            return new EventWaitHandle(true, EventResetMode.AutoReset, "CubikRubicFile"); ;
        }
    }
}
