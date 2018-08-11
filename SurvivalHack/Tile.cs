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
            var types = new List<Tile>
            {
                new Tile
                {
                    Tag = "floor_stone",
                    Symbol = new Symbol('.', new Colour(64, 64, 64), new Colour(16, 16, 16))
                },

                new Tile
                {
                    Tag = "floor_wood",
                    Natural = false,
                    Flamable = true,
                    Symbol = new Symbol('=', Colour.Parse("#471802"), Colour.Parse("#260c00"))
                },

                new Tile
                {
                    Tag = "short_grass",
                    Flamable = true,
                    Symbol = new Symbol('\'', Colour.Parse("#004c00"), Colour.Parse("#002300"))
                },

                new Tile
                {
                    Tag = "tall_grass",
                    Flamable = true,
                    BlockSight = true,
                    Symbol = new Symbol('"', Colour.Parse("#05a300"), Colour.Parse("#002300"))
                },

                new Tile
                {
                    Tag = "water",
                    WalkDanger = 0.5f,
                    MineCost = 15f,
                    Symbol = new Symbol('~', Colour.Parse("#0fa2db"), Colour.Parse("#0475a0"))
                },

                new Tile
                {
                    Tag = "water_deep",
                    WalkDanger = 0.5f,
                    MineCost = 15f,
                    Symbol = new Symbol('~', Colour.Parse("#0f12db"), Colour.Parse("#0409a0"))
                },

                new Tile
                {
                    Tag = "lava",
                    WalkDanger = 10,
                    MineCost = 15f,
                    Symbol = new Symbol('~', Colour.Parse("#ffdf3f"), Colour.Parse("#d66422"))
                },

                new Tile
                {
                    Tag = "rock",
                    Solid = true,
                    BlockSight = true,
                    Symbol = new Symbol('#', Colour.Black, Colour.Gray),
                    MineCost = 2,
                },

                new Tile
                {
                    Tag = "wall_stone",
                    Solid = true,
                    BlockSight = true,
                    Symbol = new Symbol('#', Colour.Black, new Colour(164, 87, 40)),
                    MineCost = 10
                }
            };

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
