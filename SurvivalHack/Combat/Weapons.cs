using HackConsole;
using HackConsole.Algo;
using SurvivalHack.ECM;
using System.Text;

namespace SurvivalHack.Combat
{
    public interface IWeapon : IActionComponent
    {
        bool InRange(Entity attacker, Entity defender);
        float WeaponPriority { get; }
        EAttackMove AttackMove { get; }
    }

    public class MeleeWeapon : IWeapon, IEquippableComponent
    {
        public float Damage;
        public EAttackMove AttackMove { get; }
        public EDamageType DamageType;

        public float WeaponPriority { get; set; }

        public MeleeWeapon(float damage, EAttackMove attackMove, EDamageType damageType)
        {
            Damage = damage;
            AttackMove = attackMove;
            DamageType = damageType;
        }

        public bool InRange(Entity attacker, Entity defender)
        {
            return (attacker.Pos - defender.Pos).ManhattanLength <= 1;
        }

        public void GetActions(Entity self, BaseEvent message, EUseSource source)
        {
            if (message is AttackEvent && (source == EUseSource.Item))
                message.OnEvent += ToHitRoll;
        }

        private void ToHitRoll(BaseEvent msg)
        {
            var attack = (AttackEvent)msg;
            if (attack.State.IsAHit())
            {
                var sb = new StringBuilder();
                var damage = new Damage(Damage, DamageType);
                CombatSystem.DoDamage(attack.Target, ref damage, sb);
                ColoredString.OnMessage(sb.ToString());
            }
        }

        public ESlotType SlotType => ESlotType.Hand;
    }

    public class RangedWeapon : IWeapon, IEquippableComponent
    {
        public float Damage;
        public EAttackMove AttackMove => EAttackMove.Projectile;
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
            var level = attacker.Level;
            var path = Line.Run(attacker.Pos, defender.Pos);
            foreach (var v in path)
                if (level.GetTile(v).BlockSight)
                    return false;

            return true;
        }

        public void GetActions(Entity self, BaseEvent message, EUseSource source)
        {
            if (message is AttackEvent && (source == EUseSource.Item))
                message.OnEvent += ToHitRoll;
        }

        private void ToHitRoll(BaseEvent msg)
        {
            var attack = (AttackEvent)msg;
            if (attack.State.IsAHit())
            {
                var sb = new StringBuilder();
                var damage = new Damage(Damage, DamageType);
                CombatSystem.DoDamage(attack.Target, ref damage, sb);
                ColoredString.OnMessage(sb.ToString());
            }
        }

        public ESlotType SlotType => ESlotType.Ranged;
    }
}