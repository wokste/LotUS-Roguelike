using HackConsole;
using SurvivalHack.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalHack.ECM
{
    public class Potion : IComponent
    {
        public IEnumerable<IEffect> Effects;
        public string IdentifiedName;
        public string UnidentifiedName;
        public TileGlyph Glyph;
        public bool IsIdentified;

        public Potion(string identifiedName, IEffect[] effects)
        {
            IdentifiedName = identifiedName;
            Effects = effects;
        }

        public string Describe()
        {
            return $"Does something when drunk";
            // TODO: Add better description based on effects.
        }

        public bool FitsIn(ESlotType type)
        {
            return false;
        }

        public void GetActions(Entity self, BaseEvent message, EUseSource source)
        {
            if (message is ConsumeEvent evt && source == EUseSource.Item)
            {
                evt.OnEvent += Drink;
            }
            // TODO: Throwing potions
        }

        private void Drink(BaseEvent evt)
        {
            var user = ((ConsumeEvent)evt).User;
            var sb = new StringBuilder();
            sb.Append($"{IdentifiedName}: ");

            foreach (var e in Effects)
            {
                if (e is IEntityEffect ee)
                {
                    if (ee.UseOn.HasFlag(EntityTarget.Self))
                        ee.Use(user, user, sb);
                }
            }
            ColoredString.OnMessage(sb.ToString());
        }
    }
}
