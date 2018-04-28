using SurvivalHack.ECM;
using System;
using System.Collections.Generic;
using System.Linq;

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

            Level = new Level();

            foreach (var v in Level.TileMap.Ids())
            {
                Level.TileMap[v] = TileList.Get("grass");
            }

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
 