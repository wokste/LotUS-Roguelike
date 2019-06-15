using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SurvivalHack.Effects
{
    public class ApplyStatusEffect : Effect
    {
        [XmlElement]
        public StatusEffect Effect { get; set; }

        public override float Efficiency(Entity instignator, Entity target)
        {
            return 1;
        }

        public override void Use(Entity instignator, Entity target, StringBuilder sb)
        {
            Effect.AddCopyTo(target);
        }
    }
}
