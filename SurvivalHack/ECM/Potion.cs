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
        public EffectList OnUse { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string UnidentifiedName { get; set; }

        [XmlElement]
        public TileGlyph Glyph { get; set; }

        public bool IsIdentified => UnidentifiedName == null;

        public Potion()
        { }

        public Potion(string name, EffectList effects)
        {
            Name = name;
            OnUse = effects;
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
            sb.Append($"{Name}: ");

            OnUse.Use(user, user, sb, EntityTarget.Self);
            ColoredString.OnMessage(sb.ToString());
        }
    }
}
