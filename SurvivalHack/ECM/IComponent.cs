using SurvivalHack.Combat;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SurvivalHack.ECM
{
    public interface IComponent
    {
        string Describe();
        IEnumerable<UseFunc> GetActions(Entity self, BaseEvent message, EUseSource source);
    }

    public struct UseFunc {
        public EUseOrder Order;
        public Action<BaseEvent> Action;
        public string Message;

        public UseFunc(Action<BaseEvent> action, EUseOrder order = EUseOrder.Event)
        {
            Action = action;
            Order = order;
            Message = null;
        }

        internal static UseFunc MakeInterrupt(string message)
        {
            return new UseFunc
            {
                Action = null,
                Order = EUseOrder.Interrupt,
                Message = message
            };
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
        None, 
        User,
        UserItem,
        This,
        Target,
        TargetItem,
        //Timer,
    }
}
