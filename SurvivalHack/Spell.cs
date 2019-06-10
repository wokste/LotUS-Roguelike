using HackConsole;
using SurvivalHack.Combat;
using SurvivalHack.ECM;
using SurvivalHack.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalHack
{
    class Spell : IComponent
    {
        public int MpCost;
        public IList<IEffect> Effects;

        public Spell(int mpCost, IList<IEffect> effects)
        {
            MpCost = mpCost;
            Effects = effects;
        }

        public bool Cast(Entity caster, Entity target)
        {
            var statBlock = caster.GetOne<StatBlock>();
            if (statBlock?.Spend(MpCost, 1) ?? false)
            {
                var stringBuilder = new StringBuilder();
                foreach (var e in Effects)
                {
                    if (e is IEntityEffect ee)
                    {
                        ee.Use(caster, target, stringBuilder);
                    }
                }
                ColoredString.OnMessage(stringBuilder.ToString());
                return true;
            }
            return false;
        }
    }
}
