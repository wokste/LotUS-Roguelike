using System.Collections.Generic;
using System.Diagnostics;
using HackConsole;

namespace SurvivalHack
{
    public class Inventory
    {
        public readonly List<Item> _items = new List<Item>();

        public void Add(Item item)
        {
            if (item.Type.Stacking)
            {
                var existing = _items.Find(i => i.Type == item.Type);
                if (existing != null)
                {
                    // Stacking items shouldn't create a new stack if you already have a stack.
                    existing.Count += item.Count;
                    Message.Write($"You aquired {item} making a total of {existing}", null, Color.Green);
                    return;
                }
            }
            
            Message.Write($"You aquired first {item}", null, Color.Green);
            _items.Add(item);
        }

        public Item Find(ItemType type)
        {
            return _items.Find(i => i.Type == type);
        }

        public void Add(string itemTag, int count)
        {
            var type = ItemTypeList.Get(itemTag);
            Add(type.Make(count));
        }

        public void Consume(Item item, int count = 1)
        {
            item.Count -= count;
            
            Debug.Assert(item.Count >= 0);

            if (item.Count == 0)
                _items.Remove(item);
        }
    }
}
