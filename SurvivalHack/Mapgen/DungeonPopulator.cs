using HackConsole;
using SurvivalHack.ECM;
using System;
using System.Collections.Generic;

namespace SurvivalHack.Mapgen
{
    public class DungeonPopulator
    {
        //private Dictionary<int, RandomTable<string>> _randomTables = new Dictionary<int, RandomTable<string>>();
        private Random _rnd;
        private DungeonGenerator _gen;
        private Level _level;


        public DungeonPopulator(DungeonGenerator gen, Level level, Random rnd)
        {
            _gen = gen;
            _level = level;
            _rnd = rnd;
        }

        public void Spawn(List<Room> rooms, int difficulty)
        {
            var d100 = new Range(1, 100);

            foreach (var room in rooms)
            {
                var roll = d100.Rand(_rnd);

                if (roll <= 50)
                    continue;

                FillRoom(room, difficulty);

                if (roll > 95)
                    FillRoom(room, difficulty);
            }
        }

        private void FillRoom(Room room, int difficulty)
        {
            var d100 = new Range(1, 100);
            PlaceMonsters(room, difficulty);

            if (d100.Rand(_rnd) <= 40)
                PlaceTreasure(room, difficulty);
        }

        private Vec GetFreeTile(Room room)
        {
            Vec v;
            var p0 = room.Transform.Convert(Vec.Zero);
            var p1 = room.Transform.Convert(room.Size - Vec.One);

            do
            {
                v = new Vec(
                    new Range(p0.X, p1.X).Rand(Game.Rnd),
                    new Range(p0.Y, p1.Y).Rand(Game.Rnd));
            } while (!_level.TileMap[v].Flags.HasFlag(TerrainFlag.Walk));

            return v;
        }

        void PlaceMonsters(Room room, int difficulty)
        {
            var monster = CreateMonster();
            var pos = GetFreeTile(room);
            MoveComponent.Bind(monster, _level, pos);
            _gen?.OnNewEvent(new ActEvent(monster));
        }

        private void PlaceTreasure(Room room, int difficulty)
        {
            /*
            var treasure = CreateTreasure();
            var pos = GetFreeTile(level, room, rnd);
            MoveComponent.Bind(treasure, level, pos);
            */
        }

        private Entity CreateMonster()
        {
            var d100 = new Range(1, 100);
            var rnd = d100.Rand(Game.Rnd);

            if (rnd < 60)
            {
                return new Entity
                {
                    Name = "Zombie",
                    Description = "An undead with a nasty attack. Luckily they are easy to outrun.",
                    Attack = new AttackComponent
                    {
                        Damage = new Range("10-14"),
                        HitChance = 60,
                        Range = 1,
                    },
                    Ai = new AiActor(),
                    Attitude = new Attitude(ETeam.Undead, new[] { new TeamAttitudeRule(ETargetAction.Hate, ETeam.Player) }),
                    Health = new Bar(40),
                    Flags = TerrainFlag.Walk,
                    Speed = 0.6f,
                    Symbol = new Symbol('z', Color.Red)
                };
            }
            else
            {
                return new Entity
                {
                    Name = "Giant Bat",
                    Description = "A flying monster that is a nuisance to any adventurer.",
                    Attack = new AttackComponent
                    {
                        Damage = new Range("1-3"),
                        HitChance = 60,
                        Range = 1,
                    },
                    Ai = new AiActor(),
                    Attitude = new Attitude(ETeam.None, new [] { new TeamAttitudeRule(ETargetAction.Hate, ETeam.Player) }),
                    Health = new Bar(10),
                    Flags = TerrainFlag.Fly,
                    Speed = 1.5f,
                    Symbol = new Symbol('b', Color.Red),
                };
            }
        }
    }
}
