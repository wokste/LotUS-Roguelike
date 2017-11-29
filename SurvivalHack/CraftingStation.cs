using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HackLib;

namespace SurvivalHack
{
    public class CraftingStation
    {
        private readonly List<CraftingRecipy> _recipies = new List<CraftingRecipy>();
        
        public void Init()
        {
            _recipies.Add(new CraftingRecipy
            {
                Input = new List<CraftingRecipy.ItemRef>
                {
                    new CraftingRecipy.ItemRef("stone",3),
                    new CraftingRecipy.ItemRef("wood",1)
                },
                Output = new CraftingRecipy.ItemRef("tool-axe1",1)
            });

            _recipies.Add(new CraftingRecipy
            {
                Input = new List<CraftingRecipy.ItemRef>
                {
                    new CraftingRecipy.ItemRef("bar",3),
                    new CraftingRecipy.ItemRef("wood",1)
                },
                Output = new CraftingRecipy.ItemRef("tool-axe2", 1)
            });

            _recipies.Add(new CraftingRecipy
            {
                Input = new List<CraftingRecipy.ItemRef>
                {
                    new CraftingRecipy.ItemRef("stone",3),
                    new CraftingRecipy.ItemRef("wood",1)
                },
                Output = new CraftingRecipy.ItemRef("tool-pick1", 1)
            });

            _recipies.Add(new CraftingRecipy
            {
                Input = new List<CraftingRecipy.ItemRef>
                {
                    new CraftingRecipy.ItemRef("bar",3),
                    new CraftingRecipy.ItemRef("wood",1)
                },
                Output = new CraftingRecipy.ItemRef("tool-pick2", 1)
            });

            _recipies.Add(new CraftingRecipy
            {
                Input = new List<CraftingRecipy.ItemRef>
                {
                    new CraftingRecipy.ItemRef("wood",3),
                },
                Output = new CraftingRecipy.ItemRef("tool-sword1", 1)
            });

            _recipies.Add(new CraftingRecipy
            {
                Input = new List<CraftingRecipy.ItemRef>
                {
                    new CraftingRecipy.ItemRef("bar",3),
                },
                Output = new CraftingRecipy.ItemRef("tool-sword2", 1)
            });

            _recipies.Add(new CraftingRecipy
            {
                Input = new List<CraftingRecipy.ItemRef>
                {
                    new CraftingRecipy.ItemRef("ore",3),
                },
                Output = new CraftingRecipy.ItemRef("bar", 1)
            });
        }

        public void OpenCraftingMenu(Inventory inventory)
        {
            while (true)
            {
                var recipies = _recipies.Where(r => r.CraftCount(inventory) > 0).ToArray();
                var recipy = Menu.ShowList("What do you want to craft?", recipies);
                if (recipy == null)
                    return;

                var max = recipy.CraftCount(inventory);
                var count = recipy.Output.Type.Stacking ? Menu.AskInt("How Many? (Max {max})", 0, max) : 1;
                
                recipy.Craft(inventory,count);
            }
        }
    }

    public class CraftingRecipy
    {
        public List<ItemRef> Input = new List<ItemRef>();
        public ItemRef Output;

        public void Craft(Inventory inventory, int count = 1)
        {
            foreach (var tuple in Input)
            {
                var item = inventory.Find(tuple.Type);
                Debug.Assert(item != null);

                item.Count -= count * tuple.Count;
                Debug.Assert(item.Count >= 0);
            }

            inventory.Add(Output.Type.Make(count * Output.Count));
        }

        public int CraftCount(Inventory inventory)
        {
            var returnValue = Int32.MaxValue;
            foreach (var tuple in Input)
            {
                var item = inventory.Find(tuple.Type);
                if (item == null)
                    return 0;

                var count = item.Count / tuple.Count;
                returnValue = Math.Min(returnValue, count);
            }
            return returnValue;
        }

        public override string ToString()
        {
            return Output.Type.ToString();
        }

        public class ItemRef
        {
            public ItemType Type;
            public int Count;

            public ItemRef(string typeName, int count)
            {
                Type = ItemTypeList.Get(typeName);
                Count = count;
            }
        }
    }
}
