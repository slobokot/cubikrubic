using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cubikrubic
{
    class SolutionFinder2
    {
        int[,] piecesMaskDiagonal = new int[4, 2]
        {
                {0, 1},
                {1, 0},
                {1, 1},
                {1, 1}
        };

        int[,] colorsMaskDiagonal = new int[8, 6]
        {
                { 0, 0, 1, 1, 0, 0 },
                { 0, 0, 0, 1, 0, 0 },
                { 1, 0, 0, 1, 1, 1 },
                { 1, 1, 1, 0, 0, 1 },
                { 0, 0, 1, 0, 0, 0 },
                { 0, 0, 1, 1, 0, 0 },
                { 0, 0, 1, 1, 0, 0 },
                { 0, 0, 1, 1, 0, 0 },
        };

        int[,] piecesMaskNeighbor = new int[4, 2]
        {
                {1, 1},
                {0, 0},
                {1, 1},
                {1, 1}
        };

        int[,] colorsMaskNeighbor = new int[8, 6]
        {
                { 0, 0, 1, 1, 0, 0 },
                { 0, 0, 1, 1, 0, 0 },
                { 1, 1, 1, 1, 1, 1 },
                { 1, 0, 0, 0, 0, 1 },
                { 0, 0, 0, 0, 0, 0 },
                { 0, 0, 1, 1, 0, 0 },
                { 0, 0, 1, 1, 0, 0 },
                { 0, 0, 1, 1, 0, 0 },
        };

        public void ManualRotationCheck()
        {
            var cube = PrepareCubeAndPrint();
            var moves = new[] { "a1/1", "c1/2", "a1/3", "b1/3", "a1/2", "b1", "c1/2", "b1/3", "a1/3", "c1/2" };
            cube.Rotate(moves);
            Console.WriteLine(cube.ToStringWithPieces());
        }

        public void FindSolutionWithTimer()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            FindSolution();

            stopwatch.Stop();
            Console.WriteLine("Time required: " + stopwatch.Elapsed);
        }


        Cube2 PrepareCube()
        {
            var cube = new Cube2();
            cube.Rotate("a1", 1);
            cube.Rotate("b1", 1);
            cube.Rotate("c1", 1);
            return cube;
        }

        Cube2 PrepareCubeAndPrint()
        {
            var cube = new Cube2();
            cube.Rotate("a1", 1);
            cube.Rotate("b1", 1);
            cube.Rotate("c1", 1);
            Console.WriteLine(cube.ToStringWithPieces());
            return cube;
        }


        void FindSolution()
        {
            var refCube = PrepareCubeAndPrint();

            List<Task<List<List<string>>>> taskResult = new List<Task<List<List<string>>>>();
            for (char c = 'a'; c <= 'c'; c++)
            {
                taskResult.Add(Task.Run(() =>
                {
                    return RecursivelyRotateAndCompare(new Cube2(refCube), c, 1, new Cube2(refCube), new List<string>(), colorsMaskNeighbor, piecesMaskNeighbor, 10, SwapPiecesEqFunc());
                }
                ));
            }

            List<List<string>> result = new List<List<string>>();
            foreach (var t in taskResult)
            {
                if (t.Result != null)
                    result.AddRange(t.Result);
            }

            PrintSolution(result, refCube);
        }

        void PrintSolution(List<List<string>> result, Cube2 cube)
        {
            if (result != null && result.Count > 0)
            {
                result.Sort((x, y) => x.Count.CompareTo(y.Count));
                Console.WriteLine($"Paths found: {result.Count}");
                Console.WriteLine();
                for (int i = 0; i < result[0].Count; i++)
                {
                    Console.Write((i > 0 ? ", " : "") + (i + 1) + ". " + result[0][i]);
                }
                Console.WriteLine();
                Console.WriteLine();
                cube.Rotate(result[0]);

                Console.WriteLine(cube.ToStringWithPieces());
            }
            else
            {
                Console.WriteLine("No moves found :(");
            }
        }

        Func<Cube2, Cube2, int[,], int[,], bool> SwapPiecesEqFunc()
        {
            return (cube, refCube, colorMask, piecesMask) => {
                return
                !cube.EqualsPiecesOnly(refCube) &&
                cube.EqualUsingMaskPiecesOnly(refCube, piecesMask) &&
                !cube.Equals(refCube) &&
                cube.EqualUsingMask(refCube, colorMask);
            };
        }

        Func<Cube2, Cube2, int[,], int[,], bool> RotateColorsOnlyEqFunc()
        {
            return (cube, refCube, colorMask, piecesMask) => {
                return
                cube.EqualsPiecesOnly(refCube) &&
                !cube.Equals(refCube) &&
                cube.EqualUsingMask(refCube, colorMask);
            };
        }

        List<List<string>> RecursivelyRotateAndCompare(
            Cube2 cube,
            char previousCh,
            int depth,
            Cube2 refCube,
            List<string> moves,
            int[,] colorMask,
            int[,] piecesMask,
            int maxDepth,
            Func<Cube2, Cube2, int[,], int[,], bool> eqFunc)
        {
            List<List<string>> result = null;
            if (depth != 1 && eqFunc(cube, refCube, colorMask, piecesMask))
            {
                result = new List<List<string>>();
                result.Add(moves);
                return result;
            }

            if (depth > maxDepth)
            {
                return result;
            }

            for (var ch = 'a'; ch <= 'c'; ch++)
            {
                if (ch == previousCh)
                {
                    continue;
                }

                for (int count = 1; count <= 3; count++)
                {
                    string move = ch + "1";
                    cube.Rotate(move, count);
                    List<string> newMoves = new List<string>(moves);
                    newMoves.Add(move + "/" + count);

                    var resultToAdd = RecursivelyRotateAndCompare(cube, ch, depth + 1, refCube, newMoves, colorMask, piecesMask, maxDepth, eqFunc);
                    if (resultToAdd != null)
                    {
                        if (result == null)
                            result = resultToAdd;
                        else
                            result.AddRange(resultToAdd);
                    }
                    cube.Rotate(move, -count);
                }
            }

            return result;
        }
    }
}
