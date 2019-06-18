using SurvivalHack.Combat;
using System.Text;
using System.Xml.Serialization;

namespace SurvivalHack.Effects
{
    [XmlType("Harm")]
    public class HarmEffect : IEffect
    {
        [XmlAttribute]
        public int Damage { get; set; }
        [XmlAttribute]
        public EStat Stat { get; set; }
        [XmlAttribute]
        public TargetFilter Filter { get; set; }

        [XmlAttribute]
        public EDamageType DamageType { get; set; }

        public HarmEffect()
        {
        }

        public HarmEffect(int damage, EDamageType damageType, EStat stat, TargetFilter useOn)
        {
            Damage = damage;
            DamageType = damageType;
            Stat = stat;
            Filter = useOn;
        }
        public void Use(Entity instignator, Entity target, StringBuilder sb)
        {
            CombatSystem.RollAttack(instignator, new[] { target }, (null,null));
        }

        public float Efficiency(Entity instignator, Entity target)
        {
            return 1;
        }
    }
}
