using System;
using System.Collections.Generic;
using System.Drawing;
using HackConsole;

namespace SurvivalHack.Mapgen
{
    public class AbstractMap
    {
        internal Vec Size;
        internal Random Rnd = new Random();
        private int _occupiedTiles = 0;

        internal double PercOccupied => _occupiedTiles / (double)Size.Area;

        internal List<Room> Rooms = new List<Room>();
        internal Grid<Tile> TileMap;
        internal Grid<int> MaskMap;

        internal AbstractMap(Vec size)
        {
            Size = size;
            TileMap = new Grid<Tile>(Size);
            MaskMap = new Grid<int>(Size);

            foreach (var v in TileMap.Ids())
            {
                TileMap[v] = TileList.Get("rock");
                MaskMap[v] = MASKID_VOID;
            }
        }

        internal void Set(Vec pos, Tile tile, int mask)
        {
            TileMap[pos] = tile;
            MaskMap[pos] = mask;
        }

        public const int MASKID_VOID = -3;
        public const int MASKID_NOFLOOR = -2;
        public const int MASKID_KEEP = -1;
    }
}
