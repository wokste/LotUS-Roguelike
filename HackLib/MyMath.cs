using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackLib
{
    static class MyMath
    {
        public static int Clamp(int val, int min, int max)
        {
            return val < min ? min : val > max ? max : val;
        }
    }
}
