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
        /*
        int[,] piecesMaskCenterDiagonal = new int[12, 9]
        {
                { 0, 0, 0, 1, 1, 1, 0, 0, 0 },
                { 0, 0, 0, 1, 1, 1, 0, 0, 0 },
                { 0, 0, 0, 1, 1, 1, 0, 0, 0 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 0, 0, 0, 1, 1, 1, 0, 0, 0 },
                { 0, 0, 0, 1, 1, 1, 0, 0, 0 },
                { 0, 0, 0, 1, 1, 1, 0, 0, 0 },
                { 0, 0, 0, 1, 1, 1, 0, 0, 0 },
                { 0, 0, 0, 1, 1, 1, 0, 0, 0 },
                { 0, 0, 0, 1, 1, 1, 0, 0, 0 }
        };

        int[,] colorsMaskCenterDiagonal = new int[12, 9]
        {
                { 0, 0, 0, 1, 1, 1, 0, 0, 0 },
                { 0, 0, 0, 1, 1, 1, 0, 0, 0 },
                { 0, 0, 0, 1, 1, 1, 0, 0, 0 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 0, 0, 0, 1, 1, 1, 0, 0, 0 },
                { 0, 0, 0, 1, 1, 1, 0, 0, 0 },
                { 0, 0, 0, 1, 1, 1, 0, 0, 0 },
                { 0, 0, 0, 1, 1, 1, 0, 0, 0 },
                { 0, 0, 0, 1, 1, 1, 0, 0, 0 },
                { 0, 0, 0, 1, 1, 1, 0, 0, 0 }
        };
        */

        long operationCounter = 0;

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
            var taskResult = StartSolvingAsTasks(refCube);
            
            new Thread(() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(10));
                    var counter = Interlocked.Read(ref operationCounter);
                    Console.WriteLine(counter);                    
                }
            }).Start();

            var result = AggregateTaskResults(taskResult);

            PrintSolution(result, refCube);
        }

        private List<List<string>> AggregateTaskResults(List<Task<List<List<string>>>> taskResult)
        {
            var result = new List<List<string>>();
            foreach (var t in taskResult)
            {
                if (t.Result != null)
                    result.AddRange(t.Result);
            }

            return result;
        }

        private List<Task<List<List<string>>>> StartSolvingAsTasks(Cube3 refCube)
        {
            List<Task<List<List<string>>>> taskResult = new List<Task<List<List<string>>>>();
            for (char moveLetter = 'a'; moveLetter <= 'c'; moveLetter++)
            {
                for (char moveIdx = '1'; moveIdx <= '2'; moveIdx++)
                {
                    char c = moveLetter;
                    char i = moveIdx;                    
                    taskResult.Add(Task.Run(() =>
                    {
                        return RecursivelyRotateAndCompareInternal(
                            new Cube3(refCube),
                            new Cube3(refCube),
                            c,
                            i,
                            1,                            
                            new List<string>(),
                            10,
                            SwapEdgeNeighborsEqFunc(refCube));
                    }
                    ));
                }
            }

            return taskResult;
        }

        void PrintSolution(List<List<string>> result, Cube3 cube)
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
                cube = new Cube3(cube);
                cube.Rotate(result[0]);

                Console.WriteLine(cube.ToStringWithPieces());
            }
            else
            {
                Console.WriteLine("No moves found :(");
            }
        }

        Func<Cube3, bool> SwapCenterNeighborsEqFunc(Cube3 refCube)
        {
            int[,] piecesNotEqualsMask = new int[12, 9]
            {
                { 8, 8, 8, 0, 0, 0, 8, 8, 8 },
                { 8, 8, 8, 0, 0, 0, 8, 8, 8 },
                { 8, 8, 8, 0, 0, 0, 8, 8, 8 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 1, 1, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 0, 0, 0, 0 },
                { 8, 8, 8, 0, 1, 0, 8, 8, 8 },
                { 8, 8, 8, 0, 0, 0, 8, 8, 8 },
                { 8, 8, 8, 0, 0, 0, 8, 8, 8 },
                { 8, 8, 8, 0, 0, 0, 8, 8, 8 },
                { 8, 8, 8, 0, 0, 0, 8, 8, 8 },
                { 8, 8, 8, 0, 0, 0, 8, 8, 8 }
            };

            int[,] piecesEqualsMask = new int[12, 9]
            {
                { 8, 8, 8, 0, 1, 0, 8, 8, 8 },
                { 8, 8, 8, 1, 1, 1, 8, 8, 8 },
                { 8, 8, 8, 0, 1, 0, 8, 8, 8 },
                { 0, 1, 0, 0, 1, 0, 0, 1, 0 },
                { 1, 1, 0, 0, 1, 1, 1, 1, 1 },
                { 0, 1, 0, 0, 0, 0, 0, 1, 0 },
                { 8, 8, 8, 0, 0, 0, 8, 8, 8 },
                { 8, 8, 8, 1, 1, 1, 8, 8, 8 },
                { 8, 8, 8, 0, 1, 0, 8, 8, 8 },
                { 8, 8, 8, 0, 1, 0, 8, 8, 8 },
                { 8, 8, 8, 1, 1, 1, 8, 8, 8 },
                { 8, 8, 8, 0, 1, 0, 8, 8, 8 }
            };

            int[,] colorsMaskCenterNeighbor = new int[12, 9]
            {
                { 0, 0, 0, 0, 1, 0, 0, 0, 0 },
                { 0, 0, 0, 1, 1, 1, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 0, 0, 0, 0 },
                { 0, 1, 0, 0, 1, 0, 0, 1, 0 },
                { 1, 1, 0, 0, 1, 1, 1, 1, 1 },
                { 0, 1, 0, 0, 0, 0, 0, 1, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 1, 1, 1, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 0, 0, 0, 0 },
                { 0, 0, 0, 1, 1, 1, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 0, 0, 0, 0 }
            };

            return (cube) =>
            {
                return
                !cube.EqualsPieces(refCube, piecesNotEqualsMask) &&
                cube.EqualsPieces(refCube, piecesEqualsMask) &&                
                cube.EqualsColors(refCube, colorsMaskCenterNeighbor);
            };
        }

        Func<Cube3, bool> RotateColorsOfCenterNeighborsEqFunc(Cube3 refCube)
        {
            int[,] colorsNotEqualsMask = new int[12, 9]
            {
                { 8, 8, 8, 0, 0, 0, 8, 8, 8 },
                { 8, 8, 8, 0, 0, 0, 8, 8, 8 },
                { 8, 8, 8, 0, 0, 0, 8, 8, 8 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 1, 1, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 0, 0, 0, 0 },
                { 8, 8, 8, 0, 1, 0, 8, 8, 8 },
                { 8, 8, 8, 0, 0, 0, 8, 8, 8 },
                { 8, 8, 8, 0, 0, 0, 8, 8, 8 },
                { 8, 8, 8, 0, 0, 0, 8, 8, 8 },
                { 8, 8, 8, 0, 0, 0, 8, 8, 8 },
                { 8, 8, 8, 0, 0, 0, 8, 8, 8 }
            };

            int[,] piecesEqualsMask = new int[12, 9]
            {
                { 8, 8, 8, 0, 1, 0, 8, 8, 8 },
                { 8, 8, 8, 1, 1, 1, 8, 8, 8 },
                { 8, 8, 8, 0, 1, 0, 8, 8, 8 },
                { 0, 1, 0, 0, 1, 0, 0, 1, 0 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 0, 1, 0, 0, 1, 0, 0, 1, 0 },
                { 8, 8, 8, 0, 1, 0, 8, 8, 8 },
                { 8, 8, 8, 1, 1, 1, 8, 8, 8 },
                { 8, 8, 8, 0, 1, 0, 8, 8, 8 },
                { 8, 8, 8, 0, 1, 0, 8, 8, 8 },
                { 8, 8, 8, 1, 1, 1, 8, 8, 8 },
                { 8, 8, 8, 0, 1, 0, 8, 8, 8 }
            };

            int[,] colorsMaskCenterNeighbor = new int[12, 9]
            {
                { 0, 0, 0, 0, 1, 0, 0, 0, 0 },
                { 0, 0, 0, 1, 1, 1, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 0, 0, 0, 0 },
                { 0, 1, 0, 0, 1, 0, 0, 1, 0 },
                { 1, 1, 0, 0, 1, 1, 1, 1, 1 },
                { 0, 1, 0, 0, 0, 0, 0, 1, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 1, 1, 1, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 0, 0, 0, 0 },
                { 0, 0, 0, 1, 1, 1, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 0, 0, 0, 0 }
        };

            return (cube) =>
            {
                return
                cube.EqualsPieces(refCube, piecesEqualsMask) &&
                !cube.EqualsColors(refCube, colorsNotEqualsMask) &&
                cube.EqualsColors(refCube, colorsMaskCenterNeighbor);
            };
        }

        Func<Cube3, bool> SwapEdgeNeighborsEqFunc_HalfHourNoResult(Cube3 refCube)
        {
            int[,] piecesNotEqualsMask = new int[12, 9]
            {
                { 8, 8, 8, 0, 0, 0, 8, 8, 8 },
                { 8, 8, 8, 0, 0, 0, 8, 8, 8 },
                { 8, 8, 8, 0, 0, 0, 8, 8, 8 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 1, 1, 0, 1, 1, 0, 0 },
                { 8, 8, 8, 1, 0, 1, 8, 8, 8 },
                { 8, 8, 8, 0, 0, 0, 8, 8, 8 },
                { 8, 8, 8, 0, 0, 0, 8, 8, 8 },
                { 8, 8, 8, 0, 0, 0, 8, 8, 8 },
                { 8, 8, 8, 0, 0, 0, 8, 8, 8 },
                { 8, 8, 8, 0, 0, 0, 8, 8, 8 }
            };

            int[,] colorsEqualsMask = new int[12, 9]
            {
                { 8, 8, 8, 1, 1, 1, 8, 8, 8 },
                { 8, 8, 8, 1, 1, 1, 8, 8, 8 },
                { 8, 8, 8, 1, 1, 1, 8, 8, 8 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 0, 0, 1, 0, 0, 1, 1 },
                { 8, 8, 8, 0, 1, 0, 8, 8, 8 },
                { 8, 8, 8, 1, 1, 1, 8, 8, 8 },
                { 8, 8, 8, 1, 1, 1, 8, 8, 8 },
                { 8, 8, 8, 1, 1, 1, 8, 8, 8 },
                { 8, 8, 8, 1, 1, 1, 8, 8, 8 },
                { 8, 8, 8, 1, 1, 1, 8, 8, 8 }
            };
            
            return (cube) =>
            {
                return
                //!cube.EqualsPieces(refCube, piecesNotEqualsMask) &&
                (cube.Pieces[5,3] != refCube.Pieces[5,3] || cube.Pieces[5,5] != refCube.Pieces[5,5]) &&                
                cube.EqualsColors(refCube, colorsEqualsMask);
            };
        }

        Func<Cube3, bool> SwapEdgeNeighborsEqFunc(Cube3 refCube)
        {
            int[,] piecesEqualsMask = new int[12, 9]
            {
                { 8, 8, 8, 1, 1, 1, 8, 8, 8 },
                { 8, 8, 8, 1, 1, 1, 8, 8, 8 },
                { 8, 8, 8, 1, 1, 1, 8, 8, 8 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 0, 0, 1, 0, 0, 1, 1 },
                { 8, 8, 8, 0, 1, 0, 8, 8, 8 },
                { 8, 8, 8, 1, 1, 1, 8, 8, 8 },
                { 8, 8, 8, 1, 1, 1, 8, 8, 8 },
                { 8, 8, 8, 1, 1, 1, 8, 8, 8 },
                { 8, 8, 8, 1, 1, 1, 8, 8, 8 },
                { 8, 8, 8, 1, 1, 1, 8, 8, 8 }
            };

            int[,] piecesNotEqualsMask = new int[12, 9]
            {
                { 8, 8, 8, 0, 0, 0, 8, 8, 8 },
                { 8, 8, 8, 0, 0, 0, 8, 8, 8 },
                { 8, 8, 8, 0, 0, 0, 8, 8, 8 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 1, 1, 0, 1, 1, 0, 0 },
                { 8, 8, 8, 1, 0, 1, 8, 8, 8 },
                { 8, 8, 8, 0, 0, 0, 8, 8, 8 },
                { 8, 8, 8, 0, 0, 0, 8, 8, 8 },
                { 8, 8, 8, 0, 0, 0, 8, 8, 8 },
                { 8, 8, 8, 0, 0, 0, 8, 8, 8 },
                { 8, 8, 8, 0, 0, 0, 8, 8, 8 }
            };

            int[,] colorsEqualsMask = new int[12, 9]
            {
                { 8, 8, 8, 1, 1, 1, 8, 8, 8 },
                { 8, 8, 8, 1, 1, 1, 8, 8, 8 },
                { 8, 8, 8, 1, 1, 1, 8, 8, 8 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 0, 0, 1, 0, 0, 1, 1 },
                { 8, 8, 8, 0, 1, 0, 8, 8, 8 },
                { 8, 8, 8, 1, 1, 1, 8, 8, 8 },
                { 8, 8, 8, 1, 1, 1, 8, 8, 8 },
                { 8, 8, 8, 1, 1, 1, 8, 8, 8 },
                { 8, 8, 8, 1, 1, 1, 8, 8, 8 },
                { 8, 8, 8, 1, 1, 1, 8, 8, 8 }
            };

            var piecesEqualPoints = PointsToCompare(piecesEqualsMask);
            var piecesNotEqualPoints = PointsToCompare(piecesNotEqualsMask);

            return (cube) =>
            {
                return
                cube.EqualsPieces(refCube, piecesEqualPoints) &&
                !cube.EqualsPieces(refCube, piecesNotEqualPoints);
            };
        }

        IEnumerable<Cube3.XY> PointsToCompare(int[,] mask)
        {            
            var agg = new Dictionary<int, Cube3.XY>();
            var cube = new Cube3();
            for (int y = 0; y < mask.GetLength(0); y++)
            {
                for (int x = 0; x < mask.GetLength(1); x++)
                {
                    if (mask[y,x] == 1)
                    {
                        if (!agg.ContainsKey(cube.Pieces[y,x]))
                        {
                            agg.Add(cube.Pieces[y, x], new Cube3.XY(x, y));
                        }
                    }
                }
            }

            return agg.Values.ToArray();
        }

        List<List<string>> RecursivelyRotateAndCompare(
            Cube3 cube,
            Cube3 refCube,
            char previousMoveLetter,
            char previousMoveIdx,
            int depth,
            List<string> moves,
            int maxDepth,
            Func<Cube3, bool> eqFunc)
        {
            Interlocked.Increment(ref operationCounter);
            List<List<string>> result = null;
            if (depth != 1 && eqFunc(cube))
            {
                result = new List<List<string>>();
                result.Add(moves);
                PrintSolution(result, refCube);
                return result;
            }

            if (depth > maxDepth)
            {
                return result;
            }

            for (char moveLetter = 'a'; moveLetter <= 'c'; moveLetter++)
            {
                for (char moveIdx = '1'; moveIdx <= '2'; moveIdx++)
                {
                    if (moveLetter == previousMoveLetter && moveIdx == previousMoveIdx)
                    {
                        continue;
                    }

                    var resultToAdd = RecursivelyRotateAndCompareInternal(cube, refCube, moveLetter, moveIdx, depth, moves, maxDepth, eqFunc);
                    AccumulateResult(result, resultToAdd);
                }
            }

            return result;
        }

        List<List<string>> RecursivelyRotateAndCompareInternal(
            Cube3 cube,
            Cube3 refCube,
            char moveLetter,
            char moveIdx,
            int depth,
            List<string> moves,
            int maxDepth,
            Func<Cube3, bool> eqFunc)
        {
            List<List<string>> result = null;
            for (int count = 1; count <= 3; count++)
            {
                string move = $"{moveLetter}{moveIdx}";
                cube.Rotate(move, count);
                //List<string> newMoves = new List<string>(moves);
                //newMoves.Add(move + "/" + count);

                var resultToAdd = RecursivelyRotateAndCompare(cube, refCube, moveLetter, moveIdx, depth + 1, moves, maxDepth, eqFunc);
                AccumulateResult(result, resultToAdd);
                cube.Rotate(move, -count);
            }

            return result;
        }

        void AccumulateResult(List<List<string>> current, List<List<string>> resultToAdd)
        {
            if (resultToAdd != null)
            {
                if (current == null)
                {
                    current = resultToAdd;
                }
                else
                {
                    current.AddRange(resultToAdd);
                }
            }
        }
    }
}
