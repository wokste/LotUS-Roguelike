using System.Collections.Generic;
using System.Diagnostics;
using HackConsole;
using SurvivalHack.ECM;

namespace SurvivalHack
{
    public class Inventory
    {
        public readonly List<Entity> _items = new List<Entity>();

        public void Add(Entity entity)
        {
            var stackComponent = entity.StackComponent;
            if (stackComponent != null)
            {
                var existing = _items.Find(i => i.StackComponent.MergeId == stackComponent.MergeId);
                if (existing != null)
                {
                    // Stacking items shouldn't create a new stack if you already have a stack.
                    existing.StackComponent.Count += stackComponent.Count;
                    Message.Write($"You aquired {entity} making a total of {existing}", null, Color.Green);
                    return;
                }
            }
            
            Message.Write($"You aquired {entity}", null, Color.Green);
            _items.Add(entity);
        }

        public void Remove(Entity entity) {
            _items.Remove(entity);
        }
    }
}
