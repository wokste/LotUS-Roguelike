using HackConsole;
using SurvivalHack.ECM;
using System;
using System.Xml.Serialization;

namespace SurvivalHack.Combat
{
    public class StatBlock : IComponent
    {
        [XmlIgnore]
        private readonly Stat[] _stats = new Stat[3];

        [XmlAttribute]
        public string HP { get { return _stats[0].ToString(); } set { _stats[0] = new Stat(value); } }

        [XmlAttribute]
        public string MP { get { return _stats[1].ToString(); } set { _stats[1] = new Stat(value); } }

        [XmlAttribute]
        public string XP { get { return _stats[2].ToString(); } set { _stats[2] = new Stat(value); } }

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

        public struct Stat
        {
            [XmlAttribute]
            public int Cur { get; set; }

            [XmlAttribute]
            public int Max { get; set; }

            public Stat(int val) : this()
            {
                Cur = val;
                Max = val;
            }

            public Stat(string value) : this()
            {
                throw new NotImplementedException();
            }

            public override string ToString() => (Cur == Max) ? $"{Max}" : $"{Cur}/{Max}";

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
