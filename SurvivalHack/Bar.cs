using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalHack
{
    struct Bar
    {
        private int _current;
        private int _max;

        public Bar(int val) : this()
        {
            _current = val;
            _max = val;
        }

        public int Current
        {
            get => _current;
            set => _current = Math.Min(value, _max);
        }

        public int Max
        {
            get => _max;
            set {_max = value; _current = Math.Min(_current, _max);}
        }

        public float Perc => (float) Current / (float) Max;

        public static Bar operator+ (Bar bar, int value)
        {
            return new Bar
            {
                _current = Math.Max(Math.Min(bar.Current + value, bar.Max),0),
                _max = bar.Max
            };
        }

    }
}
