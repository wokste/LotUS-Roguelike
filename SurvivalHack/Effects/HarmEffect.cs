using SurvivalHack.Combat;
using System.Text;
using System.Xml.Serialization;

namespace SurvivalHack.Effects
{
    public class HarmEffect : Effect
    {
        [XmlAttribute]
        public int Damage { get; set; }
        [XmlAttribute]
        public int StatID { get; set; }

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
        public override void Use(Entity instignator, Entity target, StringBuilder sb)
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

        public override float Efficiency(Entity instignator, Entity target)
        {
            return 1;
        }
    }
}
