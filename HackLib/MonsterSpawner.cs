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

        public void Spawn(int count)
        {
            for (var i = 0; i < count; i++)
                _game.Creatures.Add(CreateMonster());
        }

        private Creature CreateMonster()
        {
            int rnd = Dicebag.UniformInt(100);

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
                    Position = _game.GetEmptyLocation(),
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
                    Position = _game.GetEmptyLocation(),
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
                    Position = _game.GetEmptyLocation(),
                };
            }
        }
    }
}
