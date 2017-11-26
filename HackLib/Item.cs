using System.Collections.Generic;
using System.Diagnostics;

namespace HackLib
{
    public class ItemType
    {
        public string Tag;
        public string Name;
        public bool Stacking;

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
        private static readonly List<ItemType> _types = new List<ItemType>();

        public static void InitTypes()
        {
            Debug.Assert(_types.Count == 0);

            _types.Add(new ItemType
            {
                Tag = "wood",
                Name = "Wood",
                Stacking = true
            });

            _types.Add(new ItemType
            {
                Tag = "stone",
                Name = "Stone",
                Stacking = true
            });

            _types.Add(new ItemType
            {
                Tag = "ore",
                Name = "Iron Ore",
                Stacking = true
            });

            _types.Add(new ItemType
            {
                Tag = "bar",
                Name = "Iron Bar",
                Stacking = true
            });
            
            _types.Add(new ItemType
            {
                Tag = "food",
                Name = "Food",
                Stacking = true
            });

            _types.Add(new ItemType
            {
                Tag = "tool-axe1",
                Name = "Stone Axe",
                Stacking = false
            });

            _types.Add(new ItemType
            {
                Tag = "tool-axe2",
                Name = "Iron Axe",
                Stacking = false
            });

            _types.Add(new ItemType
            {
                Tag = "tool-pick1",
                Name = "Stone Pickaxe",
                Stacking = false
            });

            _types.Add(new ItemType
            {
                Tag = "tool-pick2",
                Name = "Iron Pickaxe",
                Stacking = false
            });

            _types.Add(new ItemType
            {
                Tag = "tool-sword1",
                Name = "Wooden Sword",
                Stacking = false
            });

            _types.Add(new ItemType
            {
                Tag = "tool-sword2",
                Name = "Iron Sword",
                Stacking = false
            });
        }

        public static ItemType Get(string tag)
        {
            var itemType = _types.Find(t => t.Tag == tag);
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
