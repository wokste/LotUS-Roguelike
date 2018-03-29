using Symbol = HackConsole.Symbol;
using Color = HackConsole.Color;
using SurvivalHack.ECM;

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
                monster.Move.AddToMap(map, monster);

                //_game.AddCreature(monster);
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
                    Description = "An undead with a nasty attack. Luckily they are easy to outrun.",
                    Attack = new AttackComponent
                    {
                        Damage = 20,
                        HitChance = 60,
                        Range = 1,
                    },
                    Ai = new AiController(),
                    Health = new Bar(40),
                    Move = _game.World.GetEmptyLocation(),
                    Speed = 0.6f,
                    Symbol = new Symbol('z', Color.Red)
                };
            }
            else if (rnd < 98)
            {
                return new Monster
                {
                    Name = "Giant Bat",
                    Description = "A flying monster that is a nuisance to any adventurer.",
                    Attack = new AttackComponent
                    {
                        Damage = 2,
                        HitChance = 60,
                        Range = 1,
                    },
                    Ai = new AiController(),
                    Health = new Bar(10),
                    Move = _game.World.GetEmptyLocation(TerrainFlag.Fly),
                    Speed = 1.5f,
                    Symbol = new Symbol('b', Color.Red),
                };
            }
            else
            {
                return new Monster
                {
                    Name = "Giant Fish",
                    Description = "A swimming monster that can't reach you on land.",
                    Attack = new AttackComponent
                    {
                        Damage = 8,
                        HitChance = 60,
                        Range = 1,
                    },
                    Ai = new AiController(),
                    Health = new Bar(20),
                    Move = _game.World.GetEmptyLocation(TerrainFlag.Swim),
                    Speed = 1f,
                    Symbol = new Symbol('f', Color.Red),
                };
            }
        }
    }
}
