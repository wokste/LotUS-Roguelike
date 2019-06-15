using HackConsole;
using SurvivalHack.Effects;
using System.Collections.Generic;
using System.Text;

namespace SurvivalHack.ECM
{
    public class Potion : IActionComponent
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
