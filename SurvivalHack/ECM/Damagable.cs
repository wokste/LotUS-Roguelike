using System;
using HackConsole;

namespace SurvivalHack.ECM
{
    class Damagable : IComponent
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
    }
}
