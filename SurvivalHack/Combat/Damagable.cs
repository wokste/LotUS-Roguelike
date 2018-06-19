using System.Collections.Generic;
using System.Linq;
using HackConsole;
using SurvivalHack.ECM;

namespace SurvivalHack.Combat
{
    class Damagable : ECM.IComponent
    {
        public Bar Health;

        public Damagable(int HP)
        {
            Health = new Bar(HP);
        }


        public IEnumerable<UseFunc> GetActions(Entity self, BaseEvent message, EUseSource source)
        {
            if (message is DamageEvent && source == EUseSource.Target)
                yield return new UseFunc(TakeDamage);

            if (message is HealEvent && source == EUseSource.Target)
                yield return new UseFunc(Heal);
        }

        public void TakeDamage(BaseEvent msg)
        {
            var attack = (DamageEvent)msg;

            var damage = attack.Damage;

            if (damage <= 0)
                return;

            Health.Current -= damage;

            if (Health.Current <= 0)
            {
                msg.Target.Destroy();
                attack.KillHit = true;
            }
        }

        public void Heal(BaseEvent msg)
        {
            var heal = (HealEvent)msg;

            if (Health.Current == Health.Max)
                return;

            Health.Current += heal.Restore;
            return;
        }

        public string Describe() => null;
    }
}
