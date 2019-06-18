using HackConsole;
using System.Text;
using System.Xml.Serialization;

namespace SurvivalHack.Effects
{
    [XmlType("Teleport")]
    public class TeleportEffect : IEffect
    {
        [XmlAttribute]
        public TargetFilter Filter { get; set; }

        public TeleportEffect()
        {
        }

        public TeleportEffect(TargetFilter useOn)
        {
            Filter = useOn;
        }

        public float Efficiency(Entity instignator, Entity target)
        {
            // TODO:
            return 1;
        }

        public void Use(Entity instignator, Entity target, StringBuilder _)
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
                    return;
            }
        }
    }
}
