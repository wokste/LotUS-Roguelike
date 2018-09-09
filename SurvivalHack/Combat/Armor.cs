using SurvivalHack.ECM;

namespace SurvivalHack.Combat
{
    public class Blockable : Component
    {
        public int Priority { get; set; } = 100;

        public float BlockChance;
        public EAttackState BlockMethod;

        public Blockable(float blockChance, EAttackState blockMethod)
        {
            BlockChance = blockChance;
            BlockMethod = blockMethod;
        }

        public override void GetActions(Entity self, BaseEvent message, EUseSource source)
        {
            if (message is AttackEvent && (source == EUseSource.Target || source == EUseSource.TargetItem))
                message.PreEvent += Mutate;
        }

        public void Mutate(BaseEvent msg)
        {
            AttackEvent attack = (AttackEvent)msg;

            if (attack.State != EAttackState.Hit)
                return;

            if (Game.Rnd.NextDouble() > BlockChance)
                return;

            attack.State = BlockMethod;
            return;
        }

        public override string Describe()
        {
            return $"Has a {BlockChance:%} to block incoming attacks";
        }
    }

    public class Armour : IComponent
    {
        public int Priority { get; set; } = 50;

        public float CritChance = 0.02f;
        public int DamageReduction = 1;
        public EDamageLocation ProtectLocation;

        public Armour(EDamageLocation protectLocation, int damageReduction, float critChance)
        {
            ProtectLocation = protectLocation;
            DamageReduction = damageReduction;
            CritChance = critChance;
        }

        public void GetActions(Entity self, BaseEvent message, EUseSource source)
        {
            if (message is DamageEvent && (source == EUseSource.Target || source == EUseSource.TargetItem))
                message.PreEvent += Mutate;
        }

        public void Mutate(BaseEvent msg)
        {
            var attack = (DamageEvent)msg;

            if ((attack.Location & ProtectLocation) == 0)
                return;

            if (Game.Rnd.NextDouble() < CritChance)
            {
                return;
            }

            attack.Modifiers.Add((-DamageReduction, "armor"));
            return;
        }

        public string Describe()
        {
            return $"Reduces damage by {DamageReduction}.";
        }

        public bool FitsIn(ESlotType type)
        {

            switch (type)
            {
                case ESlotType.Head:
                    return (ProtectLocation & EDamageLocation.Head) != 0;
                case ESlotType.Body:
                    return (ProtectLocation & EDamageLocation.AllBody) != 0;
                case ESlotType.Gloves:
                    return (ProtectLocation & EDamageLocation.Hands) != 0;
                case ESlotType.Feet:
                    return (ProtectLocation & EDamageLocation.Feet) != 0;
                default:
                    return false;
            }
        }
    }

    public class ElementalResistance : Component
    {
        private readonly EDamageType DamageType;
        private readonly float Mult;

        public ElementalResistance(EDamageType damageType, float mult)
        {
            DamageType = damageType;
            Mult = mult;
        }

        public override string Describe() => $"Reduces all {DamageType} damage by {Mult}";

        public override void GetActions(Entity self, BaseEvent message, EUseSource source)
        {
            if (message is DamageEvent && (source == EUseSource.Target || source == EUseSource.TargetItem))
                message.PreEvent += Mutate;
        }

        public void Mutate(BaseEvent msg)
        {
            var attack = (DamageEvent)msg;

            //TODO: Fix
            attack.Modifiers.Add((-(int)(Mult * 10), "elemental resistance"));
            return;
        }
    }
}
