using System;
using HackConsole;

namespace SurvivalHack.ECM
{
    public class MoveComponent
    {
        public Vec Pos { get; set; }
        public Level Level;

        private MoveComponent()
        {
        }

        public static void Bind(Entity self, Level level, Vec pos)
        {
            var c = level.GetChunck(pos);
            c.Add(self);

            self.Move = new MoveComponent
            {
                Level = level,
                Pos = pos
            };
        }

        internal void Unbind(Entity self)
        {
            Level.GetChunck(Pos).Remove(self);
            self.Move = null;
        }

        public virtual bool Move(Entity self, Vec direction)
        {
            var newPosition = Pos;
            newPosition += direction;

            // You cannot walk of the edge of map
            if (!Level.InBoundary(newPosition))
                return false;

            // Terrain collisions
            if (!Level.HasFlag(newPosition, self.Flags))
                return false;

            var oldChunk = Level.GetChunck(Pos);
            Pos = newPosition;
            var newChunk = Level.GetChunck(Pos);

            if (oldChunk != newChunk)
            {
                oldChunk.Remove(self);
                newChunk.Add(self);
            }

            self.GetOne<FieldOfView>()?.Update(this);

            return true;
        }
    }
}
