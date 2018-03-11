using System;
using HackConsole;

namespace SurvivalHack
{
    public class AttackComponent
    {
        public float HitChance;
        public int Damage;
        public float Range;

        public void Attack(Creature attacker, Creature defender)
        {
            if (Dicebag.UniformInt(100) <= HitChance)
            {
                var damage = Dicebag.Randomize(Damage);
                Message.Write($"{attacker.Name} attacks {defender.Name} and hits for {damage} damage.", attacker.Position, Color.Yellow);
                defender.TakeDamage(damage);
            }
            else
            {
                Message.Write($"{attacker.Name} attacks {defender.Name} but misses.", attacker.Position, Color.Cyan);

            }
        }

        public bool InRange(Creature attacker, Creature defender)
        {
            var delta = attacker.Position - defender.Position;
            return (delta.X * delta.X + delta.Y * delta.Y <= Range * (Range + 1));

            //TODO: Line of sight algorithm
        }
    }
}
