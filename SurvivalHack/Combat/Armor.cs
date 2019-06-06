using SurvivalHack.ECM;

namespace SurvivalHack.Combat
{
    interface IArmorComponent : IComponent {

    }

    public class Blockable : IActionComponent
    {
        public int Priority { get; set; } = 100;

        public float BlockChance;
        public EAttackResult BlockMethod;

        public Blockable(float blockChance, EAttackResult blockMethod)
        {
            BlockChance = blockChance;
            BlockMethod = blockMethod;
        }

        public void GetActions(Entity self, BaseEvent message, EUseSource source)
        {
            if (message is AttackEvent && (source == EUseSource.Target || source == EUseSource.TargetItem))
                message.PreEvent += Mutate;
        }

        public void Mutate(BaseEvent msg)
        {
            AttackEvent attack = (AttackEvent)msg;

            if (!attack.State.IsAHit())
                return;

            if (Game.Rnd.NextDouble() > BlockChance)
                return;

            attack.State = BlockMethod;
            return;
        }
    }

    public class Armor : IArmorComponent, IEquippableComponent
    {
        public int Priority { get; set; } = 50;

        public ESlotType SlotType { get; private set; }

        public int DamageReduction = 1;

        public Armor(int damageReduction, ESlotType slotType)
        {
            DamageReduction = damageReduction;
            SlotType = slotType;
        }
        
        public void Mutate(ref Damage attack)
        {
            attack.Dmg -= DamageReduction;
            return;
        }
    }

    public class ElementalResistance : IArmorComponent
    {
        private readonly EDamageType DamageType;
        private readonly float Mult;

        public ElementalResistance(EDamageType damageType, float mult)
        {
            DamageType = damageType;
            Mult = mult;
        }
        
        public void Mutate(ref Damage attack)
        {
            if ((attack.DamageType & DamageType) == 0)
                return;

            attack.Dmg *= Mult;
            return;
        }
    }
}
