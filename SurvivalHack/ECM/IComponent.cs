using System.Collections.Generic;

namespace SurvivalHack.ECM
{
    public interface IComponent
    {
        bool Use(Entity user, Entity item, Entity target, EUseMessage filter);
        string Describe();
    }

    public enum EUseMessage
    {
        Drink,       // Mainly for potions
        Cast,        // Spells, scrolls, etc.
        //Attack,      // Hitting someone. Vampiric daggers etc.
        //BumpAttack,  // Used in bump attacks etc.
        //Kill,      // Killing someone. E.g. 
        //Blocked,   // Item blocks an attack. Shields etc.
        //Damaged,   // Entity is damaged.
        //Destroyed, // Entity is destroyed. Spawning smaller slimes when a slime dies
    }
}
