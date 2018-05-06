namespace SurvivalHack.ECM
{
    public interface IUsableComponent : IComponent
    {
        bool Use(Entity item, Entity user, EUseMessage msg);
    }

    public enum EUseMessage
    {
        Drink,     // Mainly for potions
        Read,      // Scrolls, books, etc.
        Attack,    // Hitting someone. Vampiric daggers etc.
        Kill,      // Killing someone. E.g. 
        Block,     // Item blocks an attack. Shields etc.
        Damaged,   // Entity is damaged.
        Destroyed, // Entity is destroyed. Spawning smaller slimes when a slime dies
    }

    public class HealComponent : IUsableComponent
    {
        public int Restore;
        public int StatID;
        public EUseMessage MessageFilter;

        public HealComponent(int restore, int statID, EUseMessage filter)
        {
            Restore = restore;
            StatID = statID;
            MessageFilter = filter;
        }

        public bool Use(Entity item, Entity user, EUseMessage msg)
        {
            if (msg != MessageFilter)
                return false;

            switch (StatID)
            {
                case 0:
                    user.Health.Current += Restore;
                    break;
                case 1:
                    user.Hunger.Current += Restore;
                    break;
            }

            return true;
        }
    }
}