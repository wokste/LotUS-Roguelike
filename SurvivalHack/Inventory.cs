using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalHack
{
    class Inventory
    {
        private List<Item> items = new List<Item>();

        internal void Add(Item item)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"You aquired {item}");

            if (item.Type.Stacking)
            {
                var existing = items.Find(i => i.Type == item.Type);
                if (existing != null)
                {
                    // Stacking items shouldn't create a new stack if you already have a stack.
                    existing.Count += item.Count;
                    return;
                }
            }
            items.Add(item);
        }

        internal Item Find(ItemType type)
        {
            return items.Find(i => i.Type == type);
        }

        public void Write()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Inventory:");
            Console.ForegroundColor = ConsoleColor.Gray;
            foreach (var i in items)
            {
                Console.WriteLine($"  {i}");
            }
        }

        internal void Add(string itemTag, int count)
        {
            var type = ItemTypeList.Get(itemTag);
            Add(type.Make(count));
        }
    }
}
