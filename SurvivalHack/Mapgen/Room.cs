﻿using HackConsole;

namespace SurvivalHack.Mapgen
{
    public class Room
    {
        internal Level Level = null;

        internal Transform Transform;
        internal Vec Size => Tiles.Size;

        public Vec Center => Transform.Convert(Size / 2);

        internal Grid<Tile> Tiles;

        internal Room(Vec size)
        {
            Tiles = new Grid<Tile>(size);
        }

        internal void Render(Grid<int> maskMap)
        {
            foreach(var v in Tiles.Ids())
            { 
                var dest = Transform.Convert(v);

                if (Tiles[v] == null || maskMap[dest] == DungeonGenerator.MASKID_KEEP)
                    continue;

                int mask;
                if (Tiles[v].Flags.HasFlag(TerrainFlag.Walk))
                {
                    mask = 0; // TODO: room id.
                }
                else if (Tiles[v] == TileList.Get("rock"))
                {
                    mask = DungeonGenerator.MASKID_NOFLOOR;
                }
                else
                {
                    mask = DungeonGenerator.MASKID_KEEP;
                }

                Level.TileMap[dest] = Tiles[v];
                maskMap[dest] = mask;
            }
        }

        internal bool TryPlaceOnMap(Level level, Grid<int> maskMap)
        {
            var mapRect = new Rect(Vec.Zero, level.Size);

            if (!mapRect.Contains(Transform.Convert(Vec.Zero)) || !mapRect.Contains(Transform.Convert(Size - new Vec(1,1))))
                return false;

            foreach (var source in Tiles.Ids())
            {
                var newTile = Tiles[source];
                var dest = Transform.Convert(source);

                var oldTile = level.TileMap[dest];
                var oldMask = maskMap[dest];

                if (newTile == null)
                    continue;

                if (newTile == oldTile)
                    continue;

                switch (oldMask)
                {
                    case DungeonGenerator.MASKID_NOFLOOR:
                        if (newTile.Flags.HasFlag(TerrainFlag.Walk))
                            return false;
                        break;
                    case DungeonGenerator.MASKID_VOID:
                        break;
                    case DungeonGenerator.MASKID_KEEP:
                        if (oldTile != newTile && newTile != TileList.Get("rock"))
                            return false;
                        break;
                    default: // Floors
                        return false;
                }
            }

            Level = level;
            Render(maskMap);
            return true;
        }
    }
}
