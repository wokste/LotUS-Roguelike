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

        int GenMergeId() => MergeIdAutoIncrement++;

        internal bool Consume()
        {
            Count--;
            return (Count == 0);
        }

        public bool Use(Entity user, Entity item, Entity target, EUseMessage filter) => false;
        public string Describe() => $"Stack consists of {Count} items";
    }
}
