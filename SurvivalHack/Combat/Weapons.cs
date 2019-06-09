using HackConsole;
using HackConsole.Algo;
using SurvivalHack.ECM;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SurvivalHack.Combat
{
    public interface IWeapon : IComponent
    {
        Damage Damage { get; }
        Vec? Dir(Entity attacker, Entity defender);
        IEnumerable<Entity> Targets(Entity self, Vec value);
    }

    public class MeleeWeapon : IWeapon, IEquippableComponent
    {
        public Damage Damage { get; }

        public MeleeWeapon(Damage damage)
        {
            Damage = damage;
        }


        public MeleeWeapon(float damage, EAttackMove move, EDamageType type)
        {
            Damage = new Damage(damage, type, move);
        }

        public Vec? Dir(Entity attacker, Entity defender)
        {
            Vec delta = (defender.Pos - attacker.Pos);

            if (delta.ManhattanLength > 1)
                return null;

            return delta;
        }

        public IEnumerable<Entity> Targets(Entity attacker, Vec dir)
        {
            var level = attacker.Level;
            return level.GetEntities(attacker.Pos + dir);
        }

        public ESlotType SlotType => ESlotType.Hand;
    }

    public class RangedWeapon : MeleeWeapon// IWeapon, IEquippableComponent
    {
        //public Damage Damage;
        public float Range;

        public RangedWeapon(int damage, EDamageType type, float range) : base(new Damage(damage, type, EAttackMove.Projectile))
        {
            //Damage = damage;
            Range = range;
        }

        public RangedWeapon(Damage damage, float range) : base(damage)
        {
            //Damage = damage;
            Range = range;
        }

        //TODO: Fix ranged weapons
        /*
        public bool CanAttack(Entity attacker, Entity defender)
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
                var damageCopy = Damage;
                CombatSystem.DoDamage(attack.Target, ref damageCopy, sb);
                ColoredString.OnMessage(sb.ToString());
            }
        }

        public ESlotType SlotType => ESlotType.Ranged;
        */
    }
}