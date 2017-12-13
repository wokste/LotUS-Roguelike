using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackLib
{
    public class AttackComponent
    {
        public float HitChance;
        public int Damage;

        public void Attack(Creature attacker, Creature defender)
        {
            if (Dicebag.UniformInt(100) >= HitChance)
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
    }
}
