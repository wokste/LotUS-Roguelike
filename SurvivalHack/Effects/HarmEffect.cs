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
        public int StatID { get; set; }
        [XmlAttribute]
        public EntityTarget UseOn { get; set; }

        [XmlAttribute]
        public EDamageType DamageType { get; set; }

        public HarmEffect()
        {
        }

        public HarmEffect(int damage, EDamageType damageType, int statID, EntityTarget useOn)
        {
            Damage = damage;
            DamageType = damageType;
            StatID = statID;
            UseOn = useOn;
        }
        public void Use(Entity instignator, Entity target, StringBuilder sb)
        {
            /*
            if (target.GetOne<StatBlock>() is StatBlock stats)
            {
                // TODO: Armor.
                //stats.TakeDamage();
            }
            throw new NotImplementedException();
            */
        }

        public float Efficiency(Entity instignator, Entity target)
        {
            return 1;
        }
    }
}
