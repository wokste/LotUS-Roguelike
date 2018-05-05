using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
