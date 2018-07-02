using HackConsole;

namespace SurvivalHack.ECM
{
    public class MoveComponent
    {
        public Vec Pos { get; set; }
        public Level Level { get; private set; }

        private MoveComponent()
        {
        }

        public static void Bind(Entity self, Level level, Vec pos)
        {
            self.Move?.Unbind(self);

            var c = level.GetChunck(pos);
            c.Add(self);

            self.Move = new MoveComponent
            {
                Level = level,
                Pos = pos
            };
        }

        public void Unbind(Entity self)
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

            // Monster collisions
            if (self.EntityFlags.HasFlag(EEntityFlag.Blocking))
                foreach (var c in Level.GetEntities(newPosition))
                    if (c.EntityFlags.HasFlag(EEntityFlag.Blocking))
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
