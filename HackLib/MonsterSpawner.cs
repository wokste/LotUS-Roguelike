using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackLib
{
    class MonsterSpawner
    {
        private Game _game;

        MonsterSpawner(Game game)
        {
            _game = game;
        }

        void Spawn()
        {
            _game.Creatures.Add(new Creature
            {
                Name = "Zombie",
                Attack = new AttackComponent
                {
                    Damage = 5,
                    HitChance = 60
                },
                Health = new Bar(5),
                Hunger = new Bar(5),
                Position = _game.GetEmptyLocation(),
            });
        }
    }
}
