using System;
using System.Collections.Generic;

namespace HackLib
{
    public class Inventory
    {
        private readonly List<Item> _items = new List<Item>();

        public void Add(Item item)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"You aquired {item}");

            if (item.Type.Stacking)
            {
                var existing = _items.Find(i => i.Type == item.Type);
                if (existing != null)
                {
                    // Stacking items shouldn't create a new stack if you already have a stack.
                    existing.Count += item.Count;
                    return;
                }
            }
            _items.Add(item);
        }

        public Item Find(ItemType type)
        {
            return _items.Find(i => i.Type == type);
        }

        public void Write()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Inventory:");
            Console.ForegroundColor = ConsoleColor.Gray;
            foreach (var i in _items)
            {
                Console.WriteLine($"  {i}");
            }
        }

        public void Add(string itemTag, int count)
        {
            var type = ItemTypeList.Get(itemTag);
            Add(type.Make(count));
        }
    }
}
