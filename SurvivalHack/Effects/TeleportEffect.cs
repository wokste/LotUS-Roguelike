using HackConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalHack.Effects
{
    class TeleportEffect : IEntityEffect
    {
        public EntityTarget UseOn { get; }

        public TeleportEffect(EntityTarget useOn)
        {
            UseOn = useOn;
        }

        public float Efficiency(Entity instignator, Entity target)
        {
            // TODO:
            return 1;
        }

        public bool Use(Entity instignator, Entity target, StringBuilder sb)
        {
            var level = target.Level;
            var rnd = Game.Rnd;

            for (int i = 0; i < 10000; ++i)
            {
                var v = new Vec(rnd.Next(level.Size.X), rnd.Next(level.Size.Y));

                var tile = level.GetTile(v);

                if (!tile.IsFloor || tile.WalkDanger != 0)
                    continue;

                if (target.TrySetPos(v))
                    return true;
            }
            return false;
        }
    }
}
