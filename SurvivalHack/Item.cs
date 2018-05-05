using SurvivalHack.ECM;
using System.Collections.Generic;
using System.Diagnostics;
using HackConsole;
using System;

namespace SurvivalHack
{
    public static class ItemTypeList {

        public static Entity Get(string tag)
        {
            switch (tag)
            {
                case "potion1":
                    return new Entity
                    {
                        Name = "Red Potion",
                        Components = new List<IComponent>
                        {
                            new StackComponent(1, 0),
                            new HealComponent
                            {
                                FoodRestore = 5,
                                HealthRestore = 2,
                            },
                        },
                        Symbol = new Symbol('?', Color.Parse("#f30")),
                    };
                case "potion2":
                    return new Entity
                    {
                        Name = "Blue potion",
                        Components = new List<IComponent>
                        {
                            new StackComponent(1, 1),
                            new HealComponent
                            {
                                FoodRestore = 2,
                            },
                        },
                        Symbol = new Symbol('?', Color.Parse("#06f")),
                    };
                case "sword1":
                    return new Entity
                    {
                        Name = "Copper Sword",
                        Components = new List<IComponent>
                        {
                            new AttackComponent
                            {
                                Damage = new Range("4-8"),
                                HitChance = 70,
                            },
                        },
                        
                        Symbol = new Symbol('|', Color.Parse("#b47d6b")),
                    };
                case "sword2":
                    return new Entity
                    {
                        Name = "Iron Sword",
                        Components = new List<IComponent>
                        {
                            new AttackComponent
                            {
                                Damage = new Range("6-10"),
                                HitChance = 70,
                            },
                        },
                        Symbol = new Symbol('|', Color.Parse("#859a9a")),
                    };
                default:
                    throw new ArgumentException("unknown tag " + tag);
            }
        }
    }
}
