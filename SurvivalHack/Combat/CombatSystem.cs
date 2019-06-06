using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalHack.Combat
{
    static class CombatSystem
    {
        public static EAttackResult DoAttack(Entity attacker, Entity defender, Entity weapon, StringBuilder sb)
        {

            // TODO: Increase damage based on items the player is wearing.
            return EAttackResult.HitKill;
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

    public struct Damage
    {
        public float Dmg;
        public EDamageType DamageType;
        public bool KillHit;

        public bool Significant => (Dmg > 0);

        public Damage(float damage, EDamageType damageType)
        {
            Dmg = damage;
            DamageType = damageType;
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
