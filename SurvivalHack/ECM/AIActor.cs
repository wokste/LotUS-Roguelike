using System;
using HackConsole;

namespace SurvivalHack.ECM
{
    public class AiActor
    {
        // TODO: Move sspeed
        // TODO: Attack speed
        // TODO: Split movement and attack

        public void Act(Entity self)
        {
            if (self.Enemy == null || !self.Enemy.Alive)
                self.Enemy = self.Attitude.FindEnemy(self);

            if (self.Enemy == null)
            {
                Wander(self);
                return;
            }

            AttackEnemy(self);
        }

        private void Wander(Entity self)
        {
            for (var i = 0; i < 10; i++)
            {
                var delta = new Vec(Dicebag.UniformInt(-1,2), Dicebag.UniformInt(-1, 2));
                
                if (self.Move.Move(self, delta))
                    return;
            }
        }

        private void ChaseEnemy(Entity self)
        {
            var delta = self.Enemy.Move.Pos - self.Move.Pos;

            var deltaClamped = new Vec(MyMath.Clamp(delta.X, -1, 1), MyMath.Clamp(delta.Y, -1, 1));

            if (delta == deltaClamped)
                return;

            if (self.Move.Move(self, deltaClamped))
                return;

            if (Math.Abs(delta.X) > Math.Abs(delta.Y))
            {
                if (self.Move.Move(self, new Vec(deltaClamped.X, 0)))
                    return;

                if (self.Move.Move(self, new Vec(0, deltaClamped.Y)))
                    return;
            }
            else
            {
                if (self.Move.Move(self, new Vec(0, deltaClamped.Y)))
                    return;
                
                if (self.Move.Move(self, new Vec(deltaClamped.X, 0)))
                    return;
            }

            // Fallback. self.Enemy could not be reached.
            self.Enemy = null;
            Wander(self);
        }

        private bool AttackEnemy(Entity self)
        {
            if (self.Enemy != null && self.Attack != null && self.Attack.InRange(self, self.Enemy))
            {
                self.Attack.Attack(self, self.Enemy);
                return true;
            }
            return false;
        }

        internal void Move(Entity self)
        {
            if (self.Enemy != null)
                ChaseEnemy(self);
            else
                Wander(self);
        }
    }

    public class ActEvent : IEvent {
        Entity _entity;

        public ActEvent(Entity entity)
        {
            _entity = entity;
        }

        public int RepeatTurns => _entity.Alive ? 1 : -1;

        public void Run()
        {
            if (!_entity.Alive)
                return;

            _entity.Attitude.FindEnemy(_entity);

            // Moving.
            _entity.LeftoverMove += _entity.Speed;
            for (var i = 1; i <= _entity.LeftoverMove; i++)
                _entity.Ai.Move(_entity);
            _entity.LeftoverMove = _entity.LeftoverMove - (int)_entity.LeftoverMove;

            // Acting
            _entity.Ai.Act(_entity);
        }
    }
}
