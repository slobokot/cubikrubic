using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using cubikrubic;

namespace cubikrubicTests
{
    [TestClass]
    public class Cube2Test
    {
        [TestMethod]
        public void SequenceOfRotationLeadsToSolvedCube()
        {
            var cube = new Cube2();
            for(int i = 0; i < 15; i++)
            {
                cube.Rotate("a1");
                cube.Rotate("c1");
            }
            Assert.AreEqual(cube, new Cube2());
        }

        [TestMethod]
        public void Rotate_A1F_B1F()
        {
            var expectedCube = new Cube2(
                "  31  "+
                "  32  "+
                "554314"+
                "225314"+
                "  64  "+
                "  65  "+
                "  16  "+
                "  26  ");

            var cube = new Cube2();
            cube.Rotate("a1");
            cube.Rotate("b1");
            
            Assert.AreEqual(expectedCube, cube);
        }

        [TestMethod]
        public void Rotate_A1F()
        {
            var expectedCube = new Cube2(
                "  11  " +
                "  22  " +
                "253314" +
                "253314" +
                "  44  " +
                "  55  " +
                "  66  " +
                "  66  ");

            var cube = new Cube2();
            cube.Rotate("a1");            

            Assert.AreEqual(expectedCube, cube);
        }

        [TestMethod]
        public void Rotate_B1F()
        {
            var expectedCube = new Cube2(
                "  31  " +
                "  31  " +
                "225344" +
                "225344" +
                "  65  " +
                "  65  " +
                "  16  " +
                "  16  ");

            var cube = new Cube2();
            cube.Rotate("b1");

            Assert.AreEqual(expectedCube, cube);

            Assert.AreEqual(
                "21" + Environment.NewLine +
                "43" + Environment.NewLine +
                "65" + Environment.NewLine +
                "07" + Environment.NewLine, 
                cube.ToStringPieces());
        }
    }
}
