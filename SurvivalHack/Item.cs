using SurvivalHack.ECM;
using System.Collections.Generic;
using System.Diagnostics;
using HackConsole;

namespace SurvivalHack
{
    public class ItemType : Entity
    {
        public bool Stacking;
        public ConsumableComponent OnEat;

        public override string ToString()
        {
            return Name;
        }

        public Item Make(int count)
        {
            return new Item
            {
                Count = count,
                Type = this
            };
        }
    }

    public static class ItemTypeList {
        private static readonly Dictionary<string, ItemType> Types = new Dictionary<string, ItemType>();

        public static void InitTypes()
        {
            Debug.Assert(Types.Count == 0);

            Types.Add("pumpkin", new ItemType
            {
                Name = "Pumpkin",
                Stacking = true,
                OnEat = new ConsumableComponent
                {
                    FoodRestore = 5,
                    HealthRestore = 2,
                    Quality = 7
                }
            });

            Types.Add("mushroom", new ItemType
            {
                Name = "Mushroom",
                Stacking = true,
                OnEat = new ConsumableComponent
                {
                    FoodRestore = 2,
                    Quality = 3
                }
            });

            Types.Add("sword1", new ItemType
            {
                Name = "Wooden Sword",
                Stacking = false,
                Attack = new AttackComponent
                {
                    Damage = new Range("4-8"),
                    HitChance = 70,
                }
            });

            Types.Add("sword2", new ItemType
            {
                Name = "Iron Sword",
                Stacking = false,
                Attack = new AttackComponent
                {
                    Damage = new Range("6-10"),
                    HitChance = 70,
                }
            });
        }

        public static ItemType Get(string tag)
        {
            var itemType = Types[tag];
            Debug.Assert(itemType != null);
            return itemType;
        }
    }

    public class Item : Entity
    {
        public ItemType Type;
        public int Count;

        public override string ToString()
        {
            return Type.Stacking ? $"{Type.Name} ({Count})" : $"{Type.Name}";
        }
    }
}
