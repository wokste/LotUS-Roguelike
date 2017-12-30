using System;

namespace HackLib
{
    public class MonsterSpawner
    {
        private readonly Game _game;

        public MonsterSpawner(Game game)
        {
            _game = game;
        }

        public void Spawn(World map, int count)
        {
            for (var i = 0; i < count; i++)
            {
                var monster = CreateMonster();
                monster.Map = map;
                var ai = new AiController(monster);
                _game.AddCreature(ai);
            }
        }

        private Creature CreateMonster()
        {
            var rnd = Dicebag.UniformInt(100);

            if (rnd < 60)
            {
                return new Creature
                {
                    Name = "Zombie",
                    Attack = new AttackComponent
                    {
                        Damage = 4,
                        HitChance = 60,
                        Range = 1,
                    },
                    Health = new Bar(8),
                    Position = _game.World.GetEmptyLocation(),
                    Speed = 0.6f,
                    Symbol = new Symbol('Z', Color.Red)
                };
            }
            else
            {
                return new Creature
                {
                    Name = "Giant Bat",
                    Attack = new AttackComponent
                    {
                        Damage = 2,
                        HitChance = 60,
                        Range = 1,
                    },
                    Health = new Bar(4),
                    Position = _game.World.GetEmptyLocation(),
                    Speed = 1.5f,
                    Symbol = new Symbol('B', Color.Cyan)
                };
            }
        }
    }
}
