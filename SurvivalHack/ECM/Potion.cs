using HackConsole;
using SurvivalHack.Effects;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SurvivalHack.ECM
{
    public class Potion : IActionComponent
    {

        [XmlElement]
        public EffectList Effects { get; set; }

        [XmlAttribute]
        public string IdentifiedName { get; set; }

        [XmlAttribute]
        public string UnidentifiedName { get; set; }

        [XmlElement]
        public TileGlyph Glyph { get; set; }

        [XmlAttribute]
        public bool IsIdentified { get; set; }

        public Potion()
        { }

        public Potion(string identifiedName, EffectList effects)
        {
            IdentifiedName = identifiedName;
            Effects = effects;
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

            Effects.Use(user, user, sb, EntityTarget.Self);
            ColoredString.OnMessage(sb.ToString());
        }
    }
}
