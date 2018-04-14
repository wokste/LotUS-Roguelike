using System;
using HackConsole;

namespace SurvivalHack.ECM
{
    public class AiActor
    {
        internal void Move(Entity self)
        {
            switch (self.Attitude.TargetAction)
            {
                case EAttitude.Ignore:
                    break;
                case EAttitude.Chase:
                    MoveTo(self, self.Attitude.Target);
                    break;
                case EAttitude.Fear:
                    // TODO: Run away
                    break;
                case EAttitude.Follow:
                    MoveTo(self, self.Attitude.Target);
                    break;
                case EAttitude.Hate:
                    MoveTo(self, self.Attitude.Target);
                    break;
            }

            if (self.Attitude.Target != null)
                MoveTo(self, self.Attitude.Target);
            else
                MoveRandom(self);
        }

        private void MoveRandom(Entity self)
        {
            for (var i = 0; i < 10; i++)
            {
                var delta = new Vec(Dicebag.UniformInt(-1,2), Dicebag.UniformInt(-1, 2));
                
                if (self.Move.Move(self, delta))
                    return;
            }
        }

        private void MoveTo(Entity self, Entity other)
        {
            var delta = other.Move.Pos - self.Move.Pos;

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
            MoveRandom(self);
        }

        public void StandardAction(Entity self)
        {
            switch (self.Attitude.TargetAction)
            {
                case EAttitude.Ignore:
                case EAttitude.Chase:
                case EAttitude.Fear:
                case EAttitude.Follow:
                    break;
                case EAttitude.Hate:
                    ActionAttackEnemy(self, self.Attitude.Target);
                    break;
            }
        }

        private bool ActionAttackEnemy(Entity self, Entity enemy)
        {
            if (enemy != null && self.Attack != null && self.Attack.InRange(self, enemy))
            {
                self.Attack.Attack(self, enemy);
                return true;
            }
            return false;
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

            _entity.Attitude.UpdateTarget(_entity);

            // Moving.
            _entity.LeftoverMove += _entity.Speed;
            for (var i = 1; i <= _entity.LeftoverMove; i++)
                _entity.Ai.Move(_entity);
            _entity.LeftoverMove = _entity.LeftoverMove - (int)_entity.LeftoverMove;

            // Acting
            _entity.Ai.StandardAction(_entity);
        }
    }
}
