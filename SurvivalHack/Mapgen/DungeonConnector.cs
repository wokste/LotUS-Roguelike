using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HackConsole;
using HackConsole.Algo;

namespace SurvivalHack.Mapgen
{
    public class DungeonConnector
    {
        Level _map;
        readonly double[,] Weights; // Adjacency matrix. 0 means the rooms are connected.
        Room[] _rooms;

        int RoomCount => _rooms.Length;

        readonly RoomStatFlag[] Flags;

        public DungeonConnector(Level map, List<Room> rooms)
        {
            _map = map;
            _rooms = rooms.ToArray();

            Weights = new double[RoomCount, RoomCount];

            for (int i = 0; i < RoomCount; i++)
                for (int j = 0; j < RoomCount; j++)
                    Weights[i, j] = Weight(i, j);

            Flags = new RoomStatFlag[RoomCount];
        }

        // Inspired by https://en.wikipedia.org/wiki/Bowyer%E2%80%93Watson_algorithm
        List<Triangle> BowyerWatson(List<Vec> pointList)
        {
            // pointList is a set of coordinates defining the points to be triangulated
            var triangulation = new List<Triangle>();

            var numPoints = pointList.Count;

            // Add super-triangle
            pointList.Add(new Vec(-1, -1));
            pointList.Add(new Vec(-1, _map.Size.Y * 2 + 10));
            pointList.Add(new Vec(_map.Size.X * 2 + 10, -1));
            triangulation.Add(new Triangle(pointList, numPoints + 0, numPoints + 1, numPoints + 2));

            // Main algorithm. Add one point each time.
            for (int i = 0; i < numPoints; i++)
            {
                var badTriangles = new List<Triangle>();
                foreach (var triangle in triangulation) // first find all the triangles that are no longer valid due to the insertion
                {
                    if (triangle.InCircumCricle(pointList, i))
                    {
                        badTriangles.Add(triangle);
                    }
                }

                // find the boundary of the polygonal hole
                var polygon = new List<Edge>();
                var edges = badTriangles.SelectMany(t => t.Edges()).OrderBy(e => e.ID).ToList();

                for (int j = 0; j < edges.Count; j++)
                {
                    if (j != 0 && edges[j].ID == edges[j - 1].ID)
                        continue;

                    if (j != edges.Count + 1 && edges[j].ID == edges[j + 1].ID)
                        continue;

                    polygon.Add(edges[j]);
                }

                // remove polygonial hole
                triangulation = triangulation.Except(badTriangles).ToList();

                // re-triangulate the polygonal hole
                foreach (var edge in polygon)
                {
                    triangulation.Add(new Triangle(pointList, edge.P0, edge.P1, i));
                }
            }

            // Remove super-triangles again.
            var badIds = new[] { numPoints, numPoints + 1, numPoints + 2};
            triangulation = triangulation.Where(t => !t.ContainsID(badIds)).ToList();

            return triangulation;
        }

        public struct Triangle
        {
            public int P0;
            public int P1;
            public int P2;

            public Triangle(List<Vec> pointList, int p0, int p1, int p2)
            {
                P0 = p0;
                P1 = p1;
                P2 = p2;

                MakeCounterClockwise(pointList);
            }

            void MakeCounterClockwise(List<Vec> pointList) {
                var v1 = pointList[P1] - pointList[P0];
                var v2 = pointList[P2] - pointList[P0];

                var ccw = v1.X * v2.Y - v2.X * v1.Y > 0;

                if (!ccw)
                {
                    (P0, P1) = (P1, P0);
                }
            }

            public IEnumerable<Edge> Edges()
            {
                yield return new Edge(P0, P1);
                yield return new Edge(P0, P2);
                yield return new Edge(P1, P2);
            }

            public bool ContainsID(int i)
            {
                return P0 == i || P1 == i || P2 == i;
            }

            public bool ContainsID(IEnumerable<int> l)
            {
                foreach (var i in l)
			        if (ContainsID(i))
                        return true;

                return false;
            }

