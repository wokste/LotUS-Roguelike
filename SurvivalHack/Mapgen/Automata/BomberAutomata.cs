using HackConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalHack.Mapgen.Automata
{
    class BomberAutomata
    {
        public int TileId;
        public Func<Tile, bool> AffectPred;
        public float Density = 0.3f;

        public static void ShuffleList<T>(Random rnd, IList<T> list)
        {
            for (int n = list.Count - 1; n >= 1; n--)
            {
                int k = rnd.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }

        public void Run(Random rnd, Level map)
        {
            for (int i = 0; i < 3; ++i)
                RunIteration(rnd, map);
        }

        public void RunIteration(Random rnd, Level map)
        { 
            var innerRect = map.Size.ToRect().Grow(-3);
            var outerRect = map.Size.ToRect().Grow(-1);

            var candidates = innerRect.Iterator().Where(v => map.TileMap[v] == TileId).ToList();
            ShuffleList(rnd, candidates);

            var iteration_number = (int)(candidates.Count * Density);
            for (var i = 0; i < iteration_number; i++)
            {
                var c = new Circle(
                    candidates[i],
                    (float)(rnd.NextDouble() + 1) // TODO: Magic numbers
                );

                foreach (var v2 in c.Iterator())
                {
                    if (!outerRect.Contains(v2))
                        continue;

                    if (AffectPred(map.GetTile(v2)))
                    {
                        map.TileMap[v2] = TileId;
                        if (innerRect.Contains(v2))
                            candidates.Add(v2);
                    }
                }
            }
        }
    }
}
