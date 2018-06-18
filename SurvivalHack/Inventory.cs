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

        public readonly Entity[] Equipped = new Entity[SlotNames.Length];

        public void Add(Entity entity)
        {
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

            for (int i = 0; i < Equipped.Length; ++i)
            {
                if (Equipped[i] == entity)
                    Equipped[i] = null;
            }
        }

        public bool Equip(Entity self, Entity item, int slot)
        {
            Debug.WriteLine($"Equip item {item?.Name}");
            if (item != null)
            {
                var ecs = item.Get<EquippableComponent>().ToArray();

                if (!ecs.Any(ec => ec.FitsIn(SlotNames[slot].type)))
                    return false; // Can't equip said item in said slot
            }

            Unequip(slot);
            Unequip(item);

            Equipped[slot] = item;

            return true;
        }

        public int? EquippedInSlot(Entity item)
        {
            for (int i = 0; i < Equipped.Length; ++i)
                if (Equipped[i] == item)
                    return i;

            return null;
        }

        public bool Unequip(Entity Item)
        {
            var slot = EquippedInSlot(Item);
            if (slot is int s)
            {
                return Unequip(s);
            }
            return true;
        }

        public bool Unequip(int slot)
        {
            // TODO: Cursed items

            Equipped[slot] = null;
            return true;
        }

        public string Describe() => null;

        public IEnumerable<UseFunc> GetActions(Entity self, BaseEvent message, EUseSource source) => Enumerable.Empty<UseFunc>();
    }

    public class EquippableComponent : IComponent
    {
        private ESlotType _slotType;

        public EquippableComponent(ESlotType slotType)
        {
            _slotType = slotType;
        }

        
        public string Describe() => $"Can be equipped in {_slotType}";

        internal bool FitsIn(ESlotType type) => (type == _slotType) || (type == ESlotType.Hand && _slotType == ESlotType.Offhand);

        public IEnumerable<UseFunc> GetActions(Entity self, BaseEvent message, EUseSource source) => Enumerable.Empty<UseFunc>();
    }

    public enum ESlotType
    {
        Hand, Offhand, Ranged, Head, Body, Gloves, Feet, Neck, Ring
    }
}
