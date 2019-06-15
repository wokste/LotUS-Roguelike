﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

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

    public struct TileGlyph
    {
        public byte X;
        public byte Y;
        public byte Method;

        public const byte BASE = 0;
        public const byte TERRAIN = 1;
        public const byte PIT = 2;
        public const byte ANIM = 3;
        public const byte WALL = 4;

        public static readonly string[] MethodTypes = new[] { "", "Terrain", "pit" };

        public TileGlyph(byte x, byte y, byte method = 0)
        {
            X = x; Y = y; Method = method;
        }

        public override string ToString() => Method == 0 ? $"{X},{Y}" : $"{(GlyphMethod)Method}: {X},{Y}";

        public TileGlyph(string s) : this()
        {
            // TODO: XML
            X = 0;
            Y = 0;
            Method = 0;
        }

        public enum GlyphMethod {
            Base, Terrain, Pit, Anim, Wall
        }
    }


    public class TileGlyphTypeConverter : ExpandableObjectConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(TileGlyph);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string s)
                return new TileGlyph(s);

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is TileGlyph g)
                return g.ToString();

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class Tile
    {
        public string Tag;
        public TileGlyph Glyph;
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
                    Glyph = new TileGlyph(0,0,TileGlyph.TERRAIN), // TODO: Split dirt and stone floors
                },

                new Tile
                {
                    Tag = "floor_wood",
                    Natural = false,
                    Flamable = true,
                    Glyph = new TileGlyph(8,0,TileGlyph.TERRAIN),
                }, // TODO: wood 2

                new Tile
                {
                    Tag = "grass",
                    Flamable = true,
                    BlockSight = true,
                    Glyph = new TileGlyph(4,0,TileGlyph.TERRAIN),
                },

                new Tile
                {
                    Tag = "water",
                    WalkDanger = 0.5f,
                    MineCost = 15f,
                    Glyph = new TileGlyph(56,3,TileGlyph.PIT),
                },

                new Tile
                {
                    Tag = "acid",
                    WalkDanger = 2.5f,
                    MineCost = 15f,
                    Glyph = new TileGlyph(56,6,TileGlyph.PIT),
                },

                new Tile
                {
                    Tag = "lava",
                    WalkDanger = 10,
                    MineCost = 15f,
                    Glyph = new TileGlyph(56,9,TileGlyph.PIT),
                },

                new Tile
                {
                    Tag = "rock",
                    Solid = true,
                    BlockSight = true,
                    Glyph = new TileGlyph(0,4,TileGlyph.WALL), // TODO: 4 kinds of rock
                    MineCost = 2,
                },

                new Tile
                {
                    Tag = "rock2",
                    Solid = true,
                    BlockSight = true,
                    Glyph = new TileGlyph(4,4,TileGlyph.WALL), // TODO: 4 kinds of rock
                    MineCost = 2,
                },

                new Tile
                {
                    Tag = "wall_stone",
                    Solid = true,
                    BlockSight = true,
                    Glyph = new TileGlyph(16,4,TileGlyph.WALL), // TODO: Two walls
                    MineCost = 10,
                    Natural = false,
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

        public static Tile GetTile(this List<Tile> tiles, string tag)
        {
            var tileId = tiles.FindIndex(t => t.Tag == tag);
            Debug.Assert(tileId != -1);
            return tiles[tileId];
        }
    }
}
