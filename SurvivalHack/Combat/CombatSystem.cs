using HackConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalHack.Combat
{
    static class CombatSystem
    {
        public static void DoAttack(Entity attacker, IEnumerable<Entity> defenders, (Entity, IWeapon) weaponPair)
        {
            var sb = new StringBuilder();
            foreach (var target in defenders)
            {
                DoAttack(attacker, target, weaponPair, sb);
            }
            ColoredString.OnMessage(sb.ToString());
        }

        public static EAttackResult DoAttack(Entity attacker, Entity defender, (Entity, IWeapon) weaponPair, StringBuilder sb)
        {
            // TODO: Calculate hit/miss

            // TODO: Increase damage based on items the player is wearing.

            Damage damageCopy = weaponPair.Item2.Damage;
            var ret = DoDamage(defender, ref damageCopy, sb);
            ColoredString.OnMessage(sb.ToString());
            return ret;
        }

        public static bool IsAHit(this EAttackResult res)
        {
            return res == EAttackResult.HitNoDamage || res == EAttackResult.HitDamage || res == EAttackResult.HitKill;
        }

        public static EAttackResult DoDamage(Entity target, ref Damage dmg, StringBuilder sb)
        {
            // TODO: Reduce damage based on armour etc.

            // TODO: Target takes damage.
            var stats = target.GetOne<StatBlock>();
            stats.TakeDamage(ref dmg);

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
        public int HitChance;
        public int CritChance;
        public EAttackMove AttackMove;
    }

    public struct Damage
    {
        public float Dmg;
        public EDamageType DamageType;
        public EAttackMove AttackMove;
        public bool KillHit;

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
