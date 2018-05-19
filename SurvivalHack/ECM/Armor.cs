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
        float BlockChance;
        int DamageReduction;

        public bool Mutate(Attack attack)
        {
            // TODO: Parrying should not be possible against ranged weapons.

            if (Game.Rnd.NextDouble() > BlockChance)
                return false;

            attack.Damage -= DamageReduction;

            // TODO: I need to send 

            return true;
        }
    }

    class Armour : IDamageMutator
    {
        float CritChance;
        int DamageReduction;

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
    }
}
