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

        public Symbol Symbol;
        public float MineCost = 1;
        public bool Solid = false;
        public bool BlockSight = false;
        public float WalkDanger = 0;
        public bool Flamable = false;
        public bool Natural = true;

        public bool IsFloor => (!Solid && WalkDanger <= 0);

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
                Symbol = new Symbol('.', new Color(64, 64, 64), new Color(16, 16, 16))
            });

            types.Add(new Tile
            {
                Tag = "floor_wood",
                Natural = false,
                Flamable = true,
                Symbol = new Symbol('=', Color.Parse("#471802"), Color.Parse("#260c00"))
            });

            types.Add(new Tile
            {
                Tag = "short_grass",
                Flamable = true,
                Symbol = new Symbol('\'', Color.Parse("#004c00"), Color.Parse("#002300"))
            });

            types.Add(new Tile
            {
                Tag = "tall_grass",
                Flamable = true,
                BlockSight = true,
                Symbol = new Symbol('"', Color.Parse("#05a300"), Color.Parse("#002300"))
            });

            types.Add(new Tile
            {
                Tag = "water",
                WalkDanger = 0.5f,
                MineCost = 1.5f,
                Symbol = new Symbol('~', Color.Parse("#0fa2db"), Color.Parse("#0475a0"))
            });

            types.Add(new Tile
            {
                Tag = "lava",
                WalkDanger = 10,
                MineCost = 1.5f,
                Symbol = new Symbol('~', Color.Parse("#ffdf3f"), Color.Parse("#d66422"))
            });

            types.Add(new Tile
            {
                Tag = "rock",
                Solid = true,
                BlockSight = true,
                Symbol = new Symbol('#', Color.Black, Color.Gray),
                MineCost = 2,
            });

            types.Add(new Tile
            {
                Tag = "wall_stone",
                Solid = true,
                BlockSight = true,
                Symbol = new Symbol('#', Color.Black, new Color(164, 87, 40)),
                MineCost = 10
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
