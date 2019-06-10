using System;
using HackConsole;
using SurvivalHack.ECM;

namespace SurvivalHack.Combat
{
    public class StatBlock : IComponent
    {
        private readonly Stat[] _stats = new Stat[3];

        public StatBlock(int HP, int MP, int XP)
        {
            _stats[0] = new Stat(HP);
            _stats[1] = new Stat(MP);
            _stats[2] = new Stat(XP);

            for (int i = 0; i < _stats.Length; ++i)
            {
                _stats[i].Set(int.MaxValue); // Will be clamped anyway.
            }
        }
        
        public object Cur(int statID) => _stats[statID].Cur;

        internal bool Spend(int cost, int statID)
        {
            if (_stats[statID].Cur < cost)
                return false;

            _stats[statID].Cur -= cost;
            return true;
        }

        public object Max(int statID) => _stats[statID].Max;

        public float Perc(int statID) => _stats[statID].Perc;

        public void TakeDamage(ref Damage dmg)
        {
            var statID = 0; // TODO: attacks on different stats

            if (dmg.Dmg <= 0)
                return;

            var change = _stats[statID].Add(-1 * (int)dmg.Dmg);

            if (change == 0)
                return;

            if (_stats[statID].Cur <= 0)
            {
                dmg.KillHit = true;
            }
        }

        public int Heal(int restore, int statID)
        {
            var change = _stats[statID].Add(restore);

            /*
            if (change > 0)
                UpdateStats(msg.Target);
            */
            return change;
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

        struct Stat
        {
            internal int Cur;
            internal int Max;

            public Stat(int val) : this()
            {
                Cur = val;
                Max = val;
            }


            public float Perc => Cur / (float)Max;

            public int Add(int val) => Set(Cur + val);

            public int Set(int val)
            {
                Cur = MyMath.Clamp(val, 0, Max);
                return val - Cur;
            }
        }
    }
}
