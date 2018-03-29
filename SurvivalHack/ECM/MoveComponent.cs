using System;
using HackConsole;

namespace SurvivalHack.ECM
{
    public class MoveComponent
    {
        public Vec Pos { get; set; }
        public World World;
        public TerrainFlag Flags;

        public virtual bool Walk(Entity self, Vec direction)
        {
            var newPosition = Pos;
            newPosition += direction;

            // You cannot walk of the edge of map
            if (!World.InBoundary(newPosition.X, newPosition.Y))
                return false;

            // Terrain collisions
            if (!World.HasFlag(newPosition.X, newPosition.Y, Flags))
                return false;

            var oldChunk = World.GetChunck(Pos);
            Pos = newPosition;
            var newChunk = World.GetChunck(Pos);

            if (oldChunk != newChunk)
            {
                oldChunk.Remove(self);
                newChunk.Add(self);
            }

            self.FoV?.Update(this);

            return true;
        }

        internal void Remove(Entity self)
        {
            World.GetChunck(Pos).Remove(self);
        }

        
        public void AddToMap(World map, Entity self)
        {
            World = map;
            //Time.AddRelative(controller, 1000);
            World.GetChunck(Pos).Add(self);
        }
    }
}
