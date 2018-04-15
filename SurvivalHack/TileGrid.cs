using System;
using System.Collections.Generic;
using System.Diagnostics;
using HackConsole;

namespace SurvivalHack
{
    public class TileGrid
    {
        private readonly Tile[,] _tileGrid;
        private readonly int[,] _roomGrid;
        public readonly Vec Size;

        public TileGrid(Vec size)
        {
            Size = size;
            _tileGrid = new Tile[size.X, size.Y];
            _roomGrid = new int[size.X, size.Y];
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

            foreach (var v in Size.Iterator())
            {
                var height = heightMap.Get(v.X, v.Y);

                if (height > 0.4)
                {
                    _tileGrid[v.X, v.Y] = Dicebag.UniformInt(75) == 1 ? TileList.Get("ore") : TileList.Get("rock");
                }
                else if (height > -0.4)
                {
                    _tileGrid[v.X, v.Y] = TileList.Get("grass");
                    if (Dicebag.UniformInt(10) == 1)
                        _tileGrid[v.X, v.Y] = TileList.Get("tree");
                    else if (Dicebag.UniformInt(2500) == 1)
                        _tileGrid[v.X, v.Y] = TileList.Get("pumpkin");
                    else if (Dicebag.UniformInt(500) == 1)
                        _tileGrid[v.X, v.Y] = TileList.Get("mushroom");
                }
                else
                {
                    _tileGrid[v.X, v.Y] = TileList.Get("water");
                }
            }
        }

        public bool HasFlag(Vec v, TerrainFlag testFlag)
        {
            var flags = _tileGrid[v.X, v.Y].Flags;

            return (testFlag & flags) != 0;
        }

        public Tile GetTile(Vec v)
        {
            return _tileGrid[v.X, v.Y];
        }

        public int GetMask(Vec v)
        {
            return _roomGrid[v.X, v.Y];
        }

        internal void Set(Vec v, Tile tile, int roomId)
        {
            _tileGrid[v.X, v.Y] = tile;
            _roomGrid[v.X, v.Y] = roomId;
        }

        public const int MASKID_VOID = -3;
        public const int MASKID_NOFLOOR = -2;
        public const int MASKID_KEEP = -1;
    }

    [Flags]
    public enum TerrainFlag
    {
        Walk = 1,
        Fly = 2,
        Swim = 4,
        Sight = 8,

        None = 0,
        All = 0x7fffffff,
    }

    public class Tile
    {
        public string Tag;

        public string DropTag = "";
        public int DropCount = 0;

        public TerrainFlag Flags;

        public Symbol Char;

        public override string ToString()
        {
            return Tag;
        }
    }

    public static class TileList
    {
        private static readonly List<Tile> Types = new List<Tile>();

        public static void InitTypes()
        {
            Debug.Assert(Types.Count == 0);

            Types.Add(new Tile
            {
                Tag = "grass",
                DropTag = "",
                DropCount = 0,
                Flags = ~TerrainFlag.Swim,
                Char = new Symbol('.', Color.Green)
            });

            Types.Add(new Tile
            {
                Tag = "water",
                DropTag = "",
                DropCount = 0,
                Flags = ~TerrainFlag.Walk,
                Char = new Symbol('~', Color.Cyan)
            });

            Types.Add(new Tile
            {
                Tag = "gravel",
                DropTag = "",
                DropCount = 0,
                Flags = ~TerrainFlag.Swim,
                Char = new Symbol(',', Color.Gray)
            });

            Types.Add(new Tile
            {
                Tag = "tree",
                DropTag = "wood",
                DropCount = 3,
                Flags = TerrainFlag.Sight,
                Char = new Symbol((char)6, Color.Green)
            });

            Types.Add(new Tile
            {
                Tag = "rock",
                DropTag = "stone",
                DropCount = 3,
                Flags = TerrainFlag.None,
                Char = new Symbol('#', Color.Gray)
            });

            Types.Add(new Tile
            {
                Tag = "stone",
                DropTag = "stone",
                DropCount = 1,
                Flags = TerrainFlag.Fly,
                Char = new Symbol((char)30, Color.Gray)
            });

            Types.Add(new Tile
            {
                Tag = "ore",
                DropTag = "ore",
                DropCount = 3,
                Flags = TerrainFlag.None,
                Char = new Symbol('~', Color.Red)
            });

            Types.Add(new Tile
            {
                Tag = "pumpkin",
                DropTag = "pumpkin",
                DropCount = 1,
                Flags = TerrainFlag.Fly | TerrainFlag.Sight,
                Char = new Symbol('p', Color.Orange)
            });

            Types.Add(new Tile
            {
                Tag = "mushroom",
                DropTag = "mushroom",
                DropCount = 1,
                Flags = TerrainFlag.Fly | TerrainFlag.Sight,
                Char = new Symbol('m', Color.Pink)
            });
        }

        public static Tile Get(string tag)
        {
            var tileId = Types.FindIndex(t => t.Tag == tag);
            Debug.Assert(tileId != -1);
            return Types[tileId];
        }
    }
}
