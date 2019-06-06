using System;
using HackConsole;
using SurvivalHack.ECM;

namespace SurvivalHack.Combat
{
    public class StatBlock : IComponent
    {
        private readonly Stat[] _stats = new Stat[3];
        private readonly int _level = 0;

        public StatBlock(int HP, int MP, int XP)
        {
            _stats[0] = new Stat(HP, 1);
            _stats[1] = new Stat(MP, 1);
            _stats[2] = new Stat(XP, 1);

            for (int i = 0; i < _stats.Length; ++i)
            {
                _stats[i].Set(int.MaxValue, _level); // Will be clamped anyway.
            }
        }
        
        public object Cur(int statID) => _stats[statID].Cur;

        public object Max(int statID) => _stats[statID].Max(_level);

        public float Perc(int statID) => _stats[statID].Perc(_level);

        public void TakeDamage(ref Damage dmg)
        {
            var statID = 0; // TODO: attacks on different stats

            if (dmg.Dmg <= 0)
                return;

            var change = _stats[statID].Add(-1 * (int)dmg.Dmg, _level);

            if (change == 0)
                return;

            if (_stats[statID].Cur <= 0)
            {
                dmg.KillHit = true;
            }
        }

        public int Heal(int restore, int statID)
        {
            var change = _stats[statID].Add(restore, _level);

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
