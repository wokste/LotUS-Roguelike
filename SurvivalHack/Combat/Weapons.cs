using HackConsole.Algo;
using SurvivalHack.ECM;

namespace SurvivalHack.Combat
{
    public interface IWeapon : IComponent
    {
        bool InRange(Entity attacker, Entity defender);
        float WeaponPriority { get; }
        EAttackMove AttackMove { get; }
    }

    public class MeleeWeapon : IWeapon
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
            if (attack.State == EAttackState.Hit)
            {
                Eventing.On(new DamageEvent(attack, (int)(Damage * (0.5 + Game.Rnd.NextDouble())), DamageType, attack.Location), msg);
            }
        }

        public string Describe() => $"Melee attack deals {Damage} {DamageType} damage";

        public bool FitsIn(ESlotType type) => type == ESlotType.Hand;
    }

    public class RangedWeapon : IWeapon
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
            if (attack.State == EAttackState.Hit)
            {
                Eventing.On(new DamageEvent(attack, (int)(Damage * (0.5 + Game.Rnd.NextDouble())), DamageType, attack.Location), msg);
            }
        }

        public string Describe() => $"Ranged attack deals {Damage} {DamageType} damage";
        public bool FitsIn(ESlotType type) => type == ESlotType.Ranged;
    }
}