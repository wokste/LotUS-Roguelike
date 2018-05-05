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
        All = ~0,
    }

    public class Tile
    {
        public string Tag;

        public string DropTag = "";
        public int DropCount = 0;

        public TerrainFlag Flags;

        public Symbol Symbol;

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
                Tag = "floor",
                DropTag = "",
                DropCount = 0,
                Flags = ~TerrainFlag.Swim,
                Symbol = new Symbol('.', new Color(235, 231, 203))
            });

            Types.Add(new Tile
            {
                Tag = "water",
                DropTag = "",
                DropCount = 0,
                Flags = ~TerrainFlag.Walk,
                Symbol = new Symbol('~', new Color(136, 205, 247))
            });

            Types.Add(new Tile
            {
                Tag = "door",
                DropTag = "",
                DropCount = 0,
                Flags = ~TerrainFlag.Swim,
                Symbol = new Symbol('+', Color.Green)
            });

            Types.Add(new Tile
            {
                Tag = "rock",
                DropTag = "stone",
                DropCount = 3,
                Flags = TerrainFlag.None,
                Symbol = new Symbol('#', Color.Gray)
            });

            Types.Add(new Tile
            {
                Tag = "wall",
                DropTag = "stone",
                DropCount = 3,
                Flags = TerrainFlag.None,
                Symbol = new Symbol('#', new Color(164, 87, 40))
            });

            Types.Add(new Tile
            {
                Tag = "void",
                DropTag = "",
                DropCount = 0,
                Flags = TerrainFlag.None,
                Symbol = new Symbol(' ', Color.Black)
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
