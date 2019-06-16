using SurvivalHack.Combat;
using System.Text;
using System.Xml.Serialization;

namespace SurvivalHack.Effects
{
    [XmlType("Heal")]
    public class HealEffect : IEffect
    {
        [XmlAttribute]
        public int Restore { get; set; }

        [XmlAttribute]
        public int StatID { get; set; }

        [XmlAttribute]
        public EntityTarget UseOn { get; set; }

        public HealEffect()
        {
        }

        public HealEffect(int restore, int statID, EntityTarget useOn)
        {
            Restore = restore;
            StatID = statID;
            UseOn = useOn;
        }
        
        public void Use(Entity instignator, Entity target, StringBuilder sb)
        {
            var stats = target.GetOne<StatBlock>();

            stats.Heal(Restore, StatID);
            sb?.Append($"{Word.AName(target)} {Word.Verb(target, "heal")} {Restore} HP. "); // TODO: not always HP
        }

        public float Efficiency(Entity instignator, Entity target)
        {
            return 1; // TODO
        }
    }
}
