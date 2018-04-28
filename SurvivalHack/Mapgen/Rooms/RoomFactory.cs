using System;

namespace SurvivalHack.Mapgen.Rooms
{
    public abstract class RoomFactory
    {
        public string Name;
        public int Odds = 100;

        public abstract Room Make(Random rnd);
    }
}