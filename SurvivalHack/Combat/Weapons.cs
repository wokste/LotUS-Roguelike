using HackConsole;
using HackConsole.Algo;
using SurvivalHack.ECM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SurvivalHack.Combat
{
    interface IWeapon : ECM.IComponent
    {
        void Attack(Entity attacker, Entity weapon, Entity defender);
        bool InRange(Entity attacker, Entity defender);
        float WeaponPriority { get; }
    }

    public class MeleeWeapon : IWeapon
    {
        public float Damage;
        public EDamageType DamageType;

        public float WeaponPriority { get; set; }

        public MeleeWeapon(float damage, EDamageType damageType)
        {
            Damage = damage;
            DamageType = damageType;
        }

        public void Attack(Entity attacker, Entity weapon, Entity defender)
        {
            var Attack = new Attack
            {
                Damage = (int)(Damage * (0.5 + Game.Rnd.NextDouble())),
                DamageType = DamageType
            };

            Attack.Fight(attacker, weapon, defender);
        }

        public bool InRange(Entity attacker, Entity defender)
        {
            return (attacker.Move.Pos - defender.Move.Pos).ManhattanLength <= 2;
        }

        public string Describe() => $"Melee attack deals {Damage} damage";
        public IEnumerable<UseFunc> GetActions(EUseMessage filter, EUseSource source) => Enumerable.Empty<UseFunc>();
    }

    public class RangedWeapon : IWeapon
    {
        public float Damage;
        public EDamageType DamageType;
        public float Range;
        public float WeaponPriority { get; set; }

        public RangedWeapon(float damage, EDamageType damageType, float range)
        {
            Damage = damage;
            DamageType = damageType;
            Range = range;
        }

        public void Attack(Entity attacker, Entity weapon, Entity defender)
        {
            var Attack = new Attack
            {
                Damage = (int)(Damage * (0.5 + Game.Rnd.NextDouble())),
                DamageType = DamageType
            };

            Attack.Fight(attacker, weapon, defender);
        }

        public bool InRange(Entity attacker, Entity defender)
        {
            var level = attacker.Move.Level;
            var path = Bresenham.Run(attacker.Move.Pos, defender.Move.Pos);
            foreach (var v in path)
                if (!level.HasFlag(v, TerrainFlag.Sight))
                    return false;

            return true;
        }

        public string Describe() => $"Ranged attack deals {Damage} damage";
        public IEnumerable<UseFunc> GetActions(EUseMessage filter, EUseSource source) => Enumerable.Empty<UseFunc>();
    }
}