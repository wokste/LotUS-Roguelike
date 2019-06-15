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
        [XmlIgnore]
        public List<IComponent> Components = new List<IComponent>();
        [XmlIgnore]
        private Entity _parent;

        [XmlElement]
        public EffectList TickEffect;
        [XmlElement]
        public EffectList FinalEffect;

        [XmlAttribute]
        public int RepeatTurns { get; set; } = 1;
        [XmlAttribute]
        public int RunsToExecute { get; set; } = int.MaxValue;

        public void GetNested<T>(IList<T> list) where T : class, IComponent
        {
            foreach (var c in Components)
            {
                if (c is T cT)
                    list.Add(cT);

                if (c is INestedComponent cNested)
                    cNested.GetNested(list);
            }
        }

        public void AddCopyTo(Entity parent)
        {
            var copy = new StatusEffect
            {
                Components = Components,
                TickEffect = TickEffect,
                FinalEffect = FinalEffect,
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
            TickEffect.Use(_parent, _parent, null, EntityTarget.Self);

            if (RunsToExecute == 0)
            {
                FinalEffect.Use(_parent, _parent, null, EntityTarget.Self);
                Unattach();
            }
        }
    }
}
