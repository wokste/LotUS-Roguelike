namespace SurvivalHack.ECM
{
    public interface IComponent
    {
        string Describe();
        void GetActions(Entity self, BaseEvent message, EUseSource source);
        bool FitsIn(ESlotType type);
    }

    public abstract class Component : IComponent
    {
        public virtual string Describe() => "";
        public virtual void GetActions(Entity self, BaseEvent message, EUseSource source) { }
        public virtual bool FitsIn(ESlotType type) => false;
    }

    public enum EUseSource
    {
        None, 
        User,
        UserItem,
        Item,
        Target,
        TargetItem,
        //Timer,
    }
}
