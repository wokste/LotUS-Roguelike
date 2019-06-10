using SurvivalHack.ECM;

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

        public float BlockChance;
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

        public ESlotType SlotType { get; private set; }

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

        private readonly EDamageType DamageType;
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
