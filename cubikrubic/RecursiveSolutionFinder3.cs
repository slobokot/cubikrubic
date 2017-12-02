using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cubikrubic
{
    class RecursiveSolutionFinder3
    {
        readonly string[,,] letterIndexCountStrings = new string[3, 3, 3];
        readonly string[,] letterIndexStrings = new string[3, 3];
        readonly Action<Cube3, List<string>> print;
        readonly Func<Cube3, Cube3, bool> eqFunc;
        readonly int maxThreads;
        readonly Cube3 refCube;
        readonly int maxDepth;
        volatile int activeThreadCount = 1;
        long combinationCounter = 0;        

        public RecursiveSolutionFinder3(Cube3 cube, Action<Cube3, List<string>> print, Func<Cube3, Cube3, bool> eqFunc, int maxDepth, int maxThreads)
        {
            this.print = print;
            this.eqFunc = eqFunc;
            this.maxThreads = maxThreads;
            this.refCube = new Cube3(cube);
            this.maxDepth = maxDepth;

            
            for (char moveLetter = 'a'; moveLetter <= 'c'; moveLetter++)
            {
                for (char moveIdx = '1'; moveIdx <= '2'; moveIdx++)
                {
                    letterIndexStrings[moveLetter - 'a', moveIdx - '1'] = $"{moveLetter}{moveIdx}";
                    for(int count = 1; count <= 3; count++)
                    {
                        letterIndexCountStrings[moveLetter - 'a', moveIdx - '1', count - 1] = $"{moveLetter}{moveIdx}/{count}";
                    }
                }
            }
        }

        public long Combinations
        {
            get
            {
                return Interlocked.Read(ref combinationCounter);
            }
        }

        public List<List<string>> Run()
        {
            return RecursivelyRotateAndCompare(new Cube3(refCube), '-', '-', '-', 0, new string[maxDepth], false);
        }

        private string ToString(char moveLetter, char moveIdx)
        {
            return letterIndexStrings[moveLetter - 'a', moveIdx - '1'];
        }

        private string ToString(char moveLetter, char moveIdx, int count)
        {
            return letterIndexCountStrings[moveLetter - 'a', moveIdx - '1', count - 1];
        }

        List<List<string>> RecursivelyRotateAndCompare(Cube3 cube, char previousMoveLetter, char previousMoveIdx, char beforePreviousMoveLetter, int depth, string[] moves, bool executedInSeparateThread)
        {            
            Interlocked.Increment(ref combinationCounter);            
            List<List<string>> result = null;
            if (eqFunc(cube, refCube))
            {
                result = new List<List<string>>();
                result.Add(moves.ToList());
                print(refCube, result[0]);
                return result;
            }

            if (depth >= moves.Length)
            {
                return result;
            }

            List<Task<List<List<string>>>> tasks = null;

            for (char moveLetter = 'a'; moveLetter <= 'c'; moveLetter++)
            {
                if (moveLetter == beforePreviousMoveLetter)
                {
                    continue;
                }

                for (char moveIdx = '1'; moveIdx <= '2'; moveIdx++)
                {
                    if (moveLetter == previousMoveLetter && moveIdx == previousMoveIdx)
                    {
                        continue;
                    }

                    for (int count = 1; count <= 3; count++)
                    {
                        string move = ToString(moveLetter, moveIdx);
                        cube.Rotate(move, count);
                        moves[depth] = ToString(moveLetter, moveIdx, count);

                        if (activeThreadCount < maxThreads)
                        {                            
                            int newThreadNumber = Interlocked.Increment(ref activeThreadCount);                            

                            if (tasks == null)
                            {
                                tasks = new List<Task<List<List<string>>>>();
                            }
                            var cubeClone = new Cube3(cube);
                            var moveLetterClone = moveLetter;
                            var moveIdxClone = moveIdx;
                            var previousMoveLetterClone = previousMoveLetter;
                            var movesClone = (string[])moves.Clone();
                            tasks.Add(Task.Run(() =>
                            {
                                return RecursivelyRotateAndCompare(cubeClone, moveLetterClone, moveIdxClone, previousMoveLetter, depth + 1, movesClone, true);                                
                            }));
                        }
                        else
                        {
                            var resultToAdd = RecursivelyRotateAndCompare(cube, moveLetter, moveIdx, previousMoveLetter, depth + 1, moves, false);
                            AccumulateResult(result, resultToAdd);
                        }

                        moves[depth] = null;
                        cube.Rotate(move, -count);
                    }
                }
            }

            if (executedInSeparateThread)
            {                
                Interlocked.Decrement(ref activeThreadCount);
            }

            if (tasks != null)
            {
                tasks.ForEach(x => AccumulateResult(result, x.Result));
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
