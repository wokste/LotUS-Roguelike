using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalHack
{
    class MonsterFactory
    {
        internal Creature Create(string tag)
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
                        HP = new Bar(10)
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
                        HP = new Bar(10)
                    };
                default:
                    throw new Exception("WHUT");
            }
        }
    }
}
