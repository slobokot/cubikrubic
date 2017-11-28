using System;
using System.Collections.Generic;

namespace cubikrubic
{
    public class Cube2
    {
        const int COLORS = 6;
        const int COLORS_X = 6;
        const int COLORS_Y = 8;
        const int PIECE_X = 2;
        const int PIECE_Y = 4;
        const int EMPTY = 0;
        static readonly RotationDescriptor[] rotationDescriptors;
        static readonly Dictionary<string, RotationDescriptor> rotationDescriptorsMap;

        int[,] pieces = new int[PIECE_Y, PIECE_X];
        int[,] colors = new int[COLORS_Y, COLORS_X];

        static Cube2()
        {
            rotationDescriptors = new RotationDescriptor[] {
                new RotationDescriptor(
                    "a1", 
                    new XY[][] {
                        new [] { new XY(2,1), new XY(4,2), new XY(3,4), new XY(1,3)},
                        new [] { new XY(3,1), new XY(4,3), new XY(2,4), new XY(1,2)},
                        new [] { new XY(2,2), new XY(3,2), new XY(3,3), new XY(2,3)}},
                    new XY[] { new XY(0, 0), new XY(1, 0), new XY(1, 1), new XY(0, 1) }),
                new RotationDescriptor(
                    "a2", 
                    new XY[][] {
                        new [] { new XY(2,0), new XY(5,2), new XY(3,5), new XY(0,3)},
                        new [] { new XY(3,0), new XY(5,3), new XY(2,5), new XY(0,2)},
                        new [] { new XY(2,6), new XY(2,7), new XY(3,7), new XY(3,6)}},
                    new XY [] { new XY(0,2), new XY(1,2), new XY(1,3), new XY(0,3) }),
                new RotationDescriptor(
                    "b1", 
                    new XY[][] {
                        new [] { new XY(2,1), new XY(2,7), new XY(2,5), new XY(2,3)},
                        new [] { new XY(2,0), new XY(2,6), new XY(2,4), new XY(2,2)},
                        new [] { new XY(0,2), new XY(0,3), new XY(1,3), new XY(1,2)}}, 
                    new XY [] { new XY(0,0), new XY(0,3), new XY(0,2), new XY(0,1) }),
                new RotationDescriptor(
                    "b2", 
                    new XY[][] {
                        new [] { new XY(3,1), new XY(3,7), new XY(3,5), new XY(3,3)},
                        new [] { new XY(3,0), new XY(3,6), new XY(3,4), new XY(3,2)},
                        new [] { new XY(4,3), new XY(4,2), new XY(5,2), new XY(5,3)}}, 
                    new XY [] { new XY(1,0), new XY(1,3), new XY(1,2), new XY(1,1) }),
                new RotationDescriptor(
                    "c1", 
                    new XY[][] {
                        new [] { new XY(0,3), new XY(2,3), new XY(4,3), new XY(3,6)},
                        new [] { new XY(1,3), new XY(3,3), new XY(5,3), new XY(2,6)},
                        new [] { new XY(2,4), new XY(3,4), new XY(3,5), new XY(2,5)}},
                    new XY [] { new XY(0,1), new XY(1,1), new XY(1,2), new XY(0,2) }),
                new RotationDescriptor(
                    "c2", 
                    new XY[][] {
                        new [] { new XY(0,2), new XY(2,2), new XY(4,2), new XY(3,7)},
                        new [] { new XY(1,2), new XY(3,2), new XY(5,2), new XY(2,7)},
                        new [] { new XY(2,1), new XY(3,1), new XY(3,0), new XY(3,2)}}, 
                    new XY [] { new XY(0,0), new XY(1,0), new XY(1,3), new XY(0,3) })
            };
            rotationDescriptorsMap = new Dictionary<string, RotationDescriptor>();
            foreach (var r in rotationDescriptors)
            {
                rotationDescriptorsMap.Add(r.Name, r);
            }
        }

        public Cube2()
        {
            FillSide(2, 0, 1);
            FillSide(0, 2, 2);
            FillSide(2, 2, 3);
            FillSide(4, 2, 4);
            FillSide(2, 4, 5);
            FillSide(2, 6, 6);

            InitPieces();

            Validate();
        }
        
        public Cube2(string data)
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
        
        public Cube2(Cube2 other)
        {
            for(int x = 0; x < COLORS_X; x++)
            {
                for(int y = 0; y < COLORS_Y; y++)
                {
                    colors[y, x] = other.colors[y, x];
                }
            }
            for (int x = 0; x < PIECE_X; x++)
            {
                for (int y = 0; y < PIECE_Y; y++)
                {
                    pieces[y, x] = other.pieces[y, x];
                }
            }
        }

        private void InitPieces()
        {
            int k = 0;
            for (int y = 0; y < PIECE_Y; y++)                
            {
                for (int x = 0; x < PIECE_X; x++)
                {
                    pieces[y, x] = k++;
                }
            }
        }

        public IEnumerable<string> RotationNames
        {
            get { return rotationDescriptorsMap.Keys; }
        }

        private void FillSide(int x1, int y1, int color)
        {
            for (int x = x1; x <= x1 + 1; x++)
            {
                for (int y = y1; y <= y1 + 1; y++)
                {
                    colors[y, x] = color;
                }
            }
        }

