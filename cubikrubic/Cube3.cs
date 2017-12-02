using System;
using System.Collections.Generic;
using System.Linq;

namespace cubikrubic
{
    public class Cube3
    {
        const int COLORS = 6;
        const int COLORS_X = 9;
        const int COLORS_Y = 12;        
        const int EMPTY = 0;
        static readonly RotationDescriptor[,] rotationDescriptors;
        static readonly Dictionary<string, RotationDescriptor> rotationDescriptorsMap;

        int[,] pieces = new int[COLORS_Y, COLORS_X];
        int[,] colors = new int[COLORS_Y, COLORS_X];

        static Cube3()
        {
            rotationDescriptors = new RotationDescriptor[,] {
                {
                new RotationDescriptor(
                    "a1",
                    new XY[][] {
                        new [] { new XY(3,2), new XY(6,3), new XY(5,6), new XY(2,5)},
                        new [] { new XY(4,2), new XY(6,4), new XY(4,6), new XY(2,4)},
                        new [] { new XY(5,2), new XY(6,5), new XY(3,6), new XY(2,3)},
                        new [] { new XY(3,3), new XY(5,3), new XY(5,5), new XY(3,5)},
                        new [] { new XY(4,3), new XY(5,4), new XY(4,5), new XY(3,4)} }),
                new RotationDescriptor(
                    "a2",
                    new XY[][] {
                        new [] { new XY(3,1), new XY(7,3), new XY(5,7), new XY(1,5)},
                        new [] { new XY(4,1), new XY(7,4), new XY(4,7), new XY(1,4)},
                        new [] { new XY(5,1), new XY(7,5), new XY(3,7), new XY(1,3)}})
                },
                {
                new RotationDescriptor(
                    "b1",
                    new XY[][] {
                        new [] { new XY(3,2), new XY(3,11), new XY(3,8), new XY(3,5)},
                        new [] { new XY(3,1), new XY(3,10), new XY(3,7), new XY(3,4)},
                        new [] { new XY(3,0), new XY(3,9), new XY(3,6), new XY(3,3)},
                        new [] { new XY(2,3), new XY(0,3), new XY(0,5), new XY(2,5)},
                        new [] { new XY(2,4), new XY(1,3), new XY(0,4), new XY(1,5)} }),
                new RotationDescriptor(
                    "b2",
                    new XY[][] {
                        new [] { new XY(4,2), new XY(4,11), new XY(4,8), new XY(4,5)},
                        new [] { new XY(4,1), new XY(4,10), new XY(4,7), new XY(4,4)},
                        new [] { new XY(4,0), new XY(4,9), new XY(4,6), new XY(4,3)}})
                },
                {
                new RotationDescriptor(
                    "c1",
                     new XY[][] {
                        new [] { new XY(0,5), new XY(3,5), new XY(6,5), new XY(5,9)},
                        new [] { new XY(1,5), new XY(4,5), new XY(7,5), new XY(4,9)},
                        new [] { new XY(2,5), new XY(5,5), new XY(8,5), new XY(3,9)},
                        new [] { new XY(3,6), new XY(5,6), new XY(5,8), new XY(3,8)},
                        new [] { new XY(4,6), new XY(5,7), new XY(4,8), new XY(3,7)} }),
                new RotationDescriptor(
                    "c2",
                    new XY[][] {
                        new [] { new XY(0,4), new XY(3,4), new XY(6,4), new XY(5,10)},
                        new [] { new XY(1,4), new XY(4,4), new XY(7,4), new XY(4,10)},
                        new [] { new XY(2,4), new XY(5,4), new XY(8,4), new XY(3,10)}})
                }
            };
            rotationDescriptorsMap = new Dictionary<string, RotationDescriptor>();
            foreach (var r in rotationDescriptors)
            {
                rotationDescriptorsMap.Add(r.Name, r);
            }
            ValidateDescriptors(rotationDescriptors);
        }

        private static void ValidateDescriptors(RotationDescriptor[,] descriptors)
        {
            foreach(var descriptor in descriptors)
            {
                Dictionary<XY, object> uniquenessMap = new Dictionary<XY, object>();
                foreach(var moveList in descriptor.MovesList)
                {
                    foreach(var move in moveList)
                    {
                        uniquenessMap.Add(move, null);
                    }
                }
            }
        }

        public Cube3()
        {
            Fill(3, 0, 1, colors);
            Fill(0, 3, 2, colors);
            Fill(3, 3, 3, colors);
            Fill(6, 3, 4, colors);
            Fill(3, 6, 5, colors);
            Fill(3, 9, 6, colors);

            InitPieces();
            Validate();
        }
        
        public Cube3(string data)
        {
            int i = 0;
            for(int y = 0; y < COLORS_Y; y ++)
            {
                for(int x = 0; x < COLORS_X; x++)
                {
                    var c = data[i++];
                    colors[y, x] = c == ' ' ? 0 : c - '0';
                }
            }
            InitPieces();
            Validate();
        }
        
