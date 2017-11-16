using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalHack
{
    class ItemType
    {
        public string Tag;
        public string Name;
        public bool Stacking;

        public override string ToString()
        {
            return Name;
        }

        internal Item Make(int count)
        {
            return new Item
            {
                Count = count,
                Type = this
            };
        }
    }

    static class ItemTypeList {
        private static List<ItemType> _types = new List<ItemType>();

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
                Tag = "ore",
                Name = "Ore",
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
                Tag = "tool-axe",
                Name = "Axe",
                Stacking = false
            });
        }

        internal static ItemType Get(string tag)
        {
            return _types.Find(t => t.Tag == tag);
        }
    }

    class Item
    {
        public ItemType Type;
        public int Count;

        public override string ToString()
        {
            return Type.Stacking ? $"{Type.Name} ({Count})" : $"{Type.Name}";
        }

    }
}
