using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalHack
{
    class Program
    {
        internal static Random Rnd = new Random();

        static void Main(string[] args)
        {
            var player = new Creature
            {
                Name = "Steven",
                Attack = new Attack
                {
                    Damage = 7,
                    HitChance = 0.75f
                },
                HP = new Bar(25)
            };

            var game = new Game(player);
            game.Run();
        }
    }
}
