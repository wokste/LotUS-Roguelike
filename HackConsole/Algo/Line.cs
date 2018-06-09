using System;
using System.Collections.Generic;

namespace HackConsole.Algo
{
    public static class Line
    {
        public static IEnumerable<Vec> Run(Vec v0, Vec v1)
        {
            var cells = Math.Max(Math.Abs(v1.Y - v0.Y), Math.Abs(v1.X - v0.X));

            if (cells == 0)
            {
                yield return v0;
            }
            else
            {
                var vd = v1 - v0;
                var dx = vd.X / (double)cells;
                var dy = vd.Y / (double)cells;

                for (int i = 0; i <= cells; ++i)
                {
                    yield return new Vec((int)Math.Round(v0.X + dx * i), (int)Math.Round(v0.Y + dy * i));
                }
            }
        }
    }
}
