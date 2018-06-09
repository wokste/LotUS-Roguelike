using System.Collections.Generic;

namespace SurvivalHack.ECM
{
    public class StackComponent : IComponent
    {
        public int Count;
        public int MergeId;

        public StackComponent(int count, int mergeId)
        {
            Count = count;
            MergeId = mergeId;
        }

        private static int MergeIdAutoIncrement;

        public static int GenMergeId(int count)
        {
            var ret = MergeIdAutoIncrement;
            MergeIdAutoIncrement += count;
            return ret;
        }

        private void Consume(Entity user, Entity item, Entity target)
        {
            Count--;
            if (Count <= 0)
            {
                user?.GetOne<Inventory>()?.Remove(item);
                item.Destroy();
            }
        }

        public IEnumerable<UseFunc> GetActions(EUseMessage filter, EUseSource source) {
            if (source == EUseSource.This)
            {
                if (filter == EUseMessage.Drink) // TODO, others
                {
                    yield return new UseFunc(Consume, EUseOrder.PostEvent);
                }
            }
        }

        public string Describe() => $"Stack consists of {Count} items";
    }
}
