using System;
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

        public void TakeDamage(Entity self, Attack attack)
        {
            Health.Current -= attack.Damage;

            if (Health.Current == 0)
            {
                self.Destroy();

                Message.Write($"{self.Name} died", self.Move?.Pos, Color.Red);
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
        public IEnumerable<UseFunc> GetActions(EUseMessage filter, EUseSource source) => Enumerable.Empty<UseFunc>();
    }
}
