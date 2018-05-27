using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackConsole.Algo
{
    public static class Bresenham
    {
        public static IEnumerable<Vec> Run(Vec v0, Vec v1)
        {
            if (Math.Abs(v1.Y - v0.Y) < Math.Abs(v1.X - v0.X))
            {
                if (v0.X > v1.X)
                    return PlotLineLow(v1, v0).Reverse();
                else
                    return PlotLineLow(v0, v1);
            }
            else
            {
                if (v0.Y > v1.Y)
                    return PlotLineHigh(v1, v0).Reverse();
                else
                    return PlotLineHigh(v0, v1);
            }
        }

        static IEnumerable<Vec> PlotLineLow(Vec v0, Vec v1)
        {
            var d = v1 - v0;
            var yi = 1;

            if (d.Y < 0)
            {
                yi = -1;
                d = new Vec(d.X, -d.Y);
            }
            var D = 2 * d.Y - d.X;
            var y = v0.Y;

            for (int x = v0.X; x <= v1.X; x++)
            {
                yield return new Vec(x, y);
                if (D > 0)
                {
                    y += yi;
                    D -= 2 * d.X;
                }
                D += 2 * d.Y;
            }
        }

        static IEnumerable<Vec> PlotLineHigh(Vec v0, Vec v1)
        {
            var d = v1 - v0;
            var xi = 1;

            if (d.X < 0)
            { 
                xi = -1;
                d = new Vec(-d.X, d.Y);
            }

            var D = 2 * d.X - d.Y;
            var x = v0.X;

            for (int y = v0.Y; y <= v1.Y; y++)
            {
                yield return new Vec(x, y);
                if (D > 0)
                {
                    x += xi;
                    D -= 2 * d.Y;
                }
                D += 2 * d.X;
            }
        }
    }
}
