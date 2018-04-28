using System;
using System.Collections.Generic;
using System.Diagnostics;
using HackConsole;

namespace SurvivalHack
{
    public class Grid<T>
    {
        private readonly T[,] _grid;
        public readonly Vec Size;

        public Grid(Vec size)
        {
            Size = size;
            _grid = new T[size.X, size.Y];
        }

        public T this[Vec v] {
            get => _grid[v.X, v.Y];
            set => _grid[v.X, v.Y] = value;
        }

        public IEnumerable<Vec> Iterator()
        {
            for (var y = 0; y < Size.Y; ++y)
                for (var x = 0; x < Size.X; ++x)
                    yield return new Vec(x, y);
        }

        /*
        private void Generate()
        {
            var heightMap = new PerlinNoise
            {
                Octaves = 4,
                Persistence = 0.5f,
                Scale = 6f,
                Seed = Dicebag.UniformInt()
            };

            foreach (var v in Size.Ids())
            {
                var height = heightMap.Get(v.X, v.Y);

                if (height > 0.4)
                {
                    _grid[v.X, v.Y] = Dicebag.UniformInt(75) == 1 ? TileList.Get("ore") : TileList.Get("rock");
                }
                else if (height > -0.4)
                {
                    _grid[v.X, v.Y] = TileList.Get("grass");
                    if (Dicebag.UniformInt(10) == 1)
                        _grid[v.X, v.Y] = TileList.Get("tree");
                    else if (Dicebag.UniformInt(2500) == 1)
                        _grid[v.X, v.Y] = TileList.Get("pumpkin");
                    else if (Dicebag.UniformInt(500) == 1)
                        _grid[v.X, v.Y] = TileList.Get("mushroom");
                }
                else
                {
                    _grid[v.X, v.Y] = TileList.Get("water");
                }
            }
        }

        public bool HasFlag(Vec v, TerrainFlag testFlag)
        {
            var flags = _grid[v.X, v.Y].Flags;

            return (testFlag & flags) != 0;
        }


        public int GetMask(Vec v)
        {
            return _roomGrid[v.X, v.Y];
        }

        internal void Set(Vec v, Tile tile, int roomId)
        {
            _grid[v.X, v.Y] = tile;
            _roomGrid[v.X, v.Y] = roomId;
        }

        public const int MASKID_VOID = -3;
        public const int MASKID_NOFLOOR = -2;
        public const int MASKID_KEEP = -1;
        */
    }
}
