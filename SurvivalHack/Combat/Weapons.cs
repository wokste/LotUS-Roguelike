﻿using HackConsole;
using SurvivalHack.ECM;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace SurvivalHack.Combat
{
    public interface IWeapon : IComponent
    {
        [XmlAttribute]
        EAttackMove AttackMove { get; set; }
        Damage Damage { get; }
        Vec? Dir(Entity attacker, Entity defender);
        IEnumerable<Entity> Targets(Entity attacker, Vec dir);
    }

    public class MeleeWeapon : IWeapon, IEquippableComponent
    {
        [XmlAttribute]
        public EAttackMove AttackMove { get; set; }
        [XmlIgnore]
        public Damage Damage { get; private set; }

        [XmlAttribute("Damage")]
        public string DamageString { get { return Damage.ToString(); } set { Damage = new Damage(value); } }


        public MeleeWeapon()
        {
        }


        public MeleeWeapon(EAttackMove move, Damage damage)
        {
            AttackMove = move;
            Damage = damage;
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
        [XmlAttribute]
        public EAttackMove AttackMove { get; set; }
        [XmlIgnore]
        public Damage Damage { get; private set; }

        [XmlAttribute("Damage")]
        public string DamageString { get { return Damage.ToString(); } set { Damage = new Damage(value); } }

        [XmlAttribute]
        public Range Range { get; } = new Range(1,1);


        public SweepWeapon()
        {
        }

        public SweepWeapon(EAttackMove move, Damage damage, Range range)
        {
            AttackMove = move;
            Damage = damage;
            Range = range;
        }

        public Vec? Dir(Entity attacker, Entity defender)
        {
            Vec delta = (defender.Pos - attacker.Pos);

            if (!Range.Contains(delta.ManhattanLength))
                return null;

            return delta.Clamped;
        }

        public IEnumerable<Entity> Targets(Entity attacker, Vec _)
        {
            var level = attacker.Level;
            var center = attacker.Pos;

            return level.GetEntities(center.BoundingBox.Grow(Range.Max)).Where(e => Range.Contains((e.Pos - center).ManhattanLength));
        }

        public ESlotType SlotType => ESlotType.Hand;
    }

    public class RangedWeapon : IWeapon, IEquippableComponent
    {

        [XmlAttribute]
        public EAttackMove AttackMove { get; set; } = EAttackMove.Projectile;

        [XmlIgnore]
        public Damage Damage { get; private set; }

        [XmlAttribute("Damage")]
        public string DamageString { get { return Damage.ToString(); } set { Damage = new Damage(value); } }

        [XmlAttribute]
        public float Range { get; set; }


        public RangedWeapon()
        {
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

            return delta.Clamped;
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