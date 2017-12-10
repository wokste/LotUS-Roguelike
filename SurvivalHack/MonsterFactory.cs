using System;
using HackLib;

namespace SurvivalHack
{
    class MonsterFactory
    {
        public Creature Create(string tag)
        {
            switch (tag)
            {
                case "slime":
                    return new Creature
                    {
                        Name = "Slime",
                        Attack = new Attack
                        {
                            Damage = 5,
                            HitChance = 0.5f
                        },
                        Health = new Bar(10)
                    };
                case "orc":
                    return new Creature
                    {
                        Name = "Orc",
                        Attack = new Attack
                        {
                            Damage = 6,
                            HitChance = 0.6f
                        },
                        Health = new Bar(10)
                    };
                default:
                    throw new Exception("WHUT");
            }
        }
    }
}
