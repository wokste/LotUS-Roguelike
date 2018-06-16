using SurvivalHack.Combat;
using SurvivalHack.ECM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalHack
{
    static class Eventing
    {
        public static bool On(BaseEvent message)
        {
            var funcs = new List<UseFunc>();
            funcs.AddRange(message.Item.Components.SelectMany(c => c.GetActions(message, EUseSource.This)));

            if (message.Target != null)
            {
                funcs.AddRange(message.Target.Components.SelectMany(c => c.GetActions(message, EUseSource.Target)));
                funcs.AddRange(message.Target.ListSubEntities().SelectMany(e => e.Components.SelectMany(c => c.GetActions(message, EUseSource.TargetItem))));
            }

            if (message.Self != null)
            {
                funcs.AddRange(message.Self.Components.SelectMany(c => c.GetActions(message, EUseSource.User)));
                funcs.AddRange(message.Self.ListSubEntities().SelectMany(e => e.Components.SelectMany(c => c.GetActions(message, EUseSource.UserItem))));
            }

            if (!funcs.Any(f => f.Order == EUseOrder.Event))
            {
                return false;
            }

            if (funcs.Any(f => f.Order == EUseOrder.Interrupt))
            {
                return false;
            }

            foreach (var f in funcs.OrderBy(f => f.Order))
            {
                f.Action?.Invoke(message);
            }

            return true;
        }
    }

    public class BaseEvent
    {
        public Entity Self;
        public Entity Item;
        public Entity Target;
    }

    public class DrinkEvent : BaseEvent
    {
        public DrinkEvent(Entity self, Entity item)
        {
            Self = self;
            Item = item;
        }
    }

    public class CastEvent : BaseEvent
    {
        public CastEvent(Entity self, Entity item)
        {
            Self = self;
            Item = item;
        }
    }

    public class ThreatenEvent : BaseEvent
    {
        public ThreatenEvent(Entity self, Entity target)
        {
            Self = self;
            Target = target;
        }
    }

    public class AttackEvent : BaseEvent
    {
        public EAttackState State = EAttackState.Hit;
        public EAttackMove Move;
        public EDamageLocation Location;

        public AttackEvent(Entity self, Entity weapon, Entity target, EAttackMove move)
        {
            Self = self;
            Item = weapon;
            Target = target;
            Move = move;
            Location = GetRandomLocation();
        }

        private EDamageLocation GetRandomLocation()
        {
            var rnd = Game.Rnd.NextDouble();
            if (rnd < 0.75)
                return EDamageLocation.Body;
            else if (rnd < 0.9)
                return EDamageLocation.Head;
            else if (rnd < 0.95)
                return EDamageLocation.Hands;
            else
                return EDamageLocation.Feet;
        }
    }

    public class DamageEvent : BaseEvent
    {
        public int Damage;
        public EDamageType DamageType;
        public EDamageLocation Location;
        public bool Significant => (Damage > 0);

        public DamageEvent(BaseEvent prevMessage, int damage, EDamageType damageType, EDamageLocation location)
        {
            Self = prevMessage.Self;
            Item = prevMessage.Item;
            Target = prevMessage.Target;
            Damage = damage;
            DamageType = damageType;
            Location = location;
        }
    }
}
