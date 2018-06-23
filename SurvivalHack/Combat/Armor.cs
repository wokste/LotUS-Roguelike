using System;
using System.Collections.Generic;
using System.Linq;
using SurvivalHack.ECM;
using HackConsole;

namespace SurvivalHack.Combat
{
    class Blockable : ECM.IComponent
    {
        public int Priority { get; set; } = 100;

        public float BlockChance;
        public EAttackState BlockMethod;

        public Blockable(float blockChance, EAttackState blockMethod)
        {
            BlockChance = blockChance;
            BlockMethod = blockMethod;
        }

        public IEnumerable<UseFunc> GetActions(Entity self, BaseEvent message, EUseSource source)
        {
            if (message is AttackEvent && (source == EUseSource.Target || source == EUseSource.TargetItem))
                yield return new UseFunc(Mutate, EUseOrder.PreEvent);
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

    class Armour : ECM.IComponent
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

        public IEnumerable<UseFunc> GetActions(Entity self, BaseEvent message, EUseSource source)
        {
            if (message is DamageEvent && (source == EUseSource.Target || source == EUseSource.TargetItem))
                yield return new UseFunc(Mutate, EUseOrder.PreEvent);
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

    internal class ElementalResistance : IComponent
    {
        private readonly EDamageType DamageType;
        private readonly float Mult;

        public ElementalResistance(EDamageType damageType, float mult)
        {
            DamageType = damageType;
            Mult = mult;
        }

        public string Describe() => $"Reduces all {DamageType} damage by {Mult}";

        public IEnumerable<UseFunc> GetActions(Entity self, BaseEvent message, EUseSource source)
        {
            if (message is DamageEvent && (source == EUseSource.Target || source == EUseSource.TargetItem))
                yield return new UseFunc(Mutate, EUseOrder.PreEvent);
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
