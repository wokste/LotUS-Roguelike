using System;
using HackConsole;

namespace SurvivalHack.Mapgen.Rooms
{
    internal class RectRoomFactory : RoomFactory
    {
        public Range RangeX = new Range("4-10");
        public Range RangeY = new Range("4-10");
        public Tile FloorTile;
        public Tile WallTile;

        public RectRoomFactory()
        {
            Name = "RoomType";
            FloorTile = TileList.Get("floor");
            WallTile = TileList.Get("wall");
        }

        public override Room Make(Random rnd)
        {
            var innerSize = new Vec(RangeX.Rand(rnd), RangeY.Rand(rnd));
            if (rnd.Next(2) == 1)
                innerSize = new Vec(innerSize.Y, innerSize.X);
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