        public Cube3(Cube3 other)
        {
            for(int x = 0; x < COLORS_X; x++)
            {
                for(int y = 0; y < COLORS_Y; y++)
                {
                    colors[y, x] = other.colors[y, x];
                }
            }
            for (int x = 0; x < COLORS_X; x++)
            {
                for (int y = 0; y < COLORS_Y; y++)
                {
                    pieces[y, x] = other.pieces[y, x];
                }
            }
            Validate();
        }

        private void InitPieces()
        {
            pieces = new int[COLORS_Y, COLORS_X]
            {
                { 0, 0, 0,18,19,20, 0, 0, 0 },
                { 0, 0, 0,21,22,23, 0, 0, 0 },
                { 0, 0, 0,26, 1, 2, 0, 0, 0 },
                {18,21,26,26, 1, 2, 2,23,20 },
                {15,24, 3, 3, 4, 5, 5,25,17 },
                {12, 9, 6, 6, 7, 8, 8,11,14 },
                { 0, 0, 0, 6, 7, 8, 0, 0, 0 },
                { 0, 0, 0, 9,10,11, 0, 0, 0 },
                { 0, 0, 0,12,13,14, 0, 0, 0 },
                { 0, 0, 0,12,13,14, 0, 0, 0 },
                { 0, 0, 0,15,16,17, 0, 0, 0 },
                { 0, 0, 0,18,19,20, 0, 0, 0 }
            };
        }

        public int[,] Pieces { get { return pieces; } }
        public int[,] Colors { get { return colors; } }

        public IEnumerable<string> RotationNames
        {
            get { return rotationDescriptorsMap.Keys; }
        }

        private void Fill(int x1, int y1, int value, int[,] array)
        {
            for (int x = x1; x <= x1 + 2; x++)
            {
                for (int y = y1; y <= y1 + 2; y++)
                {
                    array[y, x] = value;
                }
            }
        }

        public void Rotate(IEnumerable<string> moves)
        {
            foreach (var move in moves)
            {
                if (move.Length == 4)
                    Rotate(move[0], move[1], move[3] - '0');
                else if (move.Length == 2)
                    Rotate(move);
                else
                    throw new Exception($"Unknown move: {move}");
            }
        }

        #region Rotate function

        static int[] countOneIndexes = new[] { 0, 3, 2, 1 };
        static int[] countMinusOneIndexes = new[] { 0, 1, 2, 3 };

        public void Rotate(string name, int count = 1)
        {
            Rotate(rotationDescriptorsMap[name], count);
        }

        public void Rotate(char letter, char idx, int count)
        {
            Rotate(rotationDescriptors[letter - 'a', idx - '1'], count);
        }

        void Rotate(RotationDescriptor rotationDescriptor, int count)
        { 
            if (count == 2 || count == -2)
            {
                foreach(var move in rotationDescriptor.MovesList)
                {
                    RotateCount2(move);
                }                
            }
            else
            {
                if (count == 3)
                {
                    count = -1;
                }
                else if (count == -3)
                {
                    count = 1;
                }
                var indexes = count == 1 ? countOneIndexes : countMinusOneIndexes;

                foreach (var move in rotationDescriptor.MovesList)
                {                    
                    RotateCountOneOrMinusOne(move, indexes);
                }                
            }
        }        

        private void RotateCountOneOrMinusOne(XY[] moves, int[] indexes)
        {
            var move0 = moves[indexes[0]];
            var move1 = moves[indexes[1]];
            var move2 = moves[indexes[2]];
            var move3 = moves[indexes[3]];
            var tmp = colors[move0.Y, move0.X];
            colors[move0.Y, move0.X] = colors[move1.Y, move1.X];
            colors[move1.Y, move1.X] = colors[move2.Y, move2.X];
            colors[move2.Y, move2.X] = colors[move3.Y, move3.X];
            colors[move3.Y, move3.X] = tmp;
            tmp = pieces[move0.Y, move0.X];
            pieces[move0.Y, move0.X] = pieces[move1.Y, move1.X];
            pieces[move1.Y, move1.X] = pieces[move2.Y, move2.X];
            pieces[move2.Y, move2.X] = pieces[move3.Y, move3.X];
            pieces[move3.Y, move3.X] = tmp;
        }

        private void RotateCount2(XY[] moves)
        {
            var move0 = moves[0];
            var move1 = moves[1];
            var move2 = moves[2];
            var move3 = moves[3];
            Swap(colors, move0, move2);
            Swap(colors, move1, move3);
            Swap(pieces, move0, move2);
            Swap(pieces, move1, move3);
        }
        

        #endregion        

        private void Swap(int[,] array, XY a, XY b)
        {
            int tmp = array[a.Y, a.X];
            array[a.Y, a.X] = array[b.Y, b.X];
            array[b.Y, b.X] = tmp;
        }

