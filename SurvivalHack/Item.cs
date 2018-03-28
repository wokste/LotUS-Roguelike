using SurvivalHack.ECM;
using System.Collections.Generic;
using System.Diagnostics;

namespace SurvivalHack
{
    public class ItemType
    {
        public string Tag;
        public string Name;
        public bool Stacking;
        public ConsumableComponent OnEat;
        public AttackComponent OnMelee;

        public override string ToString()
        {
            return Name;
        }

        public Item Make(int min = 1, int max = -1)
        {
            if (max == -1)
                max = min;

            return new Item
            {
                Count = Dicebag.UniformInt(min,max + 1),
                Type = this
            };
        }
    }

    public static class ItemTypeList {
        private static readonly List<ItemType> Types = new List<ItemType>();

        public static void InitTypes()
        {
            Debug.Assert(Types.Count == 0);

            Types.Add(new ItemType
            {
                Tag = "wood",
                Name = "Wood",
                Stacking = true
            });

            Types.Add(new ItemType
            {
                Tag = "stone",
                Name = "Stone",
                Stacking = true
            });

            Types.Add(new ItemType
            {
                Tag = "ore",
                Name = "Iron Ore",
                Stacking = true
            });
            
            Types.Add(new ItemType
            {
                Tag = "pumpkin",
                Name = "Pumpkin",
                Stacking = true,
                OnEat = new ConsumableComponent
                {
                    FoodRestore = 5,
                    HealthRestore = 2,
                    Quality = 7
                }
            });


            Types.Add(new ItemType
            {
                Tag = "mushroom",
                Name = "Mushroom",
                Stacking = true,
                OnEat = new ConsumableComponent
                {
                    FoodRestore = 2,
                    Quality = 3
                }
            });

            Types.Add(new ItemType
            {
                Tag = "axe1",
                Name = "Stone Axe",
                Stacking = false

            });

            Types.Add(new ItemType
            {
                Tag = "axe2",
                Name = "Iron Axe",
                Stacking = false
            });

            Types.Add(new ItemType
            {
                Tag = "pick1",
                Name = "Stone Pickaxe",
                Stacking = false
            });

            Types.Add(new ItemType
            {
                Tag = "pick2",
                Name = "Iron Pickaxe",
                Stacking = false
            });

            Types.Add(new ItemType
            {
                Tag = "sword1",
                Name = "Wooden Sword",
                Stacking = false,
                OnMelee = new AttackComponent
                {
                    Damage = 6,
                    HitChance = 70,
                }
            });

            Types.Add(new ItemType
            {
                Tag = "sword2",
                Name = "Iron Sword",
                Stacking = false,
                OnMelee = new AttackComponent
                {
                    Damage = 8,
                    HitChance = 70,
                }
            });
        }

        public static ItemType Get(string tag)
        {
            var itemType = Types.Find(t => t.Tag == tag);
            Debug.Assert(itemType != null);
            return itemType;
        }
    }

    public class Item
    {
        public ItemType Type;
        public int Count;

        public override string ToString()
        {
            return Type.Stacking ? $"{Type.Name} ({Count})" : $"{Type.Name}";
        }

    }
}
