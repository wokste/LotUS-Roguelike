using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HackConsole;

namespace SurvivalHack
{
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
