using System;
using System.Collections.Generic;
using System.Linq;
using HackConsole;

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

        internal void Heal(int restore, int statID)
        {
            Health.Current += restore;
        }

        public string Describe() => null;
        public bool Use(Entity user, Entity item, Entity target, ECM.EUseMessage filter) => false;
    }
}
