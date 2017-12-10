using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace HackLib
{
    public class TileGrid
    {
        public Tile[,] Grid;
        public readonly int Width;
        public readonly int Height;

        public TileGrid(int width, int height)
        {
            Grid = new Tile[width,height];
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
                        Grid[x, y].Floor = TileTypeList.Get("gravel");
                        Grid[x, y].Wall = Dicebag.UniformInt(75) == 1 ? TileTypeList.Get("ore") : TileTypeList.Get("rock");
                    }
                    else
                    {
                        Grid[x, y].Floor = TileTypeList.Get("grass");
                        if (Dicebag.UniformInt(10) == 1)
                            Grid[x, y].Wall = TileTypeList.Get("tree");
                        else if (Dicebag.UniformInt(2500) == 1)
                            Grid[x, y].Wall = TileTypeList.Get("pumpkin");
                        else if (Dicebag.UniformInt(500) == 1)
                            Grid[x, y].Wall = TileTypeList.Get("mushroom");

                    }

                    Grid[x, y].Visibility = TileVisibility.Hidden;
                }
            }
        }
    }
    
    public struct Tile
    {
        public TileType Floor;
        public TileType Wall;
        public TileVisibility Visibility;

        public bool BlocksSights => (Wall != null);
    }

    public enum TileVisibility
    {
        Hidden, Dark, Visible
    }

    public class TileType
    {
        public string Tag;

        public string DropTag = "";
        public int DropCount = 0;

        public Point SourcePos;
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
                SourcePos = new Point(0,0),
            });

            Types.Add(new TileType
            {
                Tag = "gravel",
                DropTag = "",
                DropCount = 0,
                SourcePos = new Point(1, 0),
            });

            Types.Add(new TileType
            {
                Tag = "tree",
                DropTag = "wood",
                DropCount = 3,
                SourcePos = new Point(4, 0),
            });

            Types.Add(new TileType
            {
                Tag = "rock",
                DropTag = "stone",
                DropCount = 3,
                SourcePos = new Point(4, 1),
            });

            Types.Add(new TileType
            {
                Tag = "stone",
                DropTag = "stone",
                DropCount = 1,
                SourcePos = new Point(5, 0),
            });

            Types.Add(new TileType
            {
                Tag = "ore",
                DropTag = "ore",
                DropCount = 3,
                SourcePos = new Point(5, 1),
            });

            Types.Add(new TileType
            {
                Tag = "pumpkin",
                DropTag = "pumpkin",
                DropCount = 1,
                SourcePos = new Point(6, 0),
            });

            Types.Add(new TileType
            {
                Tag = "mushroom",
                DropTag = "mushroom",
                DropCount = 1,
                SourcePos = new Point(7, 0),
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
