using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SurvivalHack.ECM;

namespace SurvivalHack.Combat
{
    interface IDamageMutator : ECM.IComponent
    {
        int Priority { get; }
        bool Mutate(Attack attack);
    }

    class Blockable : IDamageMutator
    {
        public int Priority { get; set; } = 100;

        public float BlockChance = 0.3f;

        public bool Mutate(Attack attack)
        {
            // TODO: Parrying should not be possible against ranged weapons.

            if (Game.Rnd.NextDouble() > BlockChance)
                return false;

            attack.Damage = 0;

            // TODO: I need to send information on how it is blocked to the log system

            return true;
        }

        public string Describe()
        {
            return $"Has a {BlockChance:%} to block incoming attacks";
        }

        public IEnumerable<UseFunc> GetActions(EUseMessage filter, EUseSource source) => Enumerable.Empty<UseFunc>();
    }

    class Armour : IDamageMutator
    {
        public int Priority { get; set; } = 50;

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

        public IEnumerable<UseFunc> GetActions(EUseMessage filter, EUseSource source) => Enumerable.Empty<UseFunc>();
    }
}
