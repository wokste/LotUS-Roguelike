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
        [System.ComponentModel.DefaultValue(typeof(ComponentList))]
        public ComponentList Components = new ComponentList();
        [XmlIgnore]
        private Entity _parent;

        [XmlElement]
        [System.ComponentModel.DefaultValue(typeof(EffectList))]
        public EffectList OnTick;
        [XmlElement]
        [System.ComponentModel.DefaultValue(typeof(EffectList))]
        public EffectList OnEnd;

        [XmlAttribute]
        [System.ComponentModel.DefaultValue(1)]
        public int TicksPerTurn { get; set; } = 1;
        [XmlAttribute]
        [System.ComponentModel.DefaultValue(int.MaxValue)]
        public int Turns { get; set; } = int.MaxValue;

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
                TicksPerTurn = TicksPerTurn,
                Turns = Turns,
            };

            parent.Add(copy);
            parent.Level.Game.Timeline.Insert(copy);
        }

        public void Unattach()
        {
            _parent.Components.Remove(this);
            Turns = -1;
        }

        public void Run()
        {
            Turns--;
            OnTick.Use(_parent, _parent, null, TargetFilter.Self);

            if (Turns == 0)
            {
                OnEnd.Use(_parent, _parent, null, TargetFilter.Self);
                Unattach();
            }
        }
    }
}
