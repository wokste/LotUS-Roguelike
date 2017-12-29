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
                    SourcePos = new Vec(0,1),
                    Health = new Bar(8),
                    Position = _game.World.GetEmptyLocation(),
                    Speed = 60,
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
                    SourcePos = new Vec(2, 1),
                    Health = new Bar(4),
                    Position = _game.World.GetEmptyLocation(),
                    Speed = 150,
                };
            }
        }
    }
}
