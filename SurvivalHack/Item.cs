using SurvivalHack.ECM;
using System.Collections.Generic;
using System.Diagnostics;
using HackConsole;
using System;
using SurvivalHack.ECM;

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
                        StackComponent = new StackComponent(1, 0),
                        Consume = new HealComponent
                        {
                            FoodRestore = 5,
                            HealthRestore = 2,
                            Quality = 7
                        },
                        Symbol = new Symbol('?', Color.Parse("#f30")),
                    };
                case "potion2":
                    return new Entity
                    {
                        Name = "Blue potion",
                        StackComponent = new StackComponent(1, 1),
                        Consume = new HealComponent
                        {
                            FoodRestore = 2,
                            Quality = 3
                        },
                        Symbol = new Symbol('?', Color.Parse("#06f")),
                    };
                case "sword1":
                    return new Entity
                    {
                        Name = "Copper Sword",
                        Attack = new AttackComponent
                        {
                            Damage = new Range("4-8"),
                            HitChance = 70,
                        },
                        Symbol = new Symbol('|', Color.Parse("#b47d6b")),
                    };
                case "sword2":
                    return new Entity
                    {
                        Name = "Iron Sword",
                        Attack = new AttackComponent
                        {
                            Damage = new Range("6-10"),
                            HitChance = 70,
                        },
                        Symbol = new Symbol('|', Color.Parse("#859a9a")),
                    };
                default:
                    throw new ArgumentException("unknown tag " + tag);
            }
        }
    }
}
