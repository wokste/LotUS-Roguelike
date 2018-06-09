using HackConsole;
using System;
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
            // TODO: Choose attack location
            var damagable = defender.GetOne<Damagable>();
            //DamageLoc = damagable.GenerateLoc();
            DamageLoc = Game.Rnd.NextDouble() < 0.33 ? EDamageLocation.Head : EDamageLocation.Body;
            Attacker = attacker;
            Defender = defender;

            // Collect all items that mutate damage, sorted by priority (highest to lowest)
            var defenderItems = defender.ListSubEntities().SelectMany(e => e.Get<IDamageMutator>().Select(m => (Entity: e, Mututor: m)));
            defenderItems.OrderBy(p => -p.Mututor.Priority);

            foreach (var pair in defenderItems)
            {
                pair.Mututor.Mutate(this);
                if (Damage <= 0)
                    return;
            }

            Message.Write($"{attacker.Name} attacks {defender.Name} for {Damage} damage.", attacker?.Move?.Pos, Color.Orange);
            damagable.TakeDamage(defender, this);

            // TODO: Post-Effects (poison, hooking, petrification, etc)

            // TODO: Post-attack logging
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
