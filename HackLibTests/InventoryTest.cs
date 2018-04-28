using Microsoft.VisualStudio.TestTools.UnitTesting;
using SurvivalHack;

namespace HackConsoleTests
{
    [TestClass]
    public class InventoryTest
    {
        [TestMethod]
        public void AddInventory()
        {
            var inv = new Inventory();
            var type1 = new ItemType
            {
                Stacking = true,
            };
            var type2 = new ItemType
            {
                Stacking = true,
            };

            Assert.IsNull(inv.Find(type1));
            Assert.IsNull(inv.Find(type2));

            inv.Add(new Item
            {
                Type = type1,
                Count = 5,
            });
            
            Assert.AreEqual(inv.Find(type1).Count, 5);
            Assert.IsNull(inv.Find(type2));

            inv.Add(new Item
            {
                Type = type2,
                Count = 3,
            });

            Assert.AreEqual(inv.Find(type1).Count, 5);
            Assert.AreEqual(inv.Find(type2).Count, 3);

            inv.Add(new Item
            {
                Type = type1,
                Count = 4,
            });
            
            Assert.AreEqual(inv.Find(type1).Count, 9);
            Assert.AreEqual(inv.Find(type2).Count, 3);
        }
    }
}
