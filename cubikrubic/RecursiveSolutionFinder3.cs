using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        readonly Action<Cube3, IEnumerable<string>> print;
        readonly Func<Cube3, Cube3, bool> eqFunc;
        readonly int maxThreads;
        readonly Cube3 refCube;
        readonly string[] moves;
        int activeThreadCount = 1;
        long combinationCounter = 0;

        public RecursiveSolutionFinder3(Cube3 cube, Action<Cube3, IEnumerable<string>> print, Func<Cube3, Cube3, bool> eqFunc, int maxDepth, int maxThreads, string[] moves = null)
        {
            this.print = print;
            this.eqFunc = eqFunc;
            this.maxThreads = maxThreads;
            this.refCube = new Cube3(cube);
            this.moves = new string[maxDepth];
            if (moves !=null)
            {
                for(int i = 0; i < moves.Length; i++)
                {
                    if (moves[i] == null)
                        break;
                    this.moves[i] = moves[i];
                }
            }
            
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

        public void Run()
        {
            int startDepth = 0;
            while(startDepth < moves.Length && moves[startDepth]!=null)
            {
                startDepth++;
            }

            if (maxThreads > 1)
            {                
                var spawnMovesList = GetMovesList(new Cube3(refCube), '-', '-', startDepth, moves);
                var processList = new List<Process>();
                var argumentList = new List<string>();
                foreach(var spawnMoves in spawnMovesList)
                {
                    var argument = "--child ";
                    foreach (var move in spawnMoves)
                    {
                        if (move == null)
                            break;
                        argument += move;
                    }
                    argumentList.Add(argument);
                }
                
                while(argumentList.Count > 0)
                {
                    while (processList.Count < maxThreads)
                    {
                        var argument = argumentList[0];
                        argumentList.RemoveAt(0);
                        processList.Add(Process.Start("cubikrubic.exe", argument));
                    }

                    while(processList.Count == maxThreads)
                    {
                        for(int i = 0; i < processList.Count; i++)
                        {
                            if (processList[i].HasExited)
                            {
                                processList.RemoveAt(i);
                                i--;
                            }
                        }
                        if (processList.Count == maxThreads)
                            Thread.Sleep(TimeSpan.FromSeconds(5));
                    }
                }
            }
            else
            {                
                RecursivelyRotateAndCompare(new Cube3(refCube), '-', '-', startDepth, moves);
            }
        }

        string ToString(char moveLetter, char moveIdx)
        {
            return letterIndexStrings[moveLetter - 'a', moveIdx - '1'];
        }

        string ToString(char moveLetter, char moveIdx, int count)
        {
            return letterIndexCountStrings[moveLetter - 'a', moveIdx - '1', count - 1];
        }

        void RecursivelyRotateAndCompare(Cube3 cube, char prevMoveLetter, char prevMoveIdx, int depth, string[] moves)
        {            

            Interlocked.Increment(ref combinationCounter);

            if (eqFunc(cube, refCube))
            {
                print(refCube, moves.ToList());
            }

            if (depth >= moves.Length)
            {
                return;
            }

            for (char moveLetter = 'a'; moveLetter <= 'c'; moveLetter++)
            {
                if (moveLetter == prevMoveLetter && prevMoveIdx == '2')
                {
                    continue;
                }

                for (char moveIdx = '1'; moveIdx <= '2'; moveIdx++)
                {
                    if (moveLetter == prevMoveLetter && moveIdx == prevMoveIdx)
                    {
                        continue;
                    }

                    for (int count = 1; count <= 3; count++)
                    {
                        cube.Rotate(moveLetter, moveIdx, 1);
                        moves[depth] = ToString(moveLetter, moveIdx, count);

                        RecursivelyRotateAndCompare(cube, moveLetter, moveIdx, depth + 1, moves);
                    }

                    cube.Rotate(moveLetter, moveIdx, 1);
                }
            }

            moves[depth] = null;
        }


        List<List<string>> GetMovesList(Cube3 cube, char prevMoveLetter, char prevMoveIdx, int depth, string[] moves)
        {
            List<List<string>> result = new List<List<string>>();
         
            for (char moveLetter = 'a'; moveLetter <= 'c'; moveLetter++)
            {
                if (moveLetter == prevMoveLetter && prevMoveIdx == '2')
                {
                    continue;
                }

                for (char moveIdx = '1'; moveIdx <= '2'; moveIdx++)
                {
                    if (moveLetter == prevMoveLetter && moveIdx == prevMoveIdx)
                    {
                        continue;
                    }

                    for (int count = 1; count <= 3; count++)
                    {
                        cube.Rotate(moveLetter, moveIdx, 1);
                        moves[depth] = ToString(moveLetter, moveIdx, count);

                        result.Add(moves.ToList());
                    }

                    cube.Rotate(moveLetter, moveIdx, 1);
                }
            }

            moves[depth] = null;

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
