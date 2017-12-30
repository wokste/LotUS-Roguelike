using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace HackLib
{
    public class TileGrid
    {
        private readonly Tile[,] _grid;
        public readonly int Width;
        public readonly int Height;

        public TileGrid(int width, int height)
        {
            _grid = new Tile[width,height];
            Width = width;
            Height = height;
            Generate();
        }

        private void Generate()
        {
            var heightMap = new PerlinNoise
            {
                Octaves = 4,
                Persistence = 0.5f,
                Scale = 6f,
                Seed = Dicebag.UniformInt()
            };
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    var height = heightMap.Get(x, y);
                    
                    if (height > 0.3)
                    {
                        _grid[x, y].Floor = TileTypeList.Get("gravel");
                        _grid[x, y].Wall = Dicebag.UniformInt(75) == 1 ? TileTypeList.Get("ore") : TileTypeList.Get("rock");
                    }
                    else
                    {
                        _grid[x, y].Floor = TileTypeList.Get("grass");
                        if (Dicebag.UniformInt(10) == 1)
                            _grid[x, y].Wall = TileTypeList.Get("tree");
                        else if (Dicebag.UniformInt(2500) == 1)
                            _grid[x, y].Wall = TileTypeList.Get("pumpkin");
                        else if (Dicebag.UniformInt(500) == 1)
                            _grid[x, y].Wall = TileTypeList.Get("mushroom");

                    }
                }
            }
        }

        public bool HasFlag(int x, int y, TerrainFlag testFlag)
        {
            var flags = _grid[x, y].Floor.Flags;
            if (_grid[x, y].Wall != null)
                flags |= _grid[x, y].Wall.Flags;

            return (testFlag & flags) != 0;
        }

        public TileType GetFloor(int x, int y)
        {
            return _grid[x,y].Floor;
        }

        public TileType GetWall(int x, int y)
        {
            return _grid[x,y].Wall;
        }

        public void DestroyWall(int x, int y)
        {
            _grid[x, y].Wall = null;
        }
    }
    
    public struct Tile
    {
        public TileType Floor;
        public TileType Wall;

        public bool BlocksSights => (Wall != null && (Wall.Flags & TerrainFlag.BlockSight) != 0);
    }

    [Flags]
    public enum TerrainFlag
    {
        BlockWalk = 1,
        BlockFly = 2,
        BlockSwim = 4,
        BlockSight = 8,
    }

    public class TileType
    {
        public string Tag;

        public string DropTag = "";
        public int DropCount = 0;

        public TerrainFlag Flags;

        public Symbol Char;
    }

    public static class TileTypeList
    {
        private static readonly List<TileType> Types = new List<TileType>();

        public static void InitTypes()
        {
            Debug.Assert(Types.Count == 0);

            Types.Add(new TileType
            {
                Tag = "grass",
                DropTag = "",
                DropCount = 0,
                Flags = TerrainFlag.BlockSwim,
                Char = new Symbol('.', Color.Green)
            });

            Types.Add(new TileType
            {
                Tag = "gravel",
                DropTag = "",
                DropCount = 0,
                Flags = TerrainFlag.BlockSwim,
                Char = new Symbol(',', Color.Gray)
            });

            Types.Add(new TileType
            {
                Tag = "tree",
                DropTag = "wood",
                DropCount = 3,
                Flags = TerrainFlag.BlockSwim | TerrainFlag.BlockWalk | TerrainFlag.BlockFly,
                Char = new Symbol((char)6, Color.Green)
            });

            Types.Add(new TileType
            {
                Tag = "rock",
                DropTag = "stone",
                DropCount = 3,
                Flags = TerrainFlag.BlockSwim | TerrainFlag.BlockWalk | TerrainFlag.BlockFly | TerrainFlag.BlockSight,
                Char = new Symbol('#', Color.Gray)
            });

            Types.Add(new TileType
            {
                Tag = "stone",
                DropTag = "stone",
                DropCount = 1,
                Flags = TerrainFlag.BlockSwim | TerrainFlag.BlockWalk,
                Char = new Symbol((char)30, Color.Gray)
            });

            Types.Add(new TileType
            {
                Tag = "ore",
                DropTag = "ore",
                DropCount = 3,
                Flags = TerrainFlag.BlockSwim | TerrainFlag.BlockWalk | TerrainFlag.BlockFly | TerrainFlag.BlockSight,
                Char = new Symbol('~', Color.Red)
            });

            Types.Add(new TileType
            {
                Tag = "pumpkin",
                DropTag = "pumpkin",
                DropCount = 1,
                Flags = TerrainFlag.BlockSwim | TerrainFlag.BlockWalk,
                Char = new Symbol('p', Color.Orange)
            });

            Types.Add(new TileType
            {
                Tag = "mushroom",
                DropTag = "mushroom",
                DropCount = 1,
                Flags = TerrainFlag.BlockSwim | TerrainFlag.BlockWalk,
                Char = new Symbol('m', Color.Pink)
            });
        }

        public static TileType Get(string tag)
        {
            var tileId = Types.FindIndex(t => t.Tag == tag);
            Debug.Assert(tileId != -1);
            return Types[tileId];
        }
    }
}
