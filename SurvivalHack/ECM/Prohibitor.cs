using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalHack.ECM
{
    public class Prohibitor : IComponent
    {
        public Type EventType;
        public EUseSource Source;
        public bool CurseOnly;
        public string Message;

        public Prohibitor(Type eventType, string message, EUseSource source, bool curseOnly = false)
        {
            EventType = eventType;
            Source = source;
            CurseOnly = curseOnly;
        }

        public string Describe() => null;
        public void GetActions(Entity self, BaseEvent msg, EUseSource source)
        {
            if (EventType.IsAssignableFrom(msg.GetType()) && source == Source && (!CurseOnly || self.EntityFlags.HasFlag(EEntityFlag.Cursed)))
                msg.OnEventCheck.Add(m => Message);
        }
    }
}
