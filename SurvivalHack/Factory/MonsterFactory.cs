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
                    Components = new List<IComponent>()
                    {
                        new Combat.MeleeWeapon(9, Combat.EAttackMove.Swing, Combat.EDamageType.Bludgeoing),
                        new Combat.Damagable(6),
                    },
                    Ai = new AiActor(),
                    Attitude = new Attitude(ETeam.Undead, new[] { new TeamAttitudeRule(ETargetAction.Hate, ETeam.Player) }),
                    Flags = TerrainFlag.Walk,
                    Speed = 0.6f,
                    Symbol = new Symbol('z', Color.Red)
                };
            }
            else
            {
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
                    Symbol = new Symbol('b', Color.Red),
                };
            }
        }
    }
}
