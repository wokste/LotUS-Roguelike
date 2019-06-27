using System;
using System.Collections.Generic;
using System.Linq;
using HackConsole;
using SurvivalHack.Combat;
using SurvivalHack.ECM;

namespace SurvivalHack.Ai
{
    public class AiActor
    {
        public void Move(Entity self, Goal goal)
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
                
                if (self.TryMove(delta))
                    return;
            }
        }

        private void MoveTo(Entity self, Entity other)
        {
            var delta = other.Pos - self.Pos;

            var deltaClamped = new Vec(MyMath.Clamp(delta.X, -1, 1), MyMath.Clamp(delta.Y, -1, 1));

            if (delta == deltaClamped)
                return;

            if (self.TryMove(deltaClamped))
                return;

            if (Math.Abs(delta.X) > Math.Abs(delta.Y))
            {
                if (self.TryMove(new Vec(deltaClamped.X, 0)))
                    return;

                if (self.TryMove(new Vec(0, deltaClamped.Y)))
                    return;
            }
            else
            {
                if (self.TryMove(new Vec(0, deltaClamped.Y)))
                    return;
                
                if (self.TryMove(new Vec(deltaClamped.X, 0)))
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


        public static IEnumerable<(Entity, IWeapon)> ChooseWeapon(Entity self)
        {
            IEnumerable<Entity> It()
            {
                var inventory = self.GetOne<Inventory>();

                yield return inventory?.Slots[Inventory.SLOT_WAND].Item;
                yield return inventory?.Slots[Inventory.SLOT_RANGED].Item;
                yield return inventory?.Slots[Inventory.SLOT_MAINHAND].Item;
                yield return self;
            }

            return It().Select(i => (i, i?.GetOne<IWeapon>())).Where(pair => pair.Item2 != null);
        }

        private bool ActionAttackEnemy(Entity self, Entity enemy)
        {
            if (enemy == null)
                return false;

            foreach (var weaponPair in ChooseWeapon(self))
            {
                var weaponComponent = weaponPair.Item2;
                var dir = weaponComponent.Dir(self, enemy);
                if (dir == null)
                    continue;

                var targets = weaponComponent.Targets(self, dir.Value);
                if (!targets.Contains(enemy))
                    continue;

                CombatSystem.RollAttack(self, targets.ToArray(), weaponPair);
                return true;
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
            _entity.LeftoverMove -= (int)_entity.LeftoverMove;

            // Acting
            _entity.Ai.StandardAction(_entity, goal);
        }
    }
}
