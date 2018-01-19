using System;

namespace SurvivalHack
{
    public static class Dicebag
    {
        // TODO: This should be ThreadLocal<Random>()
        private static readonly Random Rnd = new Random();

        public static int UniformInt()
        {
            return Rnd.Next();
        }

        public static int UniformInt(int max)
        {
            return Rnd.Next(max);
        }

        public static int UniformInt(int min, int max)
        {
            return Rnd.Next(min, max);
        }

        public static int Randomize(int mean)
        {
            var var = (double)(mean);
            var r = Rnd.NextDouble();

            return mean + (int)Math.Round((r - 0.5) * var);
        }
    }
}
