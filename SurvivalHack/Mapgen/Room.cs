using HackConsole;
using System;
using System.Collections.Generic;

namespace SurvivalHack.Mapgen
{
    public class Room
    {
        public Level Level = null;

        public Transform Transform;
        public Size Size => Tiles.Size;

        public Vec Center => Transform.Convert(Size.Center);

        public Grid<TileInfo> Tiles;

        public Room(Size size)
        {
            Tiles = new Grid<TileInfo>(size);
        }

        public void Render(Grid<int> maskMap, int roomID)
        {
            foreach (var v in Tiles.Ids())
            {
                var dest = Transform.Convert(v);
                if (maskMap[dest] == DungeonGenerator.MASKID_KEEP)
                    continue;

                var info = Tiles[v];

                switch (info.Method)
                {
                    case PasteMethod.Paste:
                        Level.TileMap[dest] = info.Id;
                        maskMap[dest] = Level.TileDefs[info.Id].Solid ? roomID : DungeonGenerator.MASKID_KEEP;
                        break;
                    case PasteMethod.NoFloor:
                        maskMap[dest] = DungeonGenerator.MASKID_NOFLOOR;
                        break;
                    case PasteMethod.Nil:
                        break;
                }
            }
        }

        public bool CanRender(Grid<int> maskMap)
        {
            var mapRect = new Rect(Vec.Zero, Level.Size);

            if (!mapRect.Contains(Transform.Convert(Vec.Zero)) || !mapRect.Contains(Transform.Convert(Size.BottomRight)))
                return false;

            foreach (var vecSrc in Tiles.Ids())
            {
                var newInfo = Tiles[vecSrc];
                var vecDest = Transform.Convert(vecSrc);

                var oldId = Level.TileMap[vecDest];
                var oldMask = maskMap[vecDest];

                switch (newInfo.Method)
                {
                    case PasteMethod.Paste:
                        switch (oldMask)
                        {
                            case DungeonGenerator.MASKID_NOFLOOR:
                                if (Level.TileDefs[newInfo.Id].IsFloor)
                                    return false;
                                break;
                            case DungeonGenerator.MASKID_VOID:
                                break;
                            case DungeonGenerator.MASKID_KEEP:
                            default: // Floors
                                return false;
                        }
                        break;
                    case PasteMethod.Nil:
                        break;
                    case PasteMethod.NoFloor:
                        if (Level.TileDefs[oldId].IsFloor)
                            return false;
                        break;
                }
            }
            return true;
        }

        public bool TryPlaceOnMap(Level level, Grid<int> maskMap, List<Room> rooms)
        {
            var mapRect = new Rect(Vec.Zero, level.Size);

            if (!mapRect.Contains(Transform.Convert(Vec.Zero)) || !mapRect.Contains(Transform.Convert(Size.BottomRight)))
                return false;
            
            Level = level;
            if (!CanRender(maskMap))
                return false;
            Render(maskMap, rooms.Count);
            rooms.Add(this);
            return true;
        }

        public enum PasteMethod
        {
            Nil, Paste, NoFloor
        }

        public struct TileInfo
        {
            public int Id;
            public PasteMethod Method;

            public static TileInfo Empty => new TileInfo { Id = 0, Method = PasteMethod.Nil };

            public static bool operator==(TileInfo l, TileInfo r)
            {
                return l.Id == r.Id && l.Method == r.Method;
            }

            public static bool operator!=(TileInfo l, TileInfo r)
            {
                return !(l == r);
            }
        }
    }
}
