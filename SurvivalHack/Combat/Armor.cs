using System;
using System.Collections.Generic;
using System.Linq;
using SurvivalHack.ECM;
using HackConsole;

namespace SurvivalHack.Combat
{
    public class Blockable : ECM.IComponent
    {
        public int Priority { get; set; } = 100;

        public float BlockChance;
        public EAttackState BlockMethod;

        public Blockable(float blockChance, EAttackState blockMethod)
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

            if (attack.State != EAttackState.Hit)
                return;

            if (Game.Rnd.NextDouble() > BlockChance)
                return;

            attack.State = BlockMethod;
            return;
        }

        public string Describe()
        {
            return $"Has a {BlockChance:%} to block incoming attacks";
        }
    }

    public class Armour : ECM.IComponent
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
    }

    public class ElementalResistance : IComponent
    {
        private readonly EDamageType DamageType;
        private readonly float Mult;

        public ElementalResistance(EDamageType damageType, float mult)
        {
            DamageType = damageType;
            Mult = mult;
        }

        public string Describe() => $"Reduces all {DamageType} damage by {Mult}";

        public void GetActions(Entity self, BaseEvent message, EUseSource source)
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
