using Symbol = HackConsole.Symbol;
using Color = HackConsole.Color;

namespace SurvivalHack
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
                _game.AddCreature(monster);
            }
        }

        private Monster CreateMonster()
        {
            var rnd = Dicebag.UniformInt(100);

            if (rnd < 60)
            {
                return new Monster
                {
                    Name = "Zombie",
                    Attack = new AttackComponent
                    {
                        Damage = 20,
                        HitChance = 60,
                        Range = 1,
                    },
                    Health = new Bar(40),
                    Position = _game.World.GetEmptyLocation(),
                    Speed = 0.6f,
                    Symbol = new Symbol('z', Color.Red)
                };
            }
            else if (rnd < 98)
            {
                return new Monster
                {
                    Name = "Giant Bat",
                    Attack = new AttackComponent
                    {
                        Damage = 2,
                        HitChance = 60,
                        Range = 1,
                    },
                    Health = new Bar(4),
                    Position = _game.World.GetEmptyLocation(TerrainFlag.Fly),
                    Speed = 1.5f,
                    Symbol = new Symbol('b', Color.Red),
                    MovementType = TerrainFlag.Fly
                };
            }
            else
            {
                return new Monster
                {
                    Name = "Giant Fish",
                    Attack = new AttackComponent
                    {
                        Damage = 8,
                        HitChance = 60,
                        Range = 1,
                    },
                    Health = new Bar(8),
                    Position = _game.World.GetEmptyLocation(TerrainFlag.Swim),
                    Speed = 1f,
                    Symbol = new Symbol('f', Color.Red),
                    MovementType = TerrainFlag.Swim
                };
            }
        }
    }
}
