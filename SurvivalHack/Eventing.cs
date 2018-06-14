using SurvivalHack.ECM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalHack
{
    static class Eventing
    {
        public static bool On(BaseEvent message)
        {
            var funcs = new List<UseFunc>();
            funcs.AddRange(message.Item.Components.SelectMany(c => c.GetActions(message, EUseSource.This)));

            if (!funcs.Any(f => f.Order == EUseOrder.Event))
            {
                return false;
            }

            if (message.Target != null)
            {
                funcs.AddRange(message.Target.Components.SelectMany(c => c.GetActions(message, EUseSource.Target)));
                funcs.AddRange(message.Target.ListSubEntities().SelectMany(e => e.Components.SelectMany(c => c.GetActions(message, EUseSource.TargetItem))));
            }

            if (message.Self != null)
            {
                funcs.AddRange(message.Self.Components.SelectMany(c => c.GetActions(message, EUseSource.User)));
                funcs.AddRange(message.Self.ListSubEntities().SelectMany(e => e.Components.SelectMany(c => c.GetActions(message, EUseSource.UserItem))));
            }

            if (funcs.Any(f => f.Order == EUseOrder.Interrupt))
            {
                return false;
            }

            foreach (var f in funcs.OrderBy(f => f.Order))
            {
                f.Action?.Invoke(message);
            }

            return true;
        }
    }
}
