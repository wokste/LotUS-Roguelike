using HackConsole;
using SurvivalHack.ECM;

namespace SurvivalHack.Combat
{
    public class Damagable : Component
    {
        public Stat[] Stats = new Stat[3];

        public Damagable(int HP, int Energy, int MP)
        {
            Health = new Stat(HP);
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
            // TODO: Show being damaged in some way
            /*
            if (self.EntityFlags.HasFlag(EEntityFlag.IsPlayer))
            {
                var p = Health.Perc * 3;
                byte r = 255;
                byte g = (byte)MyMath.Lerp(p, 0, 255);
                byte b = (byte)MyMath.Lerp(p - 1, 0, 255);

                self.Symbol.TextColor = new Colour(r, g, b);
            }
            */
            
        }

        public struct Stat
        {
            private readonly float _inc;
            private readonly float _base;
            public int Cur { get; private set; }

            public Stat(float baseVal, float incVal) : this()
            {
                _inc = incVal;
                _base = baseVal;
            }

            public int Max(int level) => (int)(_base + _inc * level);

            public float Perc(int level) => Cur / (float)Max(level);

            public int Add(int val, int level) => Set(Cur + val, level);

            public int Set(int val, int level)
            {
                Cur = MyMath.Clamp(val, 0, Max(level));
                return val - Cur;
            }

            public void LevelUp(int oldlevel, int newLevel)
            {
                Set(Cur + Max(newLevel) - Max(oldlevel), newLevel);
            }
        }
    }
}
