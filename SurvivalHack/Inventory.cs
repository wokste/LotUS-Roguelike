﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

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
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"You aquired {item} making a total of {existing}");
                    return;
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"You aquired first {item}");
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

        public void Consume(Item item, int count = 1)
        {
            item.Count -= count;
            
            Debug.Assert(item.Count >= 0);

            if (item.Count == 0)
                _items.Remove(item);
        }
    }
}