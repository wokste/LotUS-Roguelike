using HackConsole;
using System;
using System.Linq;

namespace SurvivalHack.Mapgen
{
    public class VoronoiNoise<T>
    {
        int Scale;
        Grid<T> Cells;
        Grid<Vec> Centers;

        public static VoronoiNoise<T> Make(Size rect, int scale, Random rnd, RandomTable<T> elems)
        {
            var cells = new Grid<T>(new Size((int)Math.Ceiling(rect.X / (double)scale), (int)Math.Ceiling(rect.Y / (double)scale)));
            var centers = new Grid<Vec>(cells.Size);

            foreach (var i in cells.Ids())
            {
                cells[i] = elems.GetRand(rnd);
                centers[i] = new Vec(i.X * scale + rnd.Next(scale), i.Y * scale + rnd.Next(scale));
            }

            return new VoronoiNoise<T> {
                Scale = scale,
                Cells = cells,
                Centers = centers
            };
        }


        private Vec Closest(Vec v)
        {
            var rect = new Rect(v.X / Scale - 1, v.Y / Scale - 1, 3, 3);
            rect = rect.Intersect(Centers.Size.ToRect());
            return rect.Iterator().OrderBy(i => (Centers[i] - v).LengthSquared).First();
        }

        public T Get(Vec v)
        {
            var c = Closest(v);
            return Cells[c];
        }

    }
}
