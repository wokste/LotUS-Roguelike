using Microsoft.VisualStudio.TestTools.UnitTesting;
using SurvivalHack;
using SurvivalHack.ECM;
using System.Collections.Generic;

namespace HackConsoleTests
{
    [TestClass]
    public class InventoryTest
    {

        public int? FindItem(Inventory inv, int mergeId)
        {
            var item = inv._items.Find(i => i.GetOne<StackComponent>() != null && i.GetOne<StackComponent>().MergeId == mergeId);
            if (item == null)
                return null;

            return item.GetOne<StackComponent>().Count;
        }

        [TestMethod]
        public void InventoryStackSize()
        {
            const int STACK_TYPE_A = 1;
            const int STACK_TYPE_B = 2;

            var inv = new Inventory();

            Assert.IsNull(FindItem(inv, STACK_TYPE_A));
            Assert.IsNull(FindItem(inv, STACK_TYPE_B));

            inv.Add(new Entity('a',"A",EEntityFlag.Pickable)
            {
                Components = new List<IComponent> { new StackComponent(5, STACK_TYPE_A) }
            });
            
            Assert.AreEqual(FindItem(inv, STACK_TYPE_A), 5);
            Assert.IsNull(FindItem(inv, STACK_TYPE_B));

            inv.Add(new Entity('b', "B", EEntityFlag.Pickable)
            {
                Components = new List<IComponent> { new StackComponent(3, STACK_TYPE_B) }
            });

            Assert.AreEqual(FindItem(inv, STACK_TYPE_A), 5);
            Assert.AreEqual(FindItem(inv, STACK_TYPE_B), 3);

            inv.Add(new Entity('a', "A", EEntityFlag.Pickable)
            {
                Components = new List<IComponent> { new StackComponent(4, STACK_TYPE_A) }
            });
            
            Assert.AreEqual(FindItem(inv, STACK_TYPE_A), 9);
            Assert.AreEqual(FindItem(inv, STACK_TYPE_B), 3);
        }
    }
}
