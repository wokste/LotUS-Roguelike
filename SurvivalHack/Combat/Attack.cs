using HackConsole;
using SurvivalHack.ECM;
using System;
using System.Diagnostics;
using System.Linq;

namespace SurvivalHack.Combat
{
    public class Attack
    {
        public int Damage;
        public EDamageType DamageType;

        public EDamageLocation DamageLoc;
        public Entity Attacker;
        public Entity Defender;

        public void Fight(Entity attacker, Entity weapon, Entity defender)
        {
            Attacker = attacker;
            Defender = defender;
            DamageLoc = Game.Rnd.NextDouble() < 0.33 ? EDamageLocation.Head : EDamageLocation.Body;

            attacker.Event(weapon, defender, ECM.EUseMessage.Attack, new[] {
                new UseFunc(ToHitRoll)
            });
        }

        private void ToHitRoll(Entity attacker, Entity weapon, Entity defender)
        {
            // Collect all items that mutate damage, sorted by priority (highest to lowest)
            var defenderItems = defender.ListSubEntities().SelectMany(e => e.Get<IDamageMutator>().Select(m => (Entity: e, Mututor: m)));
            defenderItems.OrderBy(p => -p.Mututor.Priority);

            foreach (var pair in defenderItems)
            {
                pair.Mututor.Mutate(this);
                if (Damage <= 0)
                    return;
            }

            Attacker.Event(weapon, defender, EUseMessage.Damage, new[] {
                new UseFunc(DamageTarget)
            });
        }


        private void DamageTarget(Entity attacker, Entity weapon, Entity defender)
        {
            var damagable = defender.GetOne<Damagable>();
            Message.Write($"{attacker.Name} attacks {defender.Name} for {Damage} damage.", attacker?.Move?.Pos, Color.Orange);
            damagable.TakeDamage(defender, this);
        }
    }

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
        LArm,
        RArm,
        Legs,
        Wings,
        Tail,
    }
}
