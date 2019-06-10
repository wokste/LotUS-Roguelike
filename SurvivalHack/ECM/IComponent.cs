using System.Collections.Generic;

namespace SurvivalHack.ECM
{
    public interface IComponent
    {
    }

    public interface IActionComponent : IComponent
    {
        void GetActions(Entity self, BaseEvent message, EUseSource source);
    }

    public interface INestedComponent : IComponent
    {
        void GetNested<T>(IList<T> list) where T : class, IComponent;      
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
