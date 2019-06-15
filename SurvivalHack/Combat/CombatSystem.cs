using HackConsole;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SurvivalHack.Combat
{
    static class CombatSystem
    {
        public static void RollAttack(Entity attacker, IEnumerable<Entity> defenders, (Entity, IWeapon) weaponPair)
        {
            var sb = new StringBuilder();
            foreach (var target in defenders)
            {
                if (target.GetOne<StatBlock>() != null)
                DoAttack(attacker, target, weaponPair, sb);
            }
            ColoredString.OnMessage(sb.ToString());
        }

        private static EAttackResult DoAttack(Entity attacker, Entity defender, (Entity, IWeapon) weaponPair, StringBuilder sb)
        {
            Attack attack = new Attack { HitChance = 0.7f, CritChance = 0.04f };
            Damage damageCopy = weaponPair.Item2.Damage;
            // TODO: Increase damage based on items the player is wearing.

            foreach (var armor in defender.GetNested<IArmorComponent>().OrderBy(a => -a.ArmorPriority)){
                armor.Mutate(ref attack, ref damageCopy);
            }

            var ret = DoDamage(defender, ref damageCopy, sb);
            ColoredString.OnMessage(sb.ToString());
            return ret;
        }

        private static EAttackResult DoDamage(Entity target, ref Damage dmg, StringBuilder sb)
        {   
            var stats = target.GetOne<StatBlock>();
            stats?.TakeDamage(ref dmg);

            EAttackResult res;
            if (dmg.Dmg > 0)
            {
                sb.Append($"{Word.AName(target)} {Word.Verb(target, "take")} @cd{dmg.Dmg}@ca damage");
                res = EAttackResult.HitDamage;
            }
            else
            {
                sb.Append($"{Word.AName(target)} {Word.Verb(target, "take")} @cfNo@ca damage");
                res = EAttackResult.HitNoDamage;
            }


            if (dmg.KillHit)
            {
                sb.Append($" killing {Word.It(target)}");
                target.Destroy();
            }
            sb.Append('.');
            return res;
        }
    }


    public struct Attack
    {
        [XmlAttribute]
        public float HitChance { get; set; }
        [XmlAttribute]
        public float CritChance { get; set; }
        [XmlAttribute]
        public EAttackMove AttackMove { get; set; }
    }

    public struct Damage
    {
        [XmlAttribute]
        public float Dmg { get; set; }
        [XmlAttribute]
        public EDamageType DamageType { get; set; }
        [XmlAttribute]
        public EAttackMove AttackMove { get; set; }
        public bool KillHit { get; set; }

        public bool Significant => (Dmg > 0);

        public Damage(float damage, EDamageType damageType, EAttackMove attackMove)
        {
            Dmg = damage;
            DamageType = damageType;
            AttackMove = attackMove;
            KillHit = false;
        }

        public override string ToString()
        {
            if (Dmg > 0)
                return $" @cd{Dmg}@ca damage";
            else
                return $" @cfNo@ca damage";
        }
    }
}
