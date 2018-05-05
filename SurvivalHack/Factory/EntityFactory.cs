using System;

namespace SurvivalHack.Factory
{
    public interface IEntityFactory
    {
        Entity Gen(EntityGenerationInfo info);
    }

    /// <summary>
    /// This struct is mainly used to not need to change all entity factories if more arguments are added to the Gen function
    /// </summary>
    public struct EntityGenerationInfo {
        public Random Rnd;
        public int Level;
    }
}
