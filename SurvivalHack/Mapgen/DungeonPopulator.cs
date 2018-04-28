﻿using HackConsole;
using SurvivalHack.ECM;
using System.Collections.Generic;

namespace SurvivalHack.Mapgen
{
    public class DungeonPopulator
    {
        private readonly Game _game;
        private Dictionary<int, RandomTable<string>> _randomTables = new Dictionary<int, RandomTable<string>>();

        public DungeonPopulator(Game game)
        {
            _game = game;
        }

        public void Spawn(Level level, int count)
        {
            for (var i = 0; i < count; i++)
            {
                var monster = CreateMonster();
                MoveComponent.Bind(monster, level);
                _game.Timeline.Insert(new ActEvent(monster));
            }
        }

        private Entity CreateMonster()
        {
            var rnd = Dicebag.UniformInt(100);

            if (rnd < 60)
            {
                return new Entity
                {
                    Name = "Zombie",
                    Description = "An undead with a nasty attack. Luckily they are easy to outrun.",
                    Attack = new AttackComponent
                    {
                        Damage = 20,
                        HitChance = 60,
                        Range = 1,
                    },
                    Ai = new AiActor(),
                    Attitude = new AiAttitude(),
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
                        Damage = 2,
                        HitChance = 60,
                        Range = 1,
                    },
                    Ai = new AiActor(),
                    Attitude = new AiAttitude(),
                    Health = new Bar(10),
                    Flags = TerrainFlag.Fly,
                    Speed = 1.5f,
                    Symbol = new Symbol('b', Color.Red),
                };
            }
        }
    }
}