using HackConsole;
using SurvivalHack.Ai;
using SurvivalHack.Combat;
using SurvivalHack.ECM;
using System;
using System.Collections.Generic;

namespace SurvivalHack.Factory
{
    public class MonsterFactory : IEntityFactory
    {
        readonly RandomTable<string> BasePropabilities;

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
                    return new Entity(new TileGlyph(4, 20, GlyphMethod.Anim), "Zombie", EEntityFlag.Blocks | EEntityFlag.TeamMonster)
                    {
                        Components = new ComponentList{
                            new MeleeWeapon(new Damage(9,EDamageType.Blunt)),
                            new StatBlock(20,0,1)
                        },
                        Ai = new AiActor(),
                        Attitude = new Attitude(ETeam.Undead, new[] { new TeamAttitudeRule(ETargetAction.Hate, ETeam.Player) }),
                        Speed = 0.6f,
                    };
                case "bat":
                    return new Entity(new TileGlyph(10, 23, GlyphMethod.Anim), "Giant Bat", EEntityFlag.Blocks | EEntityFlag.TeamMonster)
                    {
                        Components = new ComponentList{
                            new MeleeWeapon(new Damage(3,EDamageType.Pierce)),
                            new StatBlock(8,0,1)
                        },
                        Ai = new AiActor(),
                        Attitude = new Attitude(ETeam.None, new[] { new TeamAttitudeRule(ETargetAction.Hate, ETeam.Player) }),
                        Speed = 1.5f,
                    };
                case "skeleton":
                    return new Entity(new TileGlyph(0, 20, GlyphMethod.Anim), "Skeleton", EEntityFlag.Blocks | EEntityFlag.TeamMonster)
                    {
                        Components = new ComponentList{
                            new RangedWeapon(new Damage(9, EDamageType.Pierce), 50),
                            new StatBlock(12,0,1),
                        },
                        Ai = new AiActor(),
                        Attitude = new Attitude(ETeam.Undead, new[] { new TeamAttitudeRule(ETargetAction.Hate, ETeam.Player) }),
                        Speed = 0.6f,
                    };
                case "firebat":
                    return new Entity(new TileGlyph(12, 23, GlyphMethod.Anim), "Hell Bat", EEntityFlag.Blocks | EEntityFlag.TeamMonster)
                    {
                        Components = new ComponentList{
                            new MeleeWeapon(new Damage(6, EDamageType.Pierce | EDamageType.Fire)),
                            new StatBlock(16,8,1),
                        },
                        Ai = new AiActor(),
                        Attitude = new Attitude(ETeam.None, new[] { new TeamAttitudeRule(ETargetAction.Hate, ETeam.Player) }),
                        Speed = 1.5f,
                    };
            }
            throw new ArgumentException($"Tag {tag} not found");
        }
    }
}
