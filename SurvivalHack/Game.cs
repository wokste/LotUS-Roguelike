using SurvivalHack.ECM;
using System;
using System.Collections.Generic;
using System.Linq;
using HackConsole;

namespace SurvivalHack
{
    public class Game
    {
        public static readonly Random Rnd = new Random();

        public Level Level;
        public Timeline Timeline = new Timeline();

        public void Init()
        {
            ItemTypeList.InitTypes();
            TileList.InitTypes();

            var generator = new Mapgen.DungeonGenerator();

            Level = generator.Generate(Rnd.Next(), new Vec(128,64));
            

            var spawner = new Mapgen.DungeonPopulator(this);
            spawner.Spawn(Level, 16);
        }

        public void ActorAct(int ticks)
        {
            Timeline.Run();
        }

        public void Update(int ticks)
        {
        }
    }
}
 