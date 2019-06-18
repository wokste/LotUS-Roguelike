using SurvivalHack.Combat;
using System.Text;
using System.Xml.Serialization;
using static SurvivalHack.Combat.StatBlock;

namespace SurvivalHack.Effects
{
    [XmlType("Heal")]
    public class HealEffect : IEffect
    {
        [XmlAttribute]
        public int Restore { get; set; }

        [XmlAttribute]
        public EStat Stat { get; set; }

        [XmlAttribute]
        public TargetFilter Filter { get; set; }

        public HealEffect()
        {
        }

        public HealEffect(int restore, EStat stat, TargetFilter useOn)
        {
            Restore = restore;
            Stat = stat;
            Filter = useOn;
        }
        
        public void Use(Entity instignator, Entity target, StringBuilder sb)
        {
            var stats = target.GetOne<StatBlock>();

            stats.Heal(Restore, Stat);
            sb?.Append($"{Word.AName(target)} {Word.Verb(target, "heal")} {Restore} {Stat.ToString()}. "); // TODO: not always HP
        }

        public float Efficiency(Entity instignator, Entity target)
        {
            return 1; // TODO
        }
    }
}
