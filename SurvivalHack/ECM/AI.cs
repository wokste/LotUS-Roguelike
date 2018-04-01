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
                
                if (self.Move.Walk(self, delta))
                    return;
            }
        }

        private void ChaseEnemy(Monster self)
        {
            var delta = self.Enemy.Move.Pos - self.Move.Pos;

            var deltaClamped = new Vec(MyMath.Clamp(delta.X, -1, 1), MyMath.Clamp(delta.Y, -1, 1));

            if (delta == deltaClamped)
                return;

            if (self.Move.Walk(self, deltaClamped))
                return;

            if (Math.Abs(delta.X) > Math.Abs(delta.Y))
            {
                if (self.Move.Walk(self, new Vec(deltaClamped.X, 0)))
                    return;

                if (self.Move.Walk(self, new Vec(0, deltaClamped.Y)))
                    return;
            }
            else
            {
                if (self.Move.Walk(self, new Vec(0, deltaClamped.Y)))
                    return;
                
                if (self.Move.Walk(self, new Vec(deltaClamped.X, 0)))
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
            var pos = self.Move.Pos;

            // Todo: Sight radius
            var area = new Rect(pos - new Vec(10, 10), new Vec(21, 21));

            foreach(var e in self.Move.Level.GetEntities(area))
            {
                if (e == self)
                    continue;

                if (AttitudeSee(self, e) == EAttitude.Ignore)
                    continue;

                var delta = self.Move.Pos - e.Move.Pos;

                if (delta.LengthSquared < 100) // Todo: Sight radius
                    return e;
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
