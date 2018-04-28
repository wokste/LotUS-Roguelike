using System;

namespace SurvivalHack.Mapgen.Rooms
{
    public abstract class RoomFactory
    {
        public string Name { get; set; }
        public int Odds { get; set; }

        public abstract Room Make(Random rnd);
    }
}