using System;

namespace HackConsole
{
    public static class MyMath
    {
        public static int Clamp(int val, int min, int max)
        {
            return val < min ? min : val > max ? max : val;
        }

        public static int Lerp(float f, int v0, int v1)
        {
            return f < 0 ? v0 : f > 1 ? v1 : (int)Math.Round(v0 + f * (v1 - v0));
        }
    }
}
