using System;
using HackConsole;

namespace SurvivalHack.ECM
{
    public class AiController
    {
        // TODO: Move sspeed
        // TODO: Attack speed
        // TODO: Split movement and attack

        public void Act(Monster self)
        {
            if (self.Enemy == null || !self.Enemy.Alive)
                self.Enemy = FindEnemy(self);

            if (self.Enemy == null)
            {
                Wander(self);
                return;
            }

            AttackEnemy(self);
        }

        private void Wander(Monster self)
        {
            for (var i = 0; i < 10; i++)
            {
                var delta = new Vec(Dicebag.UniformInt(-1,2), Dicebag.UniformInt(-1, 2));
                
                if (self.Walk(delta))
                    return;
            }
        }

        private void ChaseEnemy(Monster self)
        {
            var delta = self.Enemy.Position - self.Position;

            var deltaClamped = new Vec(MyMath.Clamp(delta.X, -1, 1), MyMath.Clamp(delta.Y, -1, 1));

            if (delta == deltaClamped)
                return;

            if (self.Walk(deltaClamped))
                return;

            if (Math.Abs(delta.X) > Math.Abs(delta.Y))
            {
                if (self.Walk(new Vec(deltaClamped.X, 0)))
                    return;

                if (self.Walk(new Vec(0, deltaClamped.Y)))
                    return;
            }
            else
            {
                if (self.Walk(new Vec(0, deltaClamped.Y)))
                    return;
                
                if (self.Walk(new Vec(deltaClamped.X, 0)))
                    return;
            }

            // Fallback. self.Enemy could not be reached.
            self.Enemy = null;
            Wander(self);
        }

        private bool AttackEnemy(Monster self)
        {
            if (self.Enemy != null && self.Attack != null && self.Attack.InRange(self, self.Enemy))
            {
                self.Attack.Attack(self, self.Enemy);
                return true;
            }
            return false;
        }

        internal Entity FindEnemy(Monster self)
        {
            foreach(var c in self.Map.Creatures)
            {
                if (c == self)
                    continue;

                if (AttitudeSee(self, c) == EAttitude.Ignore)
                    continue;

                var delta = self.Position - c.Position;

                if (delta.LengthSquared < 100) // Todo: Sight radius
                    return c;
            }
            return null;
        }

        private EAttitude AttitudeSee(Monster self, Entity other) {
            return (other is Player) ? EAttitude.Hate : EAttitude.Ignore;
        }

        internal void Move(Monster self)
        {
            if (self.Enemy != null)
                ChaseEnemy(self);
            else
                Wander(self);
        }
    }

    internal enum EAttitude {
        Ignore, // Ignores other character
        Follow, // Follow it
        Hate,   // Attacks it
        Fear,   // Run away
    }
}