        public void Rotate(IEnumerable<string> moves)
        {
            foreach (var move in moves)
            {
                if (move.Length == 4)
                    Rotate(move.Substring(0, 2), move[3] - '0');
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
            var rotationDescriptor = rotationDescriptorsMap[name];

            if (count == 2 || count == -2)
            {
                for (int r = 0; r < rotationDescriptor.MovesList.Length; r++)
                {                    
                    RotateCount2(colors, rotationDescriptor.MovesList[r]);                    
                }

                RotateCount2(pieces, rotationDescriptor.CubeMoves);
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
                int[] indexes = count == 1 ? countOneIndexes : countMinusOneIndexes;

                for (int r = 0; r < rotationDescriptor.MovesList.Length; r++)
                {                    
                    RotateCountOneOrMinusOne(colors, rotationDescriptor.MovesList[r], indexes); 
                }
                RotateCountOneOrMinusOne(pieces, rotationDescriptor.CubeMoves, indexes);
            }

            Validate();
        }

        private void RotateCountOneOrMinusOne(int[,] array, XY[] moves, int[] indexes)
        {
            int tmp = array[moves[indexes[0]].Y, moves[indexes[0]].X];
            array[moves[indexes[0]].Y, moves[indexes[0]].X] = array[moves[indexes[1]].Y, moves[indexes[1]].X];
            array[moves[indexes[1]].Y, moves[indexes[1]].X] = array[moves[indexes[2]].Y, moves[indexes[2]].X];
            array[moves[indexes[2]].Y, moves[indexes[2]].X] = array[moves[indexes[3]].Y, moves[indexes[3]].X];
            array[moves[indexes[3]].Y, moves[indexes[3]].X] = tmp;
        }

        private void RotateCount2(int[,] array, XY[] moves)
        {
            Swap(array, moves[0], moves[2]);
            Swap(array, moves[1], moves[3]);
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
#if DEBUG
            int[] validation = new int[COLORS + 1];
            for (int x = 0; x < COLORS_X; x++)
            {
                for (int y = 0; y < COLORS_Y; y++)
                {
                    validation[colors[y, x]]++;
                }
            }
            if (validation[0] != 24)
            {
                throw new Exception("cube not valid");
            }
            for (int i = 1; i < validation.Length; i++)
            {
                if (validation[i] != 4)
                {
                    Console.WriteLine(ToString());
                    throw new Exception("cube not valid");
                }
            }

            validation = new int[pieces.GetLength(0) * pieces.GetLength(1)];
            for(int y = 0; y < PIECE_Y; y++)
            {
                for(int x = 0; x < PIECE_X; x++)
                {
                    validation[pieces[y, x]]++;
                }
            }
            for(int i = 0; i < validation.Length; i++)
            {
                if (validation[i] != 1)
                {
                    throw new Exception("cube is not valid");
                }
            }
#endif
        }

        public override string ToString()
        {
            string result = "";
            for (int y = 0; y < COLORS_Y; y++)
            {
                for (int x = 0; x < COLORS_X; x++)
                {
                    result += colors[y, x] == EMPTY ? " " : colors[y, x].ToString();
                }
                result += Environment.NewLine;
            }

            return result;
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
            string result = "";
            for (int y = 0; y < PIECE_Y; y++)
            {
                for (int x = 0; x < PIECE_X; x++)
                {
                    result += pieces[y, x].ToString();
                }
                result += Environment.NewLine;
            }
            return result;
        }

        public override bool Equals(object obj)
        {
            Cube2 other = (Cube2)obj;            
            return ArrayEqualsUsingMask(colors, other.colors);
        }

        public override int GetHashCode()
        {
            return colors.GetHashCode();
        }

        public bool EqualsPiecesOnly(Cube2 other)
        {            
            return ArrayEqualsUsingMask(pieces, other.pieces);
        }

        public bool EqualUsingMaskPiecesOnly(Cube2 other, int[,] mask)
        {            
            return ArrayEqualsUsingMask(pieces, other.pieces, mask);
        }

        public bool EqualUsingMask(Cube2 other, int[,] mask)
        {
            return ArrayEqualsUsingMask(colors, other.colors, mask);
        }

        private bool ArrayEqualsUsingMask(int [,] a, int[,] b, int[,]mask = null)
        {
            if (mask != null && (mask.GetLength(0) != a.GetLength(0) || mask.GetLength(1) != a.GetLength(1)))
                throw new Exception("mask has incorrect dimensions");

            for (int x = 0; x < a.GetLength(1); x++)
            {
                for (int y = 0; y < a.GetLength(0); y++)
                {
                    if (mask != null && mask[y, x] == 0)
                        continue;
                    else if (a[y, x] != b[y, x])
                        return false;                    
                }
            }
            return true;
        }

        private struct XY
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

        private struct RotationDescriptor
        {
            public readonly string Name;
            public readonly XY[][] MovesList;
            public readonly XY[] CubeMoves;

            public RotationDescriptor(string name, XY[][] movesList, XY[] cubeMoves)
            {
                this.Name = name;
                this.MovesList = movesList;
                this.CubeMoves = cubeMoves;
            }

            public override string ToString()
            {
                return Name;
            }
        }
    }
}
