using System;
using HackConsole;

namespace SurvivalHack.ECM
{
    public class MoveComponent
    {
        public Vec Pos { get; set; }
        public World World;
        public TerrainFlag MovementType = TerrainFlag.Walk;

        public virtual bool Walk(Entity self, Vec direction)
        {
            var newPosition = Pos;
            newPosition += direction;

            // You cannot walk of the edge of map
            if (!World.InBoundary(newPosition.X, newPosition.Y))
                return false;

            // Terrain collisions
            if (!World.HasFlag(newPosition.X, newPosition.Y, MovementType))
                return false;

            Pos = newPosition;
            self.FoV?.Update(this);

            return true;
        }

        internal void Remove(Entity self)
        {
            World.Creatures.Remove(self);
        }
    }
}