            // Inspiration:
            // - https://en.wikipedia.org/wiki/Delaunay_triangulation
            // - https://stackoverflow.com/questions/39984709/how-can-i-check-wether-a-point-is-inside-the-circumcircle-of-3-points
            public bool InCircumCricle(List<Vec> pointList, int i)
            {
                var v0 = pointList[P0] - pointList[i];
                var v1 = pointList[P1] - pointList[i];
                var v2 = pointList[P2] - pointList[i];

                return (
                    (v0.X * v0.X + v0.Y * v0.Y) * (v1.X * v2.Y - v2.X * v1.Y) -
                    (v1.X * v1.X + v1.Y * v1.Y) * (v0.X * v2.Y - v2.X * v0.Y) +
                    (v2.X * v2.X + v2.Y * v2.Y) * (v0.X * v1.Y - v1.X * v0.Y)
                ) > 0;
            }
        }

        public struct Edge
        {
            public int P0;
            public int P1;

            public int ID => P0 << 16 + P1;

            public Edge(int p0, int p1)
            {
                if (p0 < p1)
                {
                    P0 = p0;
                    P1 = p1;
                }
                else
                {
                    P0 = p1;
                    P1 = p0;
                }
            }

            bool ContainsID(int i)
            {
                return P0 == i || P1 == i;
            }
        }

        public void Prim()
        {
            // Connect the first room to the MST
            Flags[0] |= RoomStatFlag.Connected;

            for (int edgeCount = 0; edgeCount < RoomCount - 1; edgeCount++)
            {
                double min = double.MaxValue;
                int x = 0;
                int y = 0;

                for (int i = 0; i < RoomCount; i++)
                {
                    if (Flags[i].HasFlag(RoomStatFlag.Connected))
                    {
                        for (int j = 0; j < RoomCount; j++)
                        {
                            if (!Flags[j].HasFlag(RoomStatFlag.Connected) && Weights[i,j] != 0)
                            {
                                if (min > Weights[i,j])
                                {
                                    min = Weights[i,j];
                                    x = i;
                                    y = j;
                                }
                            }
                        }
                    }
                }
                Connect(x, y);
                Flags[y] |= RoomStatFlag.Connected;

                // Already for the eliminateDeadEnds algorithm
                if (edgeCount != 0)
                    Flags[x] |= RoomStatFlag.Path;
            }
        }

        public void EliminateDeadEnds(Random rnd, double p)
        {
            for (int i = 0; i < RoomCount; i++)
            {
                if (!Flags[i].HasFlag(RoomStatFlag.Path))
                {
                    if (rnd.NextDouble() > p)
                        continue;

                    double min = double.MaxValue;
                    int y = -1;

                    for (int j = 0; j < RoomCount; j++)
                    {
                        if (Weights[i, j] != 0)
                        {
                            if (min > Weights[i, j])
                            {
                                min = Weights[i, j];
                                y = j;
                            }
                        }
                    }

                    Connect(i, y);
                    Flags[i] |= RoomStatFlag.Path;
                }
            }
        }

        double Weight(int i, int j)
        {
            var c1 = _rooms[i].Center;
            var c2 = _rooms[j].Center;
            return (c1 - c2).Length;
        }

        void Connect(int i, int j) {

            var c1 = _rooms[i].Center;
            var c2 = _rooms[j].Center;

            var pn = new PerlinNoise(1337);
            var aStar = new AStar(_map.TileMap.Size, (Vec v) => _map.GetTile(v).HP, false);
            var path = aStar.Run(c1, c2);

            var floor = _map.TileDefs.Get("tall_grass");

            foreach (var v in path)
            {
                var tile = _map.GetTile(v);

                if (!tile.Flags.HasFlag(TerrainFlag.Walk))
                    _map.TileMap[v] = floor;
            }
            
            Weights[i, j] = 0;
            Weights[j, i] = 0;
        }
    }

    [Flags]
    enum RoomStatFlag {
        None = 0,
        Connected = 1,
        Path = 2,
    }
}
