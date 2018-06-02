using System.Collections.Generic;
using System.Linq;
using HackConsole;
using SurvivalHack.ECM;

namespace SurvivalHack
{
    public class Inventory : IComponent
    {
        public readonly List<Entity> _items = new List<Entity>();

        public static readonly (ESlotType type, string name)[] SlotNames = new(ESlotType type, string name)[] {
            (ESlotType.Hand,    "Main Hand"),
            (ESlotType.Offhand, "Offhand"),
            (ESlotType.Ranged,  "Ranged"),
            (ESlotType.Head,    "Head"),
            (ESlotType.Body,    "Body"),
            (ESlotType.Legs,    "Legs"),
            (ESlotType.Gloves,  "Gloves"),
            (ESlotType.Feet,    "Feet"),
            (ESlotType.Neck,    "Neck"),
            (ESlotType.Ring,    "1st Ring"),
            (ESlotType.Ring,    "2nd Ring"),
        };

        public readonly Entity[] Equipped = new Entity[SlotNames.Length];

        public void Add(Entity entity)
        {
            var stack1 = entity.GetOne<StackComponent>();
            if (stack1 != null)
            {
                foreach (var i in _items)
                {
                    var stack2 = i.GetOne<StackComponent>();

                    if (stack2 == null || stack1.MergeId != stack2.MergeId)
                        continue;

                    // Stacking items shouldn't create a new stack if you already have a stack.
                    stack2.Count += stack1.Count;
                    Message.Write($"You aquired {entity} making a total of {stack2.Count}", null, Color.Green);

                    return;
                };
            }

            Message.Write($"You aquired {entity}", null, Color.Green);
            _items.Add(entity);
        }

        public void Remove(Entity entity) {
            _items.Remove(entity);

            for (int i = 0; i < Equipped.Length; ++i)
            {
                if (Equipped[i] == entity)
                    Equipped[i] = null;
            }
        }

        public bool Equip(Entity self, Entity item, int slot)
        {
            if (item != null)
            {
                var ecs = item.Get<EquippableComponent>();

                if (!ecs.Any(ec => ec.SlotType != SlotNames[slot].type))
                    return false; // Can't equip said item in said slot
            }

            if (Equipped[slot] != null)
            {
                //TODO: Cursed items.

                //TODO: trigger event on unequip
            }

            Equipped[slot] = item;

            if (Equipped[slot] != null)
            {
                //TODO: trigger event on equip
            }

            return true;
        }

        public IEnumerable<UseFunc> GetActions(EUseMessage filter, EUseSource source) => Enumerable.Empty<UseFunc>();
        public string Describe() => null;
    }

    public class EquippableComponent : IComponent
    {
        public ESlotType SlotType;

        public EquippableComponent(ESlotType slotType)
        {
            SlotType = slotType;
        }

        public IEnumerable<UseFunc> GetActions(EUseMessage filter, EUseSource source) => Enumerable.Empty<UseFunc>();
        public string Describe() => $"Can be equipped in {SlotType}";
    }

    public enum ESlotType
    {
        Hand, Offhand, Ranged, Head, Body, Legs, Gloves, Feet, Neck, Ring
    }
}
