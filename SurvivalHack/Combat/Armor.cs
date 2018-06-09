using System;
using System.Collections.Generic;
using System.Linq;
using SurvivalHack.ECM;
using HackConsole;

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
            if (Game.Rnd.NextDouble() > BlockChance)
                return false;

            attack.Damage = 0;

            // TODO: I need to send information on how it is blocked to the log system

            Message.Write($"{attack.Defender} blocks {attack.Attacker}s attack", attack.Defender.Move.Pos, Color.Cyan);

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

        public Armour(int damageReduction, float critChance)
        {
            DamageReduction = damageReduction;
            CritChance = critChance;
        }

        public bool Mutate(Attack attack)
        {
            // TODO: Armour should protect only a part of the body.

            if (Game.Rnd.NextDouble() < CritChance)
            {
                // TODO: Crit messages.
                return false;
            }

            var newDamage = Math.Max(attack.Damage - DamageReduction, 0);
            Message.Write($"{attack.Defender}'s armour reduces the damage of the incoming attack from {attack.Damage} to {newDamage}", attack.Defender.Move.Pos, Color.Cyan);
            attack.Damage = newDamage;

            return true;
        }

        public string Describe()
        {
            return $"Reduces damage by {DamageReduction}.";
        }

        public IEnumerable<UseFunc> GetActions(EUseMessage filter, EUseSource source) => Enumerable.Empty<UseFunc>();
    }
}
