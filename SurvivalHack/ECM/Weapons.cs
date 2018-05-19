using HackConsole;
using System;

namespace SurvivalHack.ECM
{
    public class Attack {
        public int Damage;
        public EDamageType DamageType;

        public void Fight(Entity attacker, Entity weapon, Entity defender)
        {
            // TODO: Choose attack location

            // TODO: parrying
            // TODO: Blocking
            // TODO: Armour

            Message.Write($"{attacker.Name} attacks {defender.Name} for {Damage} damage.", attacker?.Move?.Pos, Color.Yellow);
            defender.TakeDamage(Damage, DamageType);
            
            // TODO: Post-Effects (poison, hooking, petrification, etc)

            // TODO: Post-attack logging
        }
    }

    interface IWeapon : IComponent
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
    }
}
namespace SurvivalHack
{
    [Flags]
    public enum EDamageType
    {
        Bludgeoing = 1,
        Piercing = 2,
        Slashing = 4,

        Fire = 0x10,
        Ice = 0x20,
        Thunder = 0x20,
    }

    public enum EAttackMove
    {
        Projectile = 1,
        Piercing = 2,
        Slashing = 4,

        Fire = 0x10,
        Ice = 0x20,
        Thunder = 0x20,
    }

    public enum EDamageLocation
    {
        Head,
        Body,
        Arms,
        Legs,
    }
}