using SurvivalHack.ECM;
using SurvivalHack.Effects;
using System.Collections.Generic;
using System.Linq;

namespace SurvivalHack
{
    class StatusEffect : INestedComponent, ITimeEvent
    {
        public List<IComponent> Components = new List<IComponent>();
        private Entity _parent;

        public IEnumerable<IEffect> TickEffect = Enumerable.Empty<IEffect>();
        public IEnumerable<IEffect> FinalEffect = Enumerable.Empty<IEffect>();

        public int RepeatTurns { get; set; } = 1;
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
            DoEffects(TickEffect);

            if (RunsToExecute == 0)
            {
                DoEffects(FinalEffect);
                Unattach();
            }
        }

        private void DoEffects(IEnumerable<IEffect> effects)
        {
            foreach (var e in effects)
            {
                if (e is IEntityEffect ee)
                {
                    if (ee.UseOn.HasFlag(EntityTarget.Self))
                        ee.Use(_parent, _parent, null);
                }
            }
        }
    }
}