        private void Validate()
        {
            return;
#if DEBUG
            int[] validation = new int[COLORS + 1];
            for (int x = 0; x < COLORS_X; x++)
            {
                for (int y = 0; y < COLORS_Y; y++)
                {
                    validation[colors[y, x]]++;
                    if (colors[y, x] == 0 && pieces[y, x] != 0 ||
                        colors[y, x] != 0 && pieces[y, x] == 0)
                        throw new Exception("out of bound element");
                }
            }
            if (validation[0] != 54)
            {
                throw new Exception("cube not valid, there are more than expected number out of bound fields");
            }
            for (int i = 1; i < validation.Length; i++)
            {
                if (validation[i] != 9)
                {
                    Console.WriteLine(ToString());
                    throw new Exception($"cube not valid, have {validation[i]} occurences of {i} color");
                }
            }

            validation = new int[pieces.GetLength(0) * pieces.GetLength(1)];
            for(int y = 0; y < COLORS_Y; y++)
            {
                for(int x = 0; x < COLORS_X; x++)
                {
                    validation[pieces[y, x]]++;
                }
            }
            Dictionary<int, int> v = new Dictionary<int, int>();
            for(int i = 0; i < validation.Length; i++)
            {
                if (validation[i] != 0 || validation[i] != 3)
                {
                    if (!v.ContainsKey(validation[i]))
                        v.Add(validation[i], 0);
                    v[validation[i]]++;
                }
            }
            if (v[0] != 81 || v[1]!=6 || v[2]!=12 || v[3]!=8 || v[54] != 1 || v.Count!=5)
            {
                throw new Exception("pieces not valid");
            }
#endif
        }

        public override string ToString()
        {
            return ToStringInternal(colors);
        }

        public string ToStringWithPieces()
        {
            return "Cube:" + Environment.NewLine + 
                ToString() +
                "Pieces:" + Environment.NewLine +
                ToStringPieces();            
        }

        public string ToStringPieces()
        {
            return ToStringInternal(pieces);
        }

        private string ToStringInternal(int[,] array)
        {
            string result = "";
            for (int y = 0; y < COLORS_Y; y++)
            {
                for (int x = 0; x < COLORS_X; x++)
                {
                    result += (array[y, x] == EMPTY ? "" : array[y, x].ToString()).PadLeft(3);
                }
                result += Environment.NewLine;
            }

            return result;
        }

        public override bool Equals(object obj)
        {
            Cube3 other = (Cube3)obj;            
            return EqualsColors(other);
        }

        public override int GetHashCode()
        {
            return colors.GetHashCode();
        }

        public bool EqualsPieces(Cube3 other, int[,] mask = null)
        {            
            return ArrayEqualsUsingMask(pieces, other.pieces, mask);
        }

        public bool EqualsColors(Cube3 other, int[,] mask = null)
        {
            return ArrayEqualsUsingMask(colors, other.colors, mask);
        }

        public bool EqualsColors(Cube3 other, IEnumerable<XY> points)
        {
            return ArrayEqualsUsingPoints(colors, other.colors, points);
        }

        public bool EqualsPieces(Cube3 other, IEnumerable<XY> points)
        {
            return ArrayEqualsUsingPoints(pieces, other.pieces, points);
        }

        private bool ArrayEqualsUsingPoints(int[,] a, int[,] b, IEnumerable<XY> points)
        {
            foreach (var point in points)
            {
                if (a[point.Y, point.X] != b[point.Y, point.X])
                    return false;
            }
            return true;
        }

        private bool ArrayEqualsUsingMask(int [,] a, int[,] b, int[,] mask)
        {
            if (mask != null && (mask.GetLength(0) != a.GetLength(0) || mask.GetLength(1) != a.GetLength(1)))
                throw new Exception("mask has incorrect dimensions");

            for (int y = 0; y < COLORS_Y; y++) 
            {
                for (int x = 3; x < 6; x++)
                {
                    if (mask != null && (mask[y, x] == 0 || mask[y,x] == 8))
                        continue;
                    else if (a[y, x] != b[y, x])
                        return false;                    
                }
            }
            for (int y = 3; y < 6; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    if (mask != null && (mask[y, x] == 0 || mask[y, x] == 8))
                        continue;
                    else if (a[y, x] != b[y, x])
                        return false;
                }
                for (int x = 6; x < 9; x++)
                {
                    if (mask != null && (mask[y, x] == 0 || mask[y, x] == 8))
                        continue;
                    else if (a[y, x] != b[y, x])
                        return false;
                }
            }            
            return true;
        }

        public struct XY
        {
            public readonly int X;
            public readonly int Y;

            public XY(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public override string ToString()
            {
                return $"{X},{Y}";
            }
        }

        public static IEnumerable<XY> MaskToPoints(int[,] mask)
        {
            var agg = new Dictionary<int, XY>();
            var cube = new Cube3();
            for (var y = 0; y < mask.GetLength(0); y++)
            {
                for (var x = 0; x < mask.GetLength(1); x++)
                {
                    if (mask[y, x] == 1)
                    {
                        if (!agg.ContainsKey(cube.Pieces[y, x]))
                        {
                            agg.Add(cube.Pieces[y, x], new XY(x, y));
                        }
                    }
                }
            }

            return agg.Values.ToArray();
        }

        private struct RotationDescriptor
        {
            public readonly string Name;
            public readonly XY[][] MovesList;            

            public RotationDescriptor(string name, XY[][] movesList)
            {
                this.Name = name;
                this.MovesList = movesList;                
            }

            public override string ToString()
            {
                return Name;
            }
        }
    }
}
