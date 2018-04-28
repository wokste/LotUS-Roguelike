﻿using HackConsole;

namespace SurvivalHack.ECM
{
    public class AttackComponent
    {
        public float HitChance;
        public Range Damage;
        public float Range;

        public void Attack(Entity attacker, Entity defender)
        {
            var d100 = new Range(1,100);

            if (d100.Rand(Game.Rnd) <= HitChance)
            {
                var damage = Damage.Rand(Game.Rnd);
                Message.Write($"{attacker.Name} attacks {defender.Name} and hits for {damage} damage.", attacker?.Move?.Pos, Color.Yellow);
                defender.TakeDamage(damage);
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
