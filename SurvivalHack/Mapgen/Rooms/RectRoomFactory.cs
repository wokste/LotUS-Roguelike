using System;
using System.Collections.Generic;
using HackConsole;

namespace SurvivalHack.Mapgen.Rooms
{
    public class RectRoomFactory : RoomFactory
    {
        public Range RangeX = new Range("4-10");
        public Range RangeY = new Range("4-10");
        public Room.TileInfo FloorTile;
        public Room.TileInfo WallTile;

        public RectRoomFactory(List<Tile> tileDefs)
        {
            Name = "RoomType";
            FloorTile = new Room.TileInfo { Id = tileDefs.Get("floor_wood"), Method = Room.PasteMethod.Paste };
            WallTile = new Room.TileInfo { Id = tileDefs.Get("wall_stone"), Method = Room.PasteMethod.Paste };
        }

        public override Room Make(Random rnd)
        {
            var innerSize = new Size(RangeX.Rand(rnd), RangeY.Rand(rnd));
            if (rnd.Next(2) == 1)
                innerSize = new Size(innerSize.Y, innerSize.X);
            var outerSize = innerSize + new Vec(2, 2);
            
            var room = new Room(outerSize);

            Draw(room);

            return room;
        }

        private void Draw(Room r)
        {
            var size = r.Size;

            foreach (var v in r.Tiles.Ids())
            {
                var isWall = (v.X == 0) || (v.Y == 0) || (v.X == size.X - 1) || (v.Y == size.Y - 1);
                r.Tiles[v] = isWall ? WallTile : FloorTile;
            }
        }
    }
}