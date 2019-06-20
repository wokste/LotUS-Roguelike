using SurvivalHack.ECM;
using SurvivalHack.Effects;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace SurvivalHack
{
    public class StatusEffect : INestedComponent, ITimeEvent
    {
        // TODO: XML This shouldn't ignore components, but I want to limit the scope for now
        [XmlElement]
        public ComponentList Components = new ComponentList();
        [XmlIgnore]
        private Entity _parent;

        [XmlElement]
        public EffectList OnTick;
        [XmlElement]
        public EffectList OnEnd;

        [XmlAttribute]
        public int RepeatTurns { get; set; } = 1;
        [XmlAttribute]
        public int RunsToExecute { get; set; } = int.MaxValue;

        public void GetNested<T>(IList<T> list) where T : class, IComponent
        {
            Components.GetNested(list);
        }

        public void AddCopyTo(Entity parent)
        {
            var copy = new StatusEffect
            {
                Components = Components,
                OnTick = OnTick,
                OnEnd = OnEnd,
                _parent = parent,
                RepeatTurns = RepeatTurns,
                RunsToExecute = RunsToExecute,
            };

            parent.Add(copy);
            parent.Level.Game.Timeline.Insert(copy);
        }

        public void Unattach()
        {
            _parent.Components.Remove(this);
            RepeatTurns = -1;
        }

        public void Run()
        {
            RunsToExecute--;
            OnTick.Use(_parent, _parent, null, TargetFilter.Self);

            if (RunsToExecute == 0)
            {
                OnEnd.Use(_parent, _parent, null, TargetFilter.Self);
                Unattach();
            }
        }
    }
}
