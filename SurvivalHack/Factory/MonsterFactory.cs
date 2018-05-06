using HackConsole;
using SurvivalHack.Ai;
using SurvivalHack.ECM;
using System.Collections.Generic;

namespace SurvivalHack.Factory
{
    class MonsterFactory : IEntityFactory
    {
        public Entity Gen(EntityGenerationInfo info)
        {
            var d100 = new Range(1, 100);
            var rnd = d100.Rand(Game.Rnd);

            if (rnd < 60)
            {
                return new Entity('z', "Zombie", EEntityFlag.Blocking | EEntityFlag.TeamMonster)
                {
                    Description = "An undead with a nasty attack. Luckily they are easy to outrun.",
                    Components = new List<IComponent>()
                    {
                        new AttackComponent(9, EDamageType.Bludgeoing),
                    },
                    Ai = new AiActor(),
                    Attitude = new Attitude(ETeam.Undead, new[] { new TeamAttitudeRule(ETargetAction.Hate, ETeam.Player) }),
                    Health = new Bar(9),
                    Flags = TerrainFlag.Walk,
                    Speed = 0.6f,
                    Symbol = new Symbol('z', Color.Red)
                };
            }
            else
            {
                return new Entity('b', "Giant Bat", EEntityFlag.Blocking | EEntityFlag.TeamMonster)
                {
                    Description = "A flying monster that is a nuisance to any adventurer.",
                    Components = new List<IComponent>()
                    {
                        new AttackComponent(3, EDamageType.Piercing),
                    },
                    Ai = new AiActor(),
                    Attitude = new Attitude(ETeam.None, new[] { new TeamAttitudeRule(ETargetAction.Hate, ETeam.Player) }),
                    Health = new Bar(3),
                    Flags = TerrainFlag.Fly,
                    Speed = 1.5f,
                    Symbol = new Symbol('b', Color.Red),
                };
            }
        }
    }
}
