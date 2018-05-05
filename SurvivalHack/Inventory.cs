using System.Collections.Generic;
using System.Diagnostics;
using HackConsole;
using SurvivalHack.ECM;

namespace SurvivalHack
{
    public class Inventory : IComponent
    {
        public readonly List<Entity> _items = new List<Entity>();

        public void Add(Entity entity)
        {
            var stack1 = entity.GetOne<StackComponent>();
            if (stack1 != null)
            {
                foreach (var i in _items)
                {
                    var stack2 = i.GetOne<StackComponent>();

                    if (stack2 == null)
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
        }
    }
}
