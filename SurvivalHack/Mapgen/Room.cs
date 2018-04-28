using System.Collections.Generic;
using System.Linq;
using HackConsole;

namespace SurvivalHack.Mapgen
{
    public class Room
    {
        internal AbstractMap Map = null;

        internal Transform Transform;
        internal Vec Size => Tiles.Size;

        public Vec Center => Transform.Convert(Size / 2);

        internal Grid<Tile> Tiles;

        internal Room(Vec size)
        {
            Tiles = new Grid<Tile>(size);
        }

        internal void Render()
        {
            foreach(var v in Tiles.Ids())
            { 
                var dest = Transform.Convert(v);

                if (Tiles[v] == null || Map.MaskMap[dest] == AbstractMap.MASKID_KEEP)
                    continue;

                int mask;
                if (Tiles[v].Flags.HasFlag(TerrainFlag.Walk))
                {
                    mask = 0; // TODO: room id.
                }
                else if (Tiles[v] == TileList.Get("rock"))
                {
                    mask = AbstractMap.MASKID_NOFLOOR;
                }
                else
                {
                    mask = AbstractMap.MASKID_KEEP;
                }

                Map.Set(dest, Tiles[v], mask);
            }
        }

        internal bool TryPlaceOnMap(AbstractMap map)
        {
            var mapRect = new Rect(Vec.Zero, map.Size);

            if (!mapRect.Contains(Transform.Convert(Vec.Zero)) || !mapRect.Contains(Transform.Convert(Size - new Vec(1,1))))
                return false;

            foreach (var source in Tiles.Ids())
            {
                var newTile = Tiles[source];
                var dest = Transform.Convert(source);

                var oldTile = map.TileMap[dest];
                var oldMask = map.MaskMap[dest];

                if (newTile == null)
                    continue;

                if (newTile == oldTile)
                    continue;

                switch (oldMask)
                {
                    case AbstractMap.MASKID_NOFLOOR:
                        if (newTile.Flags.HasFlag(TerrainFlag.Walk))
                            return false;
                        break;
                    case AbstractMap.MASKID_VOID:
                        break;
                    case AbstractMap.MASKID_KEEP:
                        if (oldTile != newTile && newTile != TileList.Get("rock"))
                            return false;
                        break;
                    default: // Floors
                        return false;
                }
            }

            Map = map;
            Render();
            map.Rooms.Add(this);
            return true;
        }
    }
}
