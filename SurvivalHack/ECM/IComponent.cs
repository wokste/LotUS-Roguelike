namespace SurvivalHack.ECM
{
    public interface IComponent
    {
    }

    public interface IActionComponent : IComponent
    {
        void GetActions(Entity self, BaseEvent message, EUseSource source);
    }

    public interface IEquippableComponent : IComponent
    {
        ESlotType SlotType { get; }
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
