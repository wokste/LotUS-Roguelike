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
                case "pumpkin":
                    return new Entity
                    {
                        Name = "Pumpkin",
                        StackComponent = new StackComponent(1, 0),
                        Consume = new ConsumableComponent
                        {
                            FoodRestore = 5,
                            HealthRestore = 2,
                            Quality = 7
                        }
                    };
                case "mushroom":
                    return new Entity
                    {
                        Name = "Mushroom",
                        StackComponent = new StackComponent(1, 1),
                        Consume = new ConsumableComponent
                        {
                            FoodRestore = 2,
                            Quality = 3
                        }
                    };
                case "sword1":
                    return new Entity
                    {
                        Name = "Wooden Sword",
                        Attack = new AttackComponent
                        {
                            Damage = new Range("4-8"),
                            HitChance = 70,
                        }
                    };
                case "sword2":
                    return new Entity
                    {
                        Name = "Iron Sword",
                        Attack = new AttackComponent
                        {
                            Damage = new Range("6-10"),
                            HitChance = 70,
                        }
                    };
                default:
                    throw new ArgumentException("unknown tag " + tag);
            }
        }
    }
    /*
    public class Item : Entity
    {
        public int Count;

        public override string ToString()
        {
            return Type.Stacking ? $"{Type.Name} ({Count})" : $"{Type.Name}";
        }
    }
    */
}
