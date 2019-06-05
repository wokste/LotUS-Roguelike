using System.Collections.Generic;
using System.Linq;
using HackConsole;
using SFML.Graphics;
using SurvivalHack.ECM;

namespace SurvivalHack
{
    public class Inventory : IComponent
    {
        public readonly List<Entity> Items = new List<Entity>();
        public const int SLOT_RANGED = 2;
        public const int SLOT_WAND = 3;

        public static readonly (ESlotType type, string name, char key)[] SlotNames = new(ESlotType type, string name, char key)[] {
            (ESlotType.Hand,    "Main Hand", 'm'),
            (ESlotType.Offhand, "Offhand", 'o'),
            (ESlotType.Ranged,  "Ranged", 'r'),
            (ESlotType.Wand,    "Wand", 'z'),
            (ESlotType.Head,    "Head", 'h'),
            (ESlotType.Body,    "Body", 'b'),
            (ESlotType.Gloves,  "Gloves", 'g'),
            (ESlotType.Feet,    "Feet", 'f'),
            (ESlotType.Neck,    "Neck", 'n'),
            (ESlotType.Ring,    "Ring main hand", 'M'),
            (ESlotType.Ring,    "Ring offhand", 'O'),
        };

        public readonly Slot[] Slots = new Slot[SlotNames.Length];

        public void Add(Entity item)
        {
            for (int i = 0; i < Slots.Length; ++i)
                if (CanEquipInSlot(i, item))
                    Slots[i].NewItems = true;

            var stack1 = item.GetOne<StackComponent>();
            if (stack1 != null)
            {
                foreach (var i in Items)
                {
                    var stack2 = i.GetOne<StackComponent>();

                    if (stack2 == null || stack1.Prototype != stack2.Prototype)
                        continue;

                    // Stacking items shouldn't create a new stack if you already have a stack.
                    stack2.Count += stack1.Count;
                    ColoredString.Write($"You aquired {Word.AName(item)} making a total of @cc{stack2.Count}@ca");

                    return;
                };
            }

            ColoredString.Write($"You aquired {Word.AName(item)}");
            Items.Add(item);
        }

        public bool Remove(Entity item)
        {
            if (EquippedInSlot(item) is int slot)
            {
                if (item.EntityFlags.HasFlag(EEntityFlag.Cursed))
                    return false;

                Slots[slot].Item = null;
            }

            Items.Remove(item);
            return true;
        }

        public bool Equip(Entity item, int slot)
        {
            if (item != null && !CanEquipInSlot(slot, item))
            {
                return false; // Can't equip said item in said slot
            }

            int? lastSlot = EquippedInSlot(item);

            // Check for cursed items
            if ((lastSlot != null && item.EntityFlags.HasFlag(EEntityFlag.Cursed)) || (Slots[slot].Item != null && Slots[slot].Cursed))
                return false;

            if (lastSlot is int s)
                Slots[s].Item = null;

            Slots[slot].Item = item;

            return true;
        }

        public int? EquippedInSlot(Entity item)
        {
            if (item == null)
                return null;

            for (int i = 0; i < Slots.Length; ++i)
                if (Slots[i].Item == item)
                    return i;

            return null;
        }

        public static bool CanEquipInSlot(int slotId, Entity item)
        {
            var slotType = SlotNames[slotId].type;

            bool Fits(ESlotType? actualSlot) => (slotType == actualSlot) || (slotType == ESlotType.Offhand && actualSlot == ESlotType.Hand);

            return item.Components.Any(c => Fits((c as IEquippableComponent)?.SlotType));
        }

        public struct Slot
        {
            public Entity Item;
            public bool NewItems;
            public bool Cursed => Item?.EntityFlags.HasFlag(EEntityFlag.Cursed) ?? false;

            internal Color GetBackgroundColor()
            {
                if (Cursed)
                    return new Color(64, 0, 0);
                else if (NewItems)
                    return new Color(0, 64, 0);
                else
                    return new Color(0, 0, 64);
            }
        }
    }
    
    public class Equippable : IEquippableComponent
    {
        public Equippable(ESlotType slotType)
        {
            SlotType = slotType;
        }

        public ESlotType SlotType { get; }

        public void GetActions(Entity self, BaseEvent msg, EUseSource source) {}
    }

    public enum ESlotType
    {
        Hand, Offhand, Ranged, Wand, Head, Body, Gloves, Feet, Neck, Ring
    }
}
