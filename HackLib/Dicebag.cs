using System;

namespace HackLib
{
    public static class Dicebag
    {
        // TODO: This should be ThreadLocal<Random>()
        private static readonly Random _rnd = new Random();

        public static int UniformInt()
        {
            return _rnd.Next();
        }

        public static int UniformInt(int max)
        {
            return _rnd.Next(max);
        }

        public static int UniformInt(int min, int max)
        {
            return _rnd.Next(min, max);
        }
    }
}
