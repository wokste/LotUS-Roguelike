﻿using System;

namespace HackLib
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
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"{attacker.Name} attacks {defender.Name} and hits for {damage} damage.");

                defender.TakeDamage(damage);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"{attacker.Name} attacks {defender.Name} but misses.");
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
