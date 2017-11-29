using System;
using System.Data;
using System.Drawing;

namespace HackLib
{
    public class Creature
    {
        public String Name;
        public Bar HitPoints;
        public Attack Attack;

        public Point Position { get; set; }
        
        public bool Walk(Point direction, TileGrid map)
        {
            // TODO: Precondition |direction| == 1

            var newPosition = Position;
            newPosition.Offset(direction);

            // You cannot walk of the edge of map
            if (newPosition.X < 0 || newPosition.X >= map.Width || newPosition.Y < 0 || newPosition.Y >= map.Height)
                return false;

            // Terrain collisions;
            if (!TileTypeList.Get(map.Grid[newPosition.X, newPosition.Y]).Walkable)
                return false;

            // TODO: Creature collision

            Position = newPosition;
            return true;
        }
    }
}
