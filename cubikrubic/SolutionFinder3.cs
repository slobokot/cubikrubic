using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cubikrubic
{
    class SolutionFinder3
    {
        Func<Cube3, Cube3, bool> eqFunc;
        int threads;
        int depth;
        object printLock = new object();

        public SolutionFinder3(Func<Cube3,Cube3,bool> eqFunc, int threads, int depth)
        {
            this.eqFunc = eqFunc;
            this.threads = threads;
            this.depth = depth;
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
            var finder = new RecursiveSolutionFinder3(refCube, (x,y)=>PrintSolution(x,y), eqFunc, depth, threads);

            StartPerformanceCounterThread(finder);

            var result = finder.Run();

            Console.WriteLine("================= FINISHED ============================");
            Console.WriteLine("Checked " + finder.Combinations + " combinations");            
        }

        void StartPerformanceCounterThread(RecursiveSolutionFinder3 finder)
        {
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

        void PrintSolution(Cube3 cube, List<List<string>> result)
        {
            lock (printLock)
            {
                if (result != null && result.Count > 0)
                {
                    Console.WriteLine($"Paths found: {result.Count}");
                    result.Sort((x, y) => x.Count.CompareTo(y.Count));
                    PrintSolution(cube, result[0]);
                }
                else
                {
                    Console.WriteLine("No moves found :(");
                }
            }
        }

        void PrintSolution(Cube3 cube, List<string> result)
        {
            lock (printLock)
            {
                if (result != null && result.Count > 0)
                {
                    Console.WriteLine();
                    for (int i = 0; i < result.Count; i++)
                    {
                        Console.Write((i > 0 ? ", " : "") + (i + 1) + ". " + result[i]);
                    }
                    Console.WriteLine();
                    Console.WriteLine();
                    cube = new Cube3(cube);
                    cube.Rotate(result[0]);

                    Console.WriteLine(cube.ToStringWithPieces());
                }
                else
                {
                    Console.WriteLine("No moves found :(");
                }
            }
        }
    }
}
