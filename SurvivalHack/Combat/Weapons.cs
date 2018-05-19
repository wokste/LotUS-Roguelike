using HackConsole;
using System;

namespace SurvivalHack.Combat
{
    interface IWeapon : ECM.IComponent
    {
        void Attack(Entity attacker, Entity weapon, Entity defender);
        bool InRange(Entity attacker, Entity defender);
    }

    public class MeleeWeapon : IWeapon
    {
        public float Damage;
        public EDamageType DamageType;

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
        public bool Use(Entity user, Entity item, Entity target, ECM.EUseMessage filter) => false;
    }

    public class RangedWeapon : IWeapon
    {
        public float Damage;
        public EDamageType DamageType;
        public float Range;

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
            return true;
            throw new NotImplementedException();
        }

        public string Describe() => $"Ranged attack deals {Damage} damage";
        public bool Use(Entity user, Entity item, Entity target, ECM.EUseMessage filter) => false;
    }
}