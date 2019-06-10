using HackConsole;
using System;

namespace SurvivalHack.Mapgen
{
    public class PerlinNoise
    {
        public int Octaves = 4;
        public float Persistence = 0.5f;
        public float Scale = 10f;

        public readonly int Seed;

        public PerlinNoise(int seed)
        {
            Seed = seed;
        }

        private float Noise(int x, int y)
        {
            var n = x + y * 57 + Seed;
            n = (n << 13) ^ n;
            return (1.0f - ((n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0f);
        }

        private float NoiseSmooth(int x, int y)
        {
            return (Noise(x, y) + Noise(x, y + 1) + Noise(x + 1, y) + Noise(x + 1, y + 1)) / 2;
        }

        /// <summary>
        /// Function to linearly interpolate between a0 and a1
        /// </summary>
        /// <param name="a0">The return value if w = 0</param>
        /// <param name="a1">The return value if w = 1</param>
        /// <param name="w">Weight. Should be in the range [0.0, 1.0]</param>
        /// <returns>The interpolation value</returns>
        private float Lerp(float a0, float a1, float w)
        {
            return (1.0f - w) * a0 + w * a1;
        }

        private float InterpolatedNoise(float x, float y)
        {
            var xInt = (int)Math.Floor(x);
            var xFrac = x - xInt;

            var yInt = (int)Math.Floor(y);
            var yFrac = y - yInt;

            var v1 = NoiseSmooth(xInt, yInt);
            var v2 = NoiseSmooth(xInt + 1, yInt);
            var v3 = NoiseSmooth(xInt, yInt + 1);
            var v4 = NoiseSmooth(xInt + 1, yInt + 1);

            var i1 = Lerp(v1, v2, xFrac);
            var i2 = Lerp(v3, v4, xFrac);

            return Lerp(i1, i2, yFrac);
        }

        public float Get(Vec v)
        {
            var total = 0f;

            for (var i = 0; i < Octaves; i++)
            {
                var frequency = (float)Math.Pow(2, i) / Scale;
                var amplitude = (float)Math.Pow(Persistence, i);

                total += InterpolatedNoise(v.X * frequency, v.Y * frequency) * amplitude;
            }
            return total;
        }
    }
}
