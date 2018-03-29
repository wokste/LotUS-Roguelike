using System;
using HackConsole;

namespace SurvivalHack.ECM
{
    public class MoveComponent
    {
        public Vec Pos { get; set; }
        public World World;

        private MoveComponent()
        {
        }

        public static void Bind(Entity self, World world, Vec? pos = null)
        {
            var pos2 = pos ?? world.GetEmptyLocation(self.Flags);

            var c = world.GetChunck(pos2);
            c.Add(self);

            self.Move = new MoveComponent
            {
                World = world,
                Pos = pos2
            };
        }

        internal void Unbind(Entity self)
        {
            World.GetChunck(Pos).Remove(self);
            self.Move = null;
        }

        public virtual bool Walk(Entity self, Vec direction)
        {
            var newPosition = Pos;
            newPosition += direction;

            // You cannot walk of the edge of map
            if (!World.InBoundary(newPosition.X, newPosition.Y))
                return false;

            // Terrain collisions
            if (!World.HasFlag(newPosition.X, newPosition.Y, self.Flags))
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
    }
}
