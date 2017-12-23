using System;
using System.Drawing;

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
                        HitChance = 60
                    },
                    SourcePos = new Point(2,0),
                    Health = new Bar(8),
                    Position = _game.World.GetEmptyLocation(),
                };
            }
            else if (rnd < 80)
            {
                return new Creature
                {
                    Name = "Zombie Guard",
                    Attack = new AttackComponent
                    {
                        Damage = 6,
                        HitChance = 60
                    },
                    SourcePos = new Point(3, 0),
                    Health = new Bar(16),
                    Position = _game.World.GetEmptyLocation(),
                };
            }
            else
            {
                return new Creature
                {
                    Name = "Skeleton Warrior",
                    Attack = new AttackComponent
                    {
                        Damage = 6,
                        HitChance = 60
                    },
                    SourcePos = new Point(1, 0),
                    Health = new Bar(4),
                    Position = _game.World.GetEmptyLocation(),
                };
            }
        }
    }
}
