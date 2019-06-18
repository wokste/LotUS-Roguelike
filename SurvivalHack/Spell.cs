using HackConsole;
using SurvivalHack.Combat;
using SurvivalHack.ECM;
using SurvivalHack.Effects;
using System.Collections.Generic;
using System.Text;

namespace SurvivalHack
{
    public class Spell : IComponent
    {
        // TODO: XML
        public int MpCost;
        public EffectList Effects;

        public Spell(int mpCost, EffectList effects)
        {
            MpCost = mpCost;
            Effects = effects;
        }

        public bool Cast(Entity caster, Entity target)
        {
            var statBlock = caster.GetOne<StatBlock>();
            if (statBlock?.Spend(MpCost, EStat.MP) ?? false)
            {
                var stringBuilder = new StringBuilder();
                Effects.Use(caster, target, stringBuilder, TargetFilter.Self);
                ColoredString.OnMessage(stringBuilder.ToString());
                return true;
            }
            return false;
        }
    }
}
