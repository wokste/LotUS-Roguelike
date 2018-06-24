using SurvivalHack.Combat;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SurvivalHack.ECM
{
    public interface IComponent
    {
        string Describe();
        void GetActions(Entity self, BaseEvent message, EUseSource source);
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
