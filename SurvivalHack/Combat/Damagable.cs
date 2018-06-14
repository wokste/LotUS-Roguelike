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


        public IEnumerable<UseFunc> GetActions(UseMessage message, EUseSource source)
        {
            if (message is DamageMessage && source == EUseSource.Target)
                yield return new UseFunc(TakeDamage);
        }

        public void TakeDamage(UseMessage msg)
        {
            var attack = (DamageMessage)msg;

            if (!attack.Significant)
                return;

            Health.Current -= attack.Damage;

            if (Health.Current == 0)
            {
                msg.Target.Destroy();

                Message.Write($"{msg.Target.Name} died", msg.Target.Move?.Pos, Color.Red);
            }
        }

        internal bool Heal(int restore, int statID)
        {
            if (Health.Current == Health.Max)
                return false;

            Health.Current += restore;
            return true;
        }

        public string Describe() => null;
    }
}
