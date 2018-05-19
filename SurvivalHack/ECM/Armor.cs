using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalHack.ECM
{
    interface IDamageMutator : IComponent
    {
        bool Mutate(Attack attack);
    }

    class Blockable : IDamageMutator
    {
        public float BlockChance = 0.3f;

        public bool Mutate(Attack attack)
        {
            // TODO: Parrying should not be possible against ranged weapons.

            if (Game.Rnd.NextDouble() > BlockChance)
                return false;

            attack.Damage = 0;

            // TODO: I need to send 

            return true;
        }

        public string Describe()
        {
            return $"Has a {BlockChance:%} to block incoming attacks";
        }

        public bool Use(Entity user, Entity item, Entity target, EUseMessage filter) => false;
    }

    class Armour : IDamageMutator
    {
        public float CritChance = 0.02f;
        public int DamageReduction = 1;

        public bool Mutate(Attack attack)
        {
            // TODO: Armour should protect only a part of the body.

            if (Game.Rnd.NextDouble() < CritChance)
            {
                // TODO: Crit messages.
                return false;
            }

            attack.Damage -= DamageReduction;

            return true;
        }

        public string Describe()
        {
            return $"Reduces damage by {DamageReduction}.";
        }

        public bool Use(Entity user, Entity item, Entity target, EUseMessage filter) => false;
    }
}
