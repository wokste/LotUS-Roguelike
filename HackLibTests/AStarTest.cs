using HackConsole;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HackConsole.Algo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackLibTests
{
    [TestClass]
    public class AStarTest
    {
        [TestMethod]
        public void AStar_NoDiagonal()
        {
            var grid = MakeGrid();
            var aStar = new AStar<bool>(grid, (Vec v, bool b) => b ? 1 : float.PositiveInfinity, false);

            RunPath(aStar, new Vec(3, 0), new Vec(6, 7), 10);
            RunPath(aStar, new Vec(1, 4), new Vec(8, 4), 17);
            RunPath(aStar, new Vec(8, 4), new Vec(8, 1), null);
        }

        [TestMethod]
        public void AStar_Diagonally()
        {
            var grid = MakeGrid();
            var aStar = new AStar<bool>(grid, (Vec v, bool b) => b ? 1 : float.PositiveInfinity, true);

            RunPath(aStar, new Vec(3, 0), new Vec(6, 7), 7);
            RunPath(aStar, new Vec(1, 4), new Vec(8, 4), 10);
            RunPath(aStar, new Vec(8, 4), new Vec(8, 1), null);
        }


        [TestMethod]
        public void AStar_HigherCost()
        {
            var grid = MakeGrid();
            var aStar = new AStar<bool>(grid, (Vec v, bool b) => b ? 2.5f : 100, true);

            RunPath(aStar, new Vec(3, 0), new Vec(6, 7), 7);
            RunPath(aStar, new Vec(1, 4), new Vec(8, 4), 10);
            RunPath(aStar, new Vec(8, 4), new Vec(8, 1), 3);
        }

        Grid<bool> MakeGrid() {
            var str = new string[] {
                ".......#..",
                ".......#..",
                "..#....###",
                "..#....#..",
                "..#....#..",
                "..#....#..",
                "..#.......",
                "..#.......",
            };

            var grid = new Grid<bool>(new Vec(str[0].Length, str.Length));
            foreach (var v in grid.Ids())
            {
                grid[v] = str[v.Y][v.X] == '.';
            }
            return grid;
        }

        void RunPath(AStar<bool> aStar, Vec start, Vec end, int? steps)
        {
            var path = aStar.Run(start, end);
            if (steps == null)
            {
                Assert.IsNull(path);
            }
            else
            {
                Assert.IsTrue(path.First() == start);
                Assert.IsTrue(path.Last() == end);
                Assert.AreEqual(steps, path.Count - 1);

                for (int i = 0; i < steps; ++i)
                {
                    var d = path[i] - path[i + 1];
                    Assert.AreEqual(1, Math.Max(Math.Abs(d.X), Math.Abs(d.Y)));
                }
            }
        }
    }
}
