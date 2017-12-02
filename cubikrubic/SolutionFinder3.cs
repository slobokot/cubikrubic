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

        public SolutionFinder3(Func<Cube3,Cube3,bool> eqFunc, int threads)
        {
            this.eqFunc = eqFunc;
            this.threads = threads;
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

            var finder = new RecursiveSolutionFinder3(refCube, (x,y)=>PrintSolution(x,y), eqFunc, maxDepth: 11, maxThreads: threads);

            new Thread(() =>
            {
                long combinations = 0;
                for (int i = 0; i < 10; i++)
                {
                    int seconds = 10;
                    Thread.Sleep(TimeSpan.FromSeconds(seconds));
                    var newcombinations = finder.Combinations;
                    Console.WriteLine((newcombinations - combinations) / seconds + " per sec");
                    combinations = newcombinations;
                }
            }).Start();

            var result = finder.Run();
            Console.WriteLine("================= FINISHED ============================");
            Console.WriteLine("Checked " + finder.Combinations + " combinations");
            PrintSolution(refCube, result);
        }


        void PrintSolution(Cube3 cube, List<List<string>> result)
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

        void PrintSolution(Cube3 cube, List<string> result)
        {
            if (result != null && result.Count > 0)
            {                                
                Console.WriteLine();
                for (int i = 0; i < result.Count; i++)
                {
                    Console.Write((i > 0 ? ", " : "") + (i + 1) + ". " + result[0][i]);
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
