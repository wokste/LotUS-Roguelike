using SurvivalHack.Combat;
using System.Text;

namespace SurvivalHack.Effects
{
    class HarmEffect : IEntityEffect
    {
        public int Damage;
        public int StatID;
        public EDamageType DamageType;

        public EntityTarget UseOn { get; }

        public HarmEffect(int damage, EDamageType damageType, int statID, EntityTarget useOn)
        {
            Damage = damage;
            DamageType = damageType;
            StatID = statID;
            UseOn = useOn;
        }
        public bool Use(Entity instignator, Entity target, StringBuilder sb)
        {
            /*
            if (target.GetOne<StatBlock>() is StatBlock stats)
            {
                // TODO: Armor.
                //stats.TakeDamage();
            }
            throw new NotImplementedException();
            */
            return true;
        }

        public float Efficiency(Entity instignator, Entity target)
        {
            return 1;
        }
    }
}
