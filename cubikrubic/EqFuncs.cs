using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cubikrubic
{
    class EqFuncs
    {
        /*
int[,] referenceMask = new int[12, 9]
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

        public static Func<Cube3, Cube3, bool> AlwaysTrue()
        {
            return (cube, refCube) => true;
        }

        public static Func<Cube3, Cube3, bool> SwapCenterNeighborsEqFunc_Has10MovesSolution()
        {
            var piecesNotEqualsMask = Cube3.PiecesMaskToPoints(new int[12, 9]
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
            });

            var piecesEqualsMask = Cube3.PiecesMaskToPoints(new int[12, 9]
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
            });

            var colorsMaskCenterNeighbor = Cube3.ColorsMaskToPoints(new int[12, 9]
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
            });

            return (cube, refCube) =>
            {
                return
                !cube.EqualsPieces(refCube, piecesNotEqualsMask) &&
                cube.EqualsPieces(refCube, piecesEqualsMask) &&
                cube.EqualsColors(refCube, colorsMaskCenterNeighbor);
            };
        }

        public static Func<Cube3, bool> RotateColorsOfCenterNeighborsEqFunc_UnknownStatus(Cube3 refCube)
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

        public static Func<Cube3, bool> SwapEdgeNeighborsEqFunc_HalfHourNoResult(Cube3 refCube)
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
                (cube.Pieces[5, 3] != refCube.Pieces[5, 3] || cube.Pieces[5, 5] != refCube.Pieces[5, 5]) &&
                cube.EqualsColors(refCube, colorsEqualsMask);
            };
        }

        public static Func<Cube3, Cube3,bool> SwapEdgeNeighborsEqFunc()
        {
            var piecesEqualPoints = Cube3.PiecesMaskToPoints(new int[12, 9]
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
            });

            var piecesNotEqualPoints = Cube3.PiecesMaskToPoints(new int[12, 9]
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
            });

            var colorsEqualsMask = Cube3.ColorsMaskToPoints(new int[12, 9]
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
            }); 

            return (cube, refCube) =>
            {
                return
                cube.EqualsPieces(refCube, piecesEqualPoints) &&
                !cube.EqualsPieces(refCube, piecesNotEqualPoints);
            };
        }
    }
}
