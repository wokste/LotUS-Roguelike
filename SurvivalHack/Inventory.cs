using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HackConsole;
using SurvivalHack.ECM;

namespace SurvivalHack
{
    public class Inventory : IComponent
    {
        public readonly List<Entity> Items = new List<Entity>();

        public static readonly (ESlotType type, string name, char key)[] SlotNames = new(ESlotType type, string name, char key)[] {
            (ESlotType.Hand,    "Main Hand", 'm'),
            (ESlotType.Offhand, "Offhand", 'o'),
            (ESlotType.Ranged,  "Ranged", 'r'),
            (ESlotType.Head,    "Head", 'h'),
            (ESlotType.Body,    "Body", 'b'),
            (ESlotType.Gloves,  "Gloves", 'g'),
            (ESlotType.Feet,    "Feet", 'f'),
            (ESlotType.Neck,    "Neck", 'n'),
            (ESlotType.Ring,    "Ring main hand", 'M'),
            (ESlotType.Ring,    "Ring offhand", 'O'),
        };

        public readonly Slot[] Slots = new Slot[SlotNames.Length];

        public void Add(Entity entity)
        {
            var equippable = entity.GetOne<Equippable>();
            if (equippable != null)
                for (int i = 0; i < Slots.Length; ++i)
                    if (equippable.FitsIn(SlotNames[i].type))
                        Slots[i].NewItems = true;

            var stack1 = entity.GetOne<StackComponent>();
            if (stack1 != null)
            {
                foreach (var i in Items)
                {
                    var stack2 = i.GetOne<StackComponent>();

                    if (stack2 == null || stack1.MergeId != stack2.MergeId)
                        continue;

                    // Stacking items shouldn't create a new stack if you already have a stack.
                    stack2.Count += stack1.Count;
                    ColoredString.Write($"You aquired {entity} making a total of {stack2.Count}", Color.Green);

                    return;
                };
            }

            ColoredString.Write($"You aquired {entity}", Color.Green);
            Items.Add(entity);
        }

        public void Remove(Entity entity) {
            Items.Remove(entity);

            for (int i = 0; i < Slots.Length; ++i)
            {
                if (Slots[i].Item == entity)
                    Slots[i].Item = null;
            }
        }

        public bool Equip(Entity item, int slot)
        {
            if (item != null)
            {
                var ecs = item.Get<Equippable>().ToArray();

                if (!ecs.Any(ec => ec.FitsIn(SlotNames[slot].type)))
                    return false; // Can't equip said item in said slot
            }

            int? lastSlot = EquippedInSlot(item);

            // Check for cursed items
            if ((item.EntityFlags.HasFlag(EEntityFlag.Cursed) && lastSlot != null) || (Slots[slot].Item != null && Slots[slot].Cursed))
                return false;

            if (lastSlot is int s)
                Slots[s].Item = null;

            Slots[slot].Item = item;

            return true;
        }

        public int? EquippedInSlot(Entity item)
        {
            for (int i = 0; i < Slots.Length; ++i)
                if (Slots[i].Item == item)
                    return i;

            return null;
        }

        public string Describe() => null;

        public void GetActions(Entity self, BaseEvent message, EUseSource source) {}

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
                    return new Color(0, 0, 0);
            }
        }
    }

    public class Equippable : IComponent
    {
        private readonly ESlotType _slotType;

        public Equippable(ESlotType slotType)
        {
            _slotType = slotType;
        }
        
        public string Describe() => $"Can be equipped in {_slotType}";

        public bool FitsIn(ESlotType type) => (type == _slotType) || (type == ESlotType.Hand && _slotType == ESlotType.Offhand);

        public void GetActions(Entity self, BaseEvent msg, EUseSource source) {}
    }

    public enum ESlotType
    {
        Hand, Offhand, Ranged, Head, Body, Gloves, Feet, Neck, Ring
    }
}
