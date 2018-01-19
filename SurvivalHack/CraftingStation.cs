using System;
using System.Collections.Generic;

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
                    new CraftingRecipy.ItemRef("stone",5),
                    new CraftingRecipy.ItemRef("wood",1)
                },
                Output = new CraftingRecipy.ItemRef("axe1",1)
            });

            _recipies.Add(new CraftingRecipy
            {
                Input = new List<CraftingRecipy.ItemRef>
                {
                    new CraftingRecipy.ItemRef("ore",5),
                    new CraftingRecipy.ItemRef("wood",1)
                },
                Output = new CraftingRecipy.ItemRef("axe2", 1)
            });

            _recipies.Add(new CraftingRecipy
            {
                Input = new List<CraftingRecipy.ItemRef>
                {
                    new CraftingRecipy.ItemRef("stone",5),
                    new CraftingRecipy.ItemRef("wood",1)
                },
                Output = new CraftingRecipy.ItemRef("pick1", 1)
            });

            _recipies.Add(new CraftingRecipy
            {
                Input = new List<CraftingRecipy.ItemRef>
                {
                    new CraftingRecipy.ItemRef("ore",5),
                    new CraftingRecipy.ItemRef("wood",1)
                },
                Output = new CraftingRecipy.ItemRef("pick2", 1)
            });

            _recipies.Add(new CraftingRecipy
            {
                Input = new List<CraftingRecipy.ItemRef>
                {
                    new CraftingRecipy.ItemRef("wood",5),
                },
                Output = new CraftingRecipy.ItemRef("sword1", 1)
            });

            _recipies.Add(new CraftingRecipy
            {
                Input = new List<CraftingRecipy.ItemRef>
                {
                    new CraftingRecipy.ItemRef("ore",5),
                },
                Output = new CraftingRecipy.ItemRef("sword2", 1)
            });
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
                inventory.Consume(item, count * tuple.Count);
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
