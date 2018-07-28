using HackConsole;
using System.Collections.Generic;

namespace SurvivalHack.Mapgen
{
    public class Room
    {
        public Level Level = null;

        public Transform Transform;
        public Vec Size => Tiles.Size;

        public Vec Center => Transform.Convert(Size / 2);

        public Grid<int?> Tiles;

        public Room(Vec size)
        {
            Tiles = new Grid<int?>(size);
        }

        public void Render(Grid<int> maskMap, int roomID)
        {
            foreach(var v in Tiles.Ids())
            { 
                var dest = Transform.Convert(v);
                if (maskMap[dest] == DungeonGenerator.MASKID_KEEP)
                    continue;

                if (Tiles[v] is int i)
                {
                    int mask;
                    Tile tile = Level.TileDefs[i];

                    if (tile.Flags.HasFlag(TerrainFlag.Walk))
                    {
                        mask = roomID;
                    }
                    else if (i == Level.TileDefs.Get("rock"))
                    {
                        mask = DungeonGenerator.MASKID_NOFLOOR;
                    }
                    else
                    {
                        mask = DungeonGenerator.MASKID_KEEP;
                    }

                    Level.TileMap[dest] = i;
                    maskMap[dest] = mask;
                }
            }
        }

        public bool TryPlaceOnMap(Level level, Grid<int> maskMap, List<Room> rooms)
        {
            var mapRect = new Rect(Vec.Zero, level.Size);

            if (!mapRect.Contains(Transform.Convert(Vec.Zero)) || !mapRect.Contains(Transform.Convert(Size - new Vec(1,1))))
                return false;

            foreach (var source in Tiles.Ids())
            {
                if (Tiles[source] is int newId)
                {
                    var dest = Transform.Convert(source);

                    var oldId = level.TileMap[dest];
                    var oldMask = maskMap[dest];


                    if (newId == oldId)
                        continue;

                    switch (oldMask)
                    {
                        case DungeonGenerator.MASKID_NOFLOOR:
                            if (level.TileDefs[newId].Flags.HasFlag(TerrainFlag.Walk))
                                return false;
                            break;
                        case DungeonGenerator.MASKID_VOID:
                            break;
                        case DungeonGenerator.MASKID_KEEP:
                            if (newId != level.TileDefs.Get("rock"))
                                return false;
                            break;
                        default: // Floors
                            return false;
                    }
                }
            }

            Level = level;
            Render(maskMap, rooms.Count);
            rooms.Add(this);
            return true;
        }
    }
}
