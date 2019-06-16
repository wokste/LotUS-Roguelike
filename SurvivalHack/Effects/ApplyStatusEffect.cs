using System.Text;
using System.Xml.Serialization;

namespace SurvivalHack.Effects
{
    public class ApplyStatusEffect : IEffect
    {
        [XmlElement]
        public StatusEffect Effect { get; set; }

        [XmlAttribute]
        public EntityTarget UseOn { get; set; }

        public float Efficiency(Entity instignator, Entity target)
        {
            return 1;
        }

        public void Use(Entity instignator, Entity target, StringBuilder sb)
        {
            Effect.AddCopyTo(target);
        }
    }
}
