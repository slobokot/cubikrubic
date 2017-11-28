using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using cubikrubic;

namespace cubikrubicTests
{
    [TestClass]
    public class Cube3Test
    {
        public Cube3 NewTestCube()
        {
            
           return new Cube3(
              "   363   " +
              "   123   " +
              "   624   " +
              "635242526" +
              "145433464" +
              "111664323" +
              "   531   " +
              "   615   " +
              "   522   " +
              "   464   " +
              "   551   " +
              "   152   "
          );            
        }

        [TestMethod]
        public void Rotate_A1()
        {
            var expectedCube = new Cube3(
                "   363   " +
                "   123   " +
                "   155   " +
                "635642626" +
                "143634264" +
                "111432423" +
                "   345   " +
                "   615   " +
                "   522   " +
                "   464   " +
                "   551   " +
                "   152   "
            );

            var cube = NewTestCube();
            cube.Rotate("a1");

            Assert.AreEqual(expectedCube, cube);
        }

        [TestMethod]
        public void Rotate_A2()
        {
            var expectedCube = new Cube3(
                "   363   " +
                "   143   " +
                "   624   " +
                "665242516" +
                "115433424" +
                "151664333" +
                "   531   " +
                "   262   " +
                "   522   " +
                "   464   " +
                "   551   " +
                "   152   "
            );

            var cube = NewTestCube();
            cube.Rotate("a2");

            Assert.AreEqual(expectedCube, cube);
        }

        [TestMethod]
        public void Rotate_B1()
        {
            var expectedCube = new Cube3(
              "   263   " +
              "   423   " +
              "   624   " +
              "551542526" +
              "341633464" +
              "611564323" +
              "   431   " +
              "   515   " +
              "   122   " +
              "   364   " +
              "   151   " +
              "   652   ");

            var cube = NewTestCube();
            cube.Rotate("b1");

            Assert.AreEqual(expectedCube, cube);
        }

        [TestMethod]
        public void Rotate_B2()
        {
            var expectedCube = new Cube3(
              "   343   " +
              "   133   " +
              "   664   " +
              "635232526" +
              "145413464" +
              "111624323" +
              "   561   " +
              "   655   " +
              "   552   " +
              "   464   " +
              "   521   " +
              "   122   ");

            var cube = NewTestCube();
            cube.Rotate("b2");

            Assert.AreEqual(expectedCube, cube);
        }

        [TestMethod]
        public void Rotate_C1()
        {
            var expectedCube = new Cube3(
              "   363   " +
              "   123   " +
              "   624   " +
              "635242526" +
              "145433464" +
              "464111664" +
              "   565   " +
              "   213   " +
              "   251   " +
              "   323   " +
              "   551   " +
              "   152   ");

            var cube = NewTestCube();
            cube.Rotate("c1");

            Assert.AreEqual(expectedCube, cube);
        }

        [TestMethod]
        public void Rotate_C1_Count2()
        {
            var expectedCube = new Cube3(
              "   363   " +
              "   123   " +
              "   624   " +
              "635242526" +
              "145433464" +
              "323464111" +
              "   225   " +
              "   516   " +
              "   135   " +
              "   466   " +
              "   551   " +
              "   152   ");

            var cube = NewTestCube();
            cube.Rotate("c1", 2);

            Assert.AreEqual(expectedCube, cube);
        }

        [TestMethod]
        public void Rotate_C2()
        {
            var expectedCube = new Cube3(
              "   363   " +
              "   123   " +
              "   624   " +
              "635242526" +
              "155145433" +
              "111664323" +
              "   531   " +
              "   615   " +
              "   522   " +
              "   464   " +
              "   464   " +
              "   152   ");

            var cube = NewTestCube();
            cube.Rotate("c2");

            Assert.AreEqual(expectedCube, cube);
        }
    }
}
