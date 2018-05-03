using SurvivalHack.ECM;
using System.Collections.Generic;
using System.Diagnostics;
using HackConsole;
using System;

namespace SurvivalHack
{
    public class StackComponent
    {
        public int Count;
        public int MergeId;

        public StackComponent(int count, int mergeId)
        {
            Count = count;
            MergeId = mergeId;
        }

        private static int MergeIdAutoIncrement;

        int GenMergeId() => MergeIdAutoIncrement++;

        internal bool Consume()
        {
            Count--;
            return (Count == 0);
        }
    }

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
                        Consume = new ConsumableComponent
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
                        Consume = new ConsumableComponent
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
