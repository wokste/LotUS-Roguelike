﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SurvivalHack;
using SurvivalHack.ECM;
using System.Collections.Generic;

namespace HackLibTests
{
    [TestClass]
    public class InventoryTest
    {
        [TestMethod]
        public void Inventory_StackSize()
        {
            object STACK_TYPE_A = "1";
            object STACK_TYPE_B = "2";

            var inv = new Inventory();

            int? FindItem(object prototype)
            {
                var item = inv.Items.Find(i => i.GetOne<StackComponent>() != null && i.GetOne<StackComponent>().Prototype == prototype);
                if (item == null)
                    return null;

                return item.GetOne<StackComponent>().Count;
            }

            Assert.IsNull(FindItem(STACK_TYPE_A));
            Assert.IsNull(FindItem(STACK_TYPE_B));

            inv.Add(new Entity(new TileGlyph(), "A", EEntityFlag.Pickable)
            {
                Components = new List<IComponent> { new StackComponent(5, STACK_TYPE_A) }
            });

            Assert.AreEqual(FindItem(STACK_TYPE_A), 5);
            Assert.IsNull(FindItem(STACK_TYPE_B));

            inv.Add(new Entity(new TileGlyph(), "B", EEntityFlag.Pickable)
            {
                Components = new List<IComponent> { new StackComponent(3, STACK_TYPE_B) }
            });

            Assert.AreEqual(FindItem(STACK_TYPE_A), 5);
            Assert.AreEqual(FindItem(STACK_TYPE_B), 3);

            inv.Add(new Entity(new TileGlyph(), "A", EEntityFlag.Pickable)
            {
                Components = new List<IComponent> { new StackComponent(4, STACK_TYPE_A) }
            });

            Assert.AreEqual(FindItem(STACK_TYPE_A), 9);
            Assert.AreEqual(FindItem(STACK_TYPE_B), 3);
        }

        [TestMethod]
        public void Inventory_Cursed()
        {
            var inv = new Inventory();

            var normalItem = new Entity(new TileGlyph(), "Item", EEntityFlag.Pickable)
            {
                Components = new List<IComponent> { new Equippable(ESlotType.Offhand) }
            };
            inv.Add(normalItem);

            var cursedItem = new Entity(new TileGlyph(), "Cursed Item", EEntityFlag.Pickable | EEntityFlag.Cursed)
            {
                Components = new List<IComponent> { new Equippable(ESlotType.Offhand) }
            };
            inv.Add(cursedItem);

            Assert.IsNull(inv.EquippedInSlot(normalItem));
            Assert.IsNull(inv.EquippedInSlot(cursedItem));

            // Equip works
            inv.Equip(normalItem, 0);
            Assert.AreEqual(0, inv.EquippedInSlot(normalItem));

            // Cannot be equipped in multiple slots simultaneously
            inv.Equip(normalItem, 1);
            Assert.AreEqual(1, inv.EquippedInSlot(normalItem));

            inv.Equip(normalItem, 0);

            // Get replace uncursed weapon with a cursed one to check whether unequip properly works
            inv.Equip(cursedItem, 0);
            Assert.IsNull(inv.EquippedInSlot(normalItem));
            Assert.AreEqual(0, inv.EquippedInSlot(cursedItem));

            inv.Equip(normalItem, 1);

            // Invalid action as cursed items can't be moved
            inv.Equip(cursedItem, 1);
            Assert.AreEqual(1, inv.EquippedInSlot(normalItem));
            Assert.AreEqual(0, inv.EquippedInSlot(cursedItem));

            // Invalid action as cursed items can't be moved
            inv.Equip(normalItem, 0);
            Assert.AreEqual(1, inv.EquippedInSlot(normalItem));
            Assert.AreEqual(0, inv.EquippedInSlot(cursedItem));

            // Drop your items, but dropping the cursed item doesn't work
            Assert.IsTrue(inv.Remove(normalItem));
            Assert.IsFalse(inv.Remove(cursedItem));

            Assert.IsNull(inv.EquippedInSlot(normalItem));
            Assert.AreEqual(0, inv.EquippedInSlot(cursedItem));

            Assert.IsFalse(inv.Items.Contains(normalItem));
            Assert.IsTrue(inv.Items.Contains(cursedItem));
        }
    }
}
