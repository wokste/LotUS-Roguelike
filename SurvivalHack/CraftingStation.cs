using System;
using System.Collections.Generic;
using System.Deployment.Internal;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalHack
{
    internal class CraftingStation
    {
        List<CraftingRecipy> Recipies = new List<CraftingRecipy>();
        
        internal void Init()
        {
            Recipies.Add(new CraftingRecipy
            {
                Input = new List<CraftingRecipy.ItemRef>
                {
                    new CraftingRecipy.ItemRef("stone",3),
                    new CraftingRecipy.ItemRef("wood",1)
                },
                Output = new CraftingRecipy.ItemRef("tool-axe1",1)
            });

            Recipies.Add(new CraftingRecipy
            {
                Input = new List<CraftingRecipy.ItemRef>
                {
                    new CraftingRecipy.ItemRef("bar",3),
                    new CraftingRecipy.ItemRef("wood",1)
                },
                Output = new CraftingRecipy.ItemRef("tool-axe2", 1)
            });

            Recipies.Add(new CraftingRecipy
            {
                Input = new List<CraftingRecipy.ItemRef>
                {
                    new CraftingRecipy.ItemRef("stone",3),
                    new CraftingRecipy.ItemRef("wood",1)
                },
                Output = new CraftingRecipy.ItemRef("tool-pick1", 1)
            });

            Recipies.Add(new CraftingRecipy
            {
                Input = new List<CraftingRecipy.ItemRef>
                {
                    new CraftingRecipy.ItemRef("bar",3),
                    new CraftingRecipy.ItemRef("wood",1)
                },
                Output = new CraftingRecipy.ItemRef("tool-pick2", 1)
            });

            Recipies.Add(new CraftingRecipy
            {
                Input = new List<CraftingRecipy.ItemRef>
                {
                    new CraftingRecipy.ItemRef("wood",3),
                },
                Output = new CraftingRecipy.ItemRef("tool-sword1", 1)
            });

            Recipies.Add(new CraftingRecipy
            {
                Input = new List<CraftingRecipy.ItemRef>
                {
                    new CraftingRecipy.ItemRef("bar",3),
                },
                Output = new CraftingRecipy.ItemRef("tool-sword2", 1)
            });

            Recipies.Add(new CraftingRecipy
            {
                Input = new List<CraftingRecipy.ItemRef>
                {
                    new CraftingRecipy.ItemRef("ore",3),
                },
                Output = new CraftingRecipy.ItemRef("bar", 1)
            });
        }

        internal void OpenCraftingMenu(Inventory inventory)
        {
            var recipy = Menu.ShowList("What do you want to craft?", Recipies, r => r.CraftCount(inventory) > 0);
            if (recipy == null)
                return;

            if (recipy.CraftCount(inventory) < 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("You have insufficient materials.");
                return;
            }

            recipy.Craft(inventory);
        }
    }

    internal class CraftingRecipy
    {
        internal List<ItemRef> Input = new List<ItemRef>();
        internal ItemRef Output;

        internal void Craft(Inventory inventory, int count = 1)
        {
            foreach (var tuple in Input)
            {
                var item = inventory.Find(tuple.Type);
                Debug.Assert(item != null);

                item.Count -= count * tuple.Count;
                Debug.Assert(item.Count >= 0);
            }

            inventory.Add(Output.Type.Make(Output.Count));
        }

        internal int CraftCount(Inventory inventory)
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

        internal class ItemRef
        {
            internal ItemType Type;
            internal int Count;

            public ItemRef(string typeName, int count)
            {
                Type = ItemTypeList.Get(typeName);
                Count = count;
            }
        }
    }
}
