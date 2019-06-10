using HackConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalHack.Mapgen.Encounters
{
    public class Encounter
    {
        public bool Place(Level map, Grid<byte> ids, ref int id, Random rnd) {
            var maybeLoc = ChooseLocation(map, ids, ref id, rnd);
            if (maybeLoc is Vec loc)
            {
                MorphTerrain(map, loc, ids, rnd);
                PlaceMonsters(map, loc, rnd);

                return true;
            }
            return false;
        }

        private Vec? ChooseLocation(Level map, Grid<byte> ids, ref int id, Random rnd)
        {
            const int ATTEMPTS = 25;

            for (int i = 0; i < ATTEMPTS; ++i)
            {
                var v = new Vec(Game.Rnd.Next(map.Size.X), Game.Rnd.Next(map.Size.Y));

                if (!map.GetTile(v).IsFloor)
                    continue;

                if (ids[v] != 0)
                    continue;

                return v;
            }

            return null;
        }

        private void PlaceMonsters(Level map, Vec loc, Random rnd)
        {
            throw new NotImplementedException();
        }

        private void MorphTerrain(Level map, Vec loc, Grid<byte> ids, Random rnd) {}
    }
}
