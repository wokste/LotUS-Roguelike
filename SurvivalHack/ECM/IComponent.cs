using SurvivalHack.Combat;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SurvivalHack.ECM
{
    public class UseMessage
    {
        public Entity Self;
        public Entity Item;
        public Entity Target;
    }

    public class DrinkMessage : UseMessage
    {
        public DrinkMessage(Entity self, Entity item)
        {
            Self = self;
            Item = item;
        }
    }

    public class CastMessage : UseMessage
    {
        public CastMessage(Entity self, Entity item)
        {
            Self = self;
            Item = item;
        }
    }

    public class ThreatenMessage : UseMessage
    {
        public ThreatenMessage(Entity self, Entity target)
        {
            Self = self;
            Target = target;
        }
    }

    public class AttackMessage : UseMessage
    {
        internal string State = "hit";

        public AttackMessage(Entity self, Entity weapon, Entity target)
        {
            Self = self;
            Item = weapon;
            Target = target;
        }
    }

    public class DamageMessage : UseMessage
    {
        public int Damage;
        public EDamageType DamageType;
        public bool Significant => (Damage > 0);

        public DamageMessage(UseMessage prevMessage, int damage, EDamageType damageType)
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
        IEnumerable<UseFunc> GetActions(UseMessage message, EUseSource source);
    }

    public struct UseFunc {
        public EUseOrder Order;
        public Action<UseMessage> Action;

        public UseFunc(Action<UseMessage> action, EUseOrder order = EUseOrder.Event)
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
