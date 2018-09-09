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

        private void Consume(BaseEvent msg)
        {
            Count--;
            if (Count <= 0)
            {
                msg.User?.GetOne<Inventory>()?.Remove(msg.Item);
                msg.Item.Destroy();
            }
        }

        public void GetActions(Entity self, BaseEvent message, EUseSource source) {
            if (source == EUseSource.Item)
            {
                if (message is ConsumeEvent) // TODO, others
                {
                    message.PostEvent += Consume;
                }
            }
        }

        public string Describe() => $"Stack consists of {Count} items";

        public bool FitsIn(ESlotType type) => false;
    }
}
