using HackConsole;
using SurvivalHack.ECM;
using System.Xml.Serialization;

namespace SurvivalHack.Combat
{
    public class StatBlock : IComponent
    {
        [XmlElement]
        public Stat[] _stats = new Stat[3];


        public StatBlock()
        {
        }

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
        
        public object Cur(EStat stat) => _stats[(int)stat].Cur;

        internal bool Spend(int cost, EStat stat)
        {
            if (_stats[(int)stat].Cur < cost)
                return false;

            _stats[(int)stat].Cur -= cost;
            return true;
        }

        public object Max(EStat stat) => _stats[(int)stat].Max;

        public float Perc(EStat stat) => _stats[(int)stat].Perc;

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

        public int Heal(int restore, EStat statID)
        {
            var change = _stats[(int)statID].Add(restore);

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
            [XmlAttribute]
            internal int Cur { get; set; }

            [XmlAttribute]
            internal int Max { get; set; }

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

    public enum EStat
    {
        HP,
        MP
    }
}
