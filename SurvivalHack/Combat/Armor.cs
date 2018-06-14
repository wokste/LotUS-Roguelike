﻿using System;
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

        public IEnumerable<UseFunc> GetActions(BaseEvent message, EUseSource source)
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

            Message.Write($"{attack.Target} blocks {attack.Self}s attack", attack.Target.Move.Pos, Color.Cyan);

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

        public Armour(int damageReduction, float critChance)
        {
            DamageReduction = damageReduction;
            CritChance = critChance;
        }

        public IEnumerable<UseFunc> GetActions(BaseEvent message, EUseSource source)
        {
            if (message is DamageEvent && (source == EUseSource.Target || source == EUseSource.TargetItem))
                yield return new UseFunc(Mutate, EUseOrder.PreEvent);
        }

        public void Mutate(BaseEvent msg)
        {
            var attack = (DamageEvent)msg;

            // TODO: Armour should protect only a part of the body.

            if (Game.Rnd.NextDouble() < CritChance)
            {
                // TODO: Crit messages.
                return;
            }

            var newDamage = Math.Max(attack.Damage - DamageReduction, 0);
            Message.Write($"{attack.Target}'s armour reduces the damage of the incoming attack from {attack.Damage} to {newDamage}", attack.Target.Move.Pos, Color.Cyan);
            attack.Damage = newDamage;

            return;
        }

        public string Describe()
        {
            return $"Reduces damage by {DamageReduction}.";
        }
    }
}
