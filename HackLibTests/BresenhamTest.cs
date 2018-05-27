using System;
using System.Linq;
using HackConsole;
using HackConsole.Algo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HackLibTests
{
    [TestClass]
    public class BresenhamTest
    {
        [TestMethod]
        public void Brensen_HorVert()
        {
            RunLine(new Vec(3, 3), new Vec(3, 6), 3);
            RunLine(new Vec(3, 3), new Vec(6, 3), 3);
            RunLine(new Vec(3, 3), new Vec(3, 0), 3);
            RunLine(new Vec(3, 3), new Vec(0, 3), 3);
        }

        [TestMethod]
        public void Brensen_Diagonal()
        {
            RunLine(new Vec(3, 3), new Vec(6, 6), 3);
            RunLine(new Vec(3, 3), new Vec(6, 0), 3);
            RunLine(new Vec(3, 3), new Vec(0, 0), 3);
            RunLine(new Vec(3, 3), new Vec(0, 6), 3);
        }

        [TestMethod]
        public void Brensen_Specific()
        {
            var start = new Vec(3, 3);
            var end = new Vec(4, 6);

            var path = Bresenham.Run(start, end).ToArray();
            Assert.AreEqual(path.Length, 4);
            Assert.AreEqual(path[0], new Vec(3, 3));
            Assert.AreEqual(path[1], new Vec(3, 4));
            Assert.AreEqual(path[2], new Vec(4, 5));
            Assert.AreEqual(path[3], new Vec(4, 6));
        }

        void RunLine(Vec start, Vec end, int steps)
        {
            var path = Bresenham.Run(start, end).ToArray();
            Assert.AreEqual(path.First(), start);
            Assert.AreEqual(path.Last(), end);
            Assert.AreEqual(steps, path.Length - 1);

            for (int i = 0; i < steps; ++i)
            {
                var d = path[i] - path[i + 1];
                Assert.AreEqual(1, Math.Max(Math.Abs(d.X), Math.Abs(d.Y)));
            }
        }
    }
}
