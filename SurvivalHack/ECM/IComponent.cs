using SurvivalHack.Combat;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SurvivalHack.ECM
{
    public interface IComponent
    {
        string Describe();
        IEnumerable<UseFunc> GetActions(BaseEvent message, EUseSource source);
    }

    public struct UseFunc {
        public EUseOrder Order;
        public Action<BaseEvent> Action;

        public UseFunc(Action<BaseEvent> action, EUseOrder order = EUseOrder.Event)
        {
            Action = action;
            Order = order;
        }
    }

    public enum EUseOrder
    {
        Interrupt,
        PreEvent,
        Event,
        PostEvent,
    }

    public enum EUseSource
    {
        User,
        UserItem,
        This,
        Target,
        TargetItem,
        //Timer,
    }
}
