using System;
using System.Collections.Generic;
using System.Linq;
using HackConsole;
using SurvivalHack.ECM;

namespace SurvivalHack.Combat
{
    public class Damagable : Component
    {
        public Bar Health;

        public Damagable(int HP)
        {
            Health = new Bar(HP);
        }

        public override void GetActions(Entity self, BaseEvent message, EUseSource source)
        {
            if (message is DamageEvent && source == EUseSource.Target)
                message.OnEvent += TakeDamage;

            if (message is HealEvent && source == EUseSource.Target)
                message.OnEvent += Heal;
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

            UpdateStats(msg.Target);
        }

        public void Heal(BaseEvent msg)
        {
            var heal = (HealEvent)msg;

            if (Health.Current == Health.Max)
                return;

            Health.Current += heal.Restore;
            UpdateStats(msg.Target);
            return;
        }

        private void UpdateStats(Entity self)
        {
            if (self.EntityFlags.HasFlag(EEntityFlag.IsPlayer))
            {
                var p = Health.Perc * 3;
                byte r = 255;
                byte g = (byte)MyMath.Lerp(p, 0, 255);
                byte b = (byte)MyMath.Lerp(p - 1, 0, 255);

                self.Symbol.TextColor = new Color(r, g, b);
            }
        }
    }
}
