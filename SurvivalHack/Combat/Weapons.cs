using HackConsole;
using HackConsole.Algo;
using SurvivalHack.ECM;
using System.Collections.Generic;
using System.Linq;

namespace SurvivalHack.Combat
{
    interface IWeapon : IComponent
    {
        bool InRange(Entity attacker, Entity defender);
        float WeaponPriority { get; }
    }

    public class MeleeWeapon : IWeapon
    {
        public float Damage;
        public EDamageType DamageType;

        public float WeaponPriority { get; set; }

        public MeleeWeapon(float damage, EDamageType damageType)
        {
            Damage = damage;
            DamageType = damageType;
        }

        public bool InRange(Entity attacker, Entity defender)
        {
            return (attacker.Move.Pos - defender.Move.Pos).ManhattanLength <= 1;
        }

        public IEnumerable<UseFunc> GetActions(UseMessage message, EUseSource source)
        {
            if (message is AttackMessage && (source == EUseSource.This))
                yield return new UseFunc(ToHitRoll);
            if (message is DamageMessage && (source == EUseSource.This))
                yield return new UseFunc(DamageMessage);
        }

        private void ToHitRoll(UseMessage msg)
        {
            var attack = (AttackMessage)msg;
            if (attack.State == "hit")
            {
                msg.Self.Event(new DamageMessage(msg as AttackMessage, (int)(Damage * (0.5 + Game.Rnd.NextDouble())), DamageType));
            }
        }

        private void DamageMessage(UseMessage msg)
        {
            Message.Write($"{msg.Self.Name} attacks {msg.Target.Name} for {Damage} damage.", msg.Self?.Move?.Pos, Color.Orange);
        }

        public string Describe() => $"Melee attack deals {Damage} damage";
    }

    public class RangedWeapon : IWeapon
    {
        public float Damage;
        public EDamageType DamageType;
        public float Range;
        public float WeaponPriority { get; set; }

        public RangedWeapon(float damage, EDamageType damageType, float range)
        {
            Damage = damage;
            DamageType = damageType;
            Range = range;
        }

        public bool InRange(Entity attacker, Entity defender)
        {
            var level = attacker.Move.Level;
            var path = Line.Run(attacker.Move.Pos, defender.Move.Pos);
            foreach (var v in path)
                if (!level.HasFlag(v, TerrainFlag.Sight))
                    return false;

            return true;
        }

        public IEnumerable<UseFunc> GetActions(UseMessage message, EUseSource source)
        {
            if (message is AttackMessage && (source == EUseSource.This))
                yield return new UseFunc(ToHitRoll);
            if (message is DamageMessage && (source == EUseSource.This))
                yield return new UseFunc(DamageMessage);
        }

        private void ToHitRoll(UseMessage msg)
        {
            var attack = (AttackMessage)msg;
            if (attack.State == "hit")
            {
                msg.Self.Event(new DamageMessage(msg as AttackMessage, (int)(Damage * (0.5 + Game.Rnd.NextDouble())), DamageType));
            }
        }

        private void DamageMessage(UseMessage msg)
        {
            Message.Write($"{msg.Self.Name} attacks {msg.Target.Name} for {Damage} damage.", msg.Self?.Move?.Pos, Color.Orange);
        }

        public string Describe() => $"Ranged attack deals {Damage} damage";
        
    }
}