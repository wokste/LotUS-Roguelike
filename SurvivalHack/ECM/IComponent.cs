using SurvivalHack.Combat;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SurvivalHack.ECM
{
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

        public AttackEvent(Entity self, Entity weapon, Entity target, EAttackMove move)
        {
            Self = self;
            Item = weapon;
            Target = target;
            Move = move;
        }
    }

    public class DamageEvent : BaseEvent
    {
        public int Damage;
        public EDamageType DamageType;
        public bool Significant => (Damage > 0);

        public DamageEvent(BaseEvent prevMessage, int damage, EDamageType damageType)
        {
            Self = prevMessage.Self;
            Item = prevMessage.Item;
            Target = prevMessage.Target;
            Damage = damage;
            DamageType = damageType;
        }
    }


    public interface IComponent
    {
        string Describe();
        IEnumerable<UseFunc> GetActions(BaseEvent message, EUseSource source);
    }

    public struct UseFunc {
        public EUseOrder Order;
        public Action<BaseEvent> Action;

        public UseFunc(Action<BaseEvent> action, EUseOrder order = EUseOrder.Event)
        {
            Action = action;
            Order = order;
        }
    }

    public enum EUseOrder
    {
        Interrupt,
        PreEvent,
        Event,
        PostEvent,
    }

    public enum EUseSource
    {
        User,
        UserItem,
        This,
        Target,
        TargetItem,
        //Timer,
    }
}
