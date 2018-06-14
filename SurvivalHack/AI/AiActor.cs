using System;
using HackConsole;
using SurvivalHack.ECM;

namespace SurvivalHack.Ai
{
    public class AiActor
    {
        internal void Move(Entity self, Goal goal)
        {
            switch (goal.TargetAction)
            {
                case ETargetAction.Ignore:
                    MoveRandom(self);
                    break;
                case ETargetAction.Chase:
                    MoveTo(self, goal.Target);
                    break;
                case ETargetAction.Fear:
                    // TODO: Run away
                    break;
                case ETargetAction.Follow:
                    MoveTo(self, goal.Target);
                    break;
                case ETargetAction.Hate:
                    MoveTo(self, goal.Target);
                    break;
            }
        }

        private void MoveRandom(Entity self)
        {
            var move = new Range(-1, 1);
            for (var i = 0; i < 10; i++)
            {
                var delta = new Vec(move.Rand(Game.Rnd), move.Rand(Game.Rnd));
                
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

        public void StandardAction(Entity self, Goal goal)
        {
            switch (goal.TargetAction)
            {
                case ETargetAction.Ignore:
                case ETargetAction.Chase:
                case ETargetAction.Fear:
                case ETargetAction.Follow:
                    break;
                case ETargetAction.Hate:
                    ActionAttackEnemy(self, goal.Target);
                    break;
            }
        }

        private bool ActionAttackEnemy(Entity self, Entity enemy)
        {
            if (enemy != null)
            {
                (var weapon, var comp) = self.GetWeapon(enemy);

                if (weapon != null)
                {
                    Eventing.On(new AttackEvent(self, weapon, enemy, comp.AttackMove));
                    return true;
                }
            }
            return false;
        }
    }

    public class ActEvent : ITimeEvent {
        readonly Entity _entity;

        public ActEvent(Entity entity)
        {
            _entity = entity;
        }

        public int RepeatTurns => _entity.EntityFlags.HasFlag(EEntityFlag.Destroyed) ? -1 : 1;

        public void Run()
        {
            if (_entity.EntityFlags.HasFlag(EEntityFlag.Destroyed))
                return;

            var goal = _entity.Attitude.GetGoal(_entity);

            // Moving.
            _entity.LeftoverMove += _entity.Speed;
            for (var i = 1; i <= _entity.LeftoverMove; i++)
                _entity.Ai.Move(_entity, goal);
            _entity.LeftoverMove = _entity.LeftoverMove - (int)_entity.LeftoverMove;

            // Acting
            _entity.Ai.StandardAction(_entity, goal);
        }
    }
}
