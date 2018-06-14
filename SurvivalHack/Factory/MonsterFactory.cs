﻿using HackConsole;
using SurvivalHack.Ai;
using SurvivalHack.ECM;
using System;
using System.Collections.Generic;

namespace SurvivalHack.Factory
{
    class MonsterFactory : IEntityFactory
    {
        RandomTable<string> BasePropabilities;

        public MonsterFactory()
        {
            BasePropabilities = RandomTable<string>.FromString("bat:2,zombie:3,firebat:1,skeleton:1");
        }

        public Entity Gen(EntityGenerationInfo info)
        {
            var tag = BasePropabilities.GetRand(info.Rnd);

            var e = GetBasic(tag);

            return e;
        }

        public Entity GetBasic(string tag)
        {
            switch (tag)
            {
                case "zombie":
                    return new Entity('z', "Zombie", EEntityFlag.Blocking | EEntityFlag.TeamMonster)
                    {
                        Components = new List<IComponent>()
                        {
                            new Combat.MeleeWeapon(9, Combat.EAttackMove.Swing, Combat.EDamageType.Bludgeoing),
                            new Combat.Damagable(6),
                        },
                        Ai = new AiActor(),
                        Attitude = new Attitude(ETeam.Undead, new[] { new TeamAttitudeRule(ETargetAction.Hate, ETeam.Player) }),
                        Flags = TerrainFlag.Walk,
                        Speed = 0.6f,
                        Symbol = new Symbol('z', Color.White)
                    };
                case "bat":
                    return new Entity('b', "Giant Bat", EEntityFlag.Blocking | EEntityFlag.TeamMonster)
                    {
                        Components = new List<IComponent>()
                        {
                            new Combat.MeleeWeapon(3, Combat.EAttackMove.Close, Combat.EDamageType.Piercing),
                            new Combat.Damagable(3),
                        },
                        Ai = new AiActor(),
                        Attitude = new Attitude(ETeam.None, new[] { new TeamAttitudeRule(ETargetAction.Hate, ETeam.Player) }),
                        Flags = TerrainFlag.Fly,
                        Speed = 1.5f,
                        Symbol = new Symbol('b', Color.White),
                    };
                case "skeleton":
                    return new Entity('z', "Skeleton", EEntityFlag.Blocking | EEntityFlag.TeamMonster)
                    {
                        Components = new List<IComponent>()
                        {
                            new Combat.RangedWeapon(9, Combat.EDamageType.Piercing, 50),
                            new Combat.Damagable(6),
                        },
                        Ai = new AiActor(),
                        Attitude = new Attitude(ETeam.Undead, new[] { new TeamAttitudeRule(ETargetAction.Hate, ETeam.Player) }),
                        Flags = TerrainFlag.Walk,
                        Speed = 0.6f,
                        Symbol = new Symbol('s', Color.White)
                    };
                case "firebat":
                    return new Entity('b', "Fire Bat", EEntityFlag.Blocking | EEntityFlag.TeamMonster)
                    {
                        Components = new List<IComponent>()
                        {
                            new Combat.MeleeWeapon(6, Combat.EAttackMove.Close, Combat.EDamageType.Piercing | Combat.EDamageType.Fire),
                            new Combat.Damagable(3),
                        },
                        Ai = new AiActor(),
                        Attitude = new Attitude(ETeam.None, new[] { new TeamAttitudeRule(ETargetAction.Hate, ETeam.Player) }),
                        Flags = TerrainFlag.Fly,
                        Speed = 1.5f,
                        Symbol = new Symbol('b', Color.Red),
                    };
            }
            throw new ArgumentException($"Tag {tag} not found");
        }
    }
}
