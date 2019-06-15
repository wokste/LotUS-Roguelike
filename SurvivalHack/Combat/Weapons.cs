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
        IEnumerable<Entity> Targets(Entity attacker, Vec dir);
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

    public class SweepWeapon : IWeapon, IEquippableComponent
    {
        public Damage Damage { get; }
        public int MinRange { get; } = 1;
        public int MaxRange { get; } = 1;

        public SweepWeapon(Damage damage, int minRange = 1, int maxRange = 1 )
        {
            Damage = damage;
        }

        public Vec? Dir(Entity attacker, Entity defender)
        {
            Vec delta = (defender.Pos - attacker.Pos);

            if (delta.ManhattanLength > MaxRange)
                return null;

            return delta.Clamped;
        }

        public IEnumerable<Entity> Targets(Entity attacker, Vec _)
        {
            var level = attacker.Level;
            var center = attacker.Pos;

            return level.GetEntities(center.BoundingBox.Grow(MaxRange)).Where(e => (e.Pos - center).ManhattanLength >= MinRange);
        }

        public ESlotType SlotType => ESlotType.Hand;
    }

    public class RangedWeapon : IWeapon, IEquippableComponent
    {
        public Damage Damage { get; }
        public float Range;

        public RangedWeapon(int damage, EDamageType type, float range)
        {
            Damage = new Damage(damage,type, EAttackMove.Projectile);
            Range = range;
        }

        public RangedWeapon(Damage damage, float range)
        {
            Damage = damage;
            Range = range;
        }

        //TODO: Fix ranged weapons
        
        public Vec? Dir(Entity attacker, Entity defender)
        {
            Vec delta = (defender.Pos - attacker.Pos);

            return new Vec(MyMath.Clamp(delta.X, -1, 1), MyMath.Clamp(delta.Y, -1, 1));
        }

        public IEnumerable<Entity> Targets(Entity attacker, Vec dir)
        {
            var level = attacker.Level;
            var pos = attacker.Pos;

            while (!level.GetTile(pos).Solid)
            {
                pos += dir;
                var targets = level.GetEntities(pos);

                foreach (var target in targets)
                    yield return target;

                // TODO: Single-hit bullets. This works great for lightning bolt but that isn't really the idea here.
            }
        }

        public ESlotType SlotType => ESlotType.Ranged;
    }
}