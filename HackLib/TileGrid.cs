using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace HackLib
{
    public class TileGrid
    {
        public int[,] Grid;
        public readonly int Width;
        public readonly int Height;

        public TileGrid(int width, int height)
        {
            Grid = new int[width,height];
            Width = width;
            Height = height;
            Generate();
        }

        private void Generate()
        {
            PerlinNoise heightMap = new PerlinNoise
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
                    var isRock = heightMap.Get(x, y) > 0.3;
                    
                    if (isRock)
                        Grid[x, y] = Dicebag.UniformInt(30) == 1 ? TileTypeList.GetID("ore") : TileTypeList.GetID("rock");
                    else
                        Grid[x, y] = Dicebag.UniformInt(5) == 1 ? TileTypeList.GetID("tree") : TileTypeList.GetID("grass");
                }
            }
        }

        public void Render(Rectangle viewport, Point translation)
        {
            for (var y = 0; y < viewport.Height; y++)
            {
                var tY = y + translation.Y;
                if (tY < 0 || tY >= Height)
                    continue;
                
                Console.SetCursorPosition(viewport.Left, viewport.Top + y);
                
                for (var x = 0; x < viewport.Width; x++)
                {
                    var tX = x + translation.X;

                    if (tX < 0 || tX >= Width)
                    {
                        Console.Write(' ');
                        continue;
                    }
                    
                    var tile = TileTypeList.Get(Grid[x + translation.X, y + translation.Y]);

                    Console.ForegroundColor = tile.Color;
                    Console.Write(tile.Char);
                }
            }
        }
    }

    public class TileType
    {
        public string Tag;
        public char Char;
        public ConsoleColor Color;

        public string DropTag = "";
        public int DropCount = 0;
        public bool Walkable = false;
    }

    public static class TileTypeList
    {
        private static readonly List<TileType> _types = new List<TileType>();

        public static void InitTypes()
        {
            Debug.Assert(_types.Count == 0);

            _types.Add(new TileType
            {
                Tag = "grass",
                Char = '.',
                Color = ConsoleColor.Gray,
                DropTag = "",
                DropCount = 0,
                Walkable = true,
            });

            _types.Add(new TileType
            {
                Tag = "tree",
                Char = '^',
                Color = ConsoleColor.Green,
                DropTag = "wood",
                DropCount = 5
            });

            _types.Add(new TileType
            {
                Tag = "rock",
                Char = '#',
                Color = ConsoleColor.Red,
                DropTag = "stone",
                DropCount = 1
            });

            _types.Add(new TileType
            {
                Tag = "ore",
                Char = '#',
                Color = ConsoleColor.Yellow,
                DropTag = "ore",
                DropCount = 1
            });
        }
        
        public static int GetID(string tag)
        {
            var tileID = _types.FindIndex(t => t.Tag == tag);
            Debug.Assert(tileID != -1);
            return tileID;
        }

        public static TileType Get(int id)
        {
            return _types[id];
        }
    }
}
