﻿using System.Collections.Generic;

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

        private void Consume(UseMessage msg)
        {
            Count--;
            if (Count <= 0)
            {
                msg.Self?.GetOne<Inventory>()?.Remove(msg.Item);
                msg.Item.Destroy();
            }
        }

        public IEnumerable<UseFunc> GetActions(UseMessage msg, EUseSource source) {
            if (source == EUseSource.This)
            {
                if (msg is DrinkMessage) // TODO, others
                {
                    yield return new UseFunc(Consume, EUseOrder.PostEvent);
                }
            }
        }

        public string Describe() => $"Stack consists of {Count} items";
    }
}
