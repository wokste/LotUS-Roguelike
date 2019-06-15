using SurvivalHack.ECM;
using System.Xml.Serialization;

namespace SurvivalHack.Combat
{
    interface IArmorComponent : IComponent
    {
        int ArmorPriority { get; }

        void Mutate(ref Attack attack, ref Damage damage);
    }

    public class Blockable : IArmorComponent
    {
        public int ArmorPriority => 100;

        [XmlAttribute]
        public float BlockChance;

        [XmlAttribute]
        public EAttackResult BlockMethod;

        public Blockable(float blockChance, EAttackResult blockMethod)
        {
            BlockChance = blockChance;
            BlockMethod = blockMethod;
        }

        public void Mutate(ref Attack attack, ref Damage _)
        {
            attack.HitChance *= (1 - BlockChance);
        }
    }

    public class Armor : IArmorComponent, IEquippableComponent
    {
        public int ArmorPriority => 50;

        [XmlAttribute]
        public ESlotType SlotType { get; private set; }

        [XmlAttribute]
        public int DamageReduction = 1;

        public Armor(int damageReduction, ESlotType slotType)
        {
            DamageReduction = damageReduction;
            SlotType = slotType;
        }
        
        public void Mutate(ref Attack _, ref Damage damage)
        {
            damage.Dmg -= DamageReduction;
            return;
        }
    }

    public class ElementalResistance : IArmorComponent
    {
        public int ArmorPriority => 30;

        [XmlAttribute]
        private readonly EDamageType DamageType;

        [XmlAttribute]
        private readonly float Mult;

        public ElementalResistance(EDamageType damageType, float mult)
        {
            DamageType = damageType;
            Mult = mult;
        }
        
        public void Mutate(ref Attack _, ref Damage damage)
        {
            if ((damage.DamageType & DamageType) == 0)
                return;

            damage.Dmg *= Mult;
            return;
        }
    }
}
