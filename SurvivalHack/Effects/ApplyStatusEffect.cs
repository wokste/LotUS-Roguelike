using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalHack.Effects
{
    class ApplyStatusEffect : IEntityEffect
    {
        public StatusEffect Effect;

        public EntityTarget UseOn { get; set; }

        public float Efficiency(Entity instignator, Entity target)
        {
            return 1;
        }

        public bool Use(Entity instignator, Entity target, StringBuilder sb)
        {
            Effect.AddCopyTo(target);
            return true;
        }
    }
}
