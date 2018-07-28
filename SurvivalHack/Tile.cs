using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public TerrainFlag Flags;

        public Symbol Symbol;
        public float HP = 1;
        public bool isWall = false;
        public bool Walkable = false;

        public override string ToString()
        {
            return Tag;
        }

        public static List<Tile> InitTypes()
        {
            var types = new List<Tile>();

            types.Add(new Tile
            {
                Tag = "floor_stone",
                Flags = ~TerrainFlag.Swim,
                Symbol = new Symbol('.', new Color(64, 64, 64), new Color(16, 16, 16))
            });

            types.Add(new Tile
            {
                Tag = "floor_wood",
                Flags = ~TerrainFlag.Swim,
                Symbol = new Symbol('=', Color.Parse("#471802"), Color.Parse("#260c00"))
            });

            types.Add(new Tile
            {
                Tag = "short_grass",
                Flags = ~TerrainFlag.Swim,
                Symbol = new Symbol('\'', Color.Parse("#004c00"), Color.Parse("#002300"))
            });

            types.Add(new Tile
            {
                Tag = "tall_grass",
                Flags = ~TerrainFlag.Swim,
                Symbol = new Symbol('"', Color.Parse("#05a300"), Color.Parse("#002300"))
            });

            types.Add(new Tile
            {
                Tag = "water",
                Flags = ~TerrainFlag.Walk,
                Symbol = new Symbol('~', Color.Parse("#0fa2db"), Color.Parse("#0475a0"))
            });

            types.Add(new Tile
            {
                Tag = "lava",
                Flags = ~TerrainFlag.Walk,
                Symbol = new Symbol('~', Color.Parse("#ffdf3f"), Color.Parse("#d66422"))
            });

            types.Add(new Tile
            {
                Tag = "rock",
                Flags = TerrainFlag.None,
                Symbol = new Symbol('#', Color.Black, Color.Gray),
                HP = 2,
            });

            types.Add(new Tile
            {
                Tag = "wall_stone",
                Flags = TerrainFlag.None,
                Symbol = new Symbol('#', Color.Black, new Color(164, 87, 40)),
                HP = 10
            });

            return types;
        }
    }

    public static class TileGenerics
    {
        public static int Get(this List<Tile> tiles, string tag)
        {
            var tileId = tiles.FindIndex(t => t.Tag == tag);
            Debug.Assert(tileId != -1);
            return tileId;
        }
    }
}
