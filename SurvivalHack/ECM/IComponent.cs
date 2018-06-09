using System;
using System.Collections.Generic;

namespace SurvivalHack.ECM
{
    public interface IComponent
    {
        string Describe();

        //bool Use(Entity user, Entity item, Entity target, EUseMessage filter);

        IEnumerable<UseFunc> GetActions(EUseMessage filter, EUseSource source);
    }

    public struct UseFunc {
        public EUseOrder Order;
        public Action<Entity, Entity, Entity> Action;

        public UseFunc(Action<Entity, Entity, Entity> action, EUseOrder order = EUseOrder.Event)
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
        This,
        Target,
        Timer,
    }

    public enum EUseMessage
    {
        Drink,       // Mainly for potions
        Cast,        // Spells, scrolls, etc.
        Threaten,    // Do an attack.
        Attack,      // Being attacked obviously.
        Damage,      // Successfully hit with an attack

        //Attack,      // Hitting someone. Vampiric daggers etc.
        //BumpAttack,  // Used in bump attacks etc.
        //Kill,      // Killing someone. E.g. 
        //Blocked,   // Item blocks an attack. Shields etc.
        //Damaged,   // Entity is damaged.
        //Destroyed, // Entity is destroyed. Spawning smaller slimes when a slime dies
    }
}
