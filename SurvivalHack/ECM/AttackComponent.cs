using HackConsole;
using System;

namespace SurvivalHack.ECM
{
    public interface IAttackComponent : IComponent
    {
        void Attack(Entity attacker, Entity defender);
        bool InRange(Entity attacker, Entity defender);
    }

    public class AttackComponent : IAttackComponent
    {
        public float HitChance = 0.7f;
        public float Damage;
        public float Range = 1;
        public EDamageType DamageType;

        public AttackComponent(float damage, EDamageType damageType)
        {
            Damage = damage;
            DamageType = damageType;
        }

        public void Attack(Entity attacker, Entity defender)
        {
            if (Game.Rnd.NextDouble() <= HitChance)
            {
                var damage = (int)(Damage * (0.5 + Game.Rnd.NextDouble()));
                Message.Write($"{attacker.Name} attacks {defender.Name} and hits for {damage} damage.", attacker?.Move?.Pos, Color.Yellow);
                defender.TakeDamage(damage, DamageType);
            }
            else
            {
                Message.Write($"{attacker.Name} attacks {defender.Name} but misses.", attacker?.Move?.Pos, Color.Cyan);
            }
        }

        public bool InRange(Entity attacker, Entity defender)
        {
            var delta = attacker.Move.Pos - defender.Move.Pos;
            return (delta.X * delta.X + delta.Y * delta.Y <= Range * (Range + 1));

            //TODO: Line of sight algorithm
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
}