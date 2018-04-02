using SurvivalHack.ECM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SurvivalHack
{
    public class Game
    {
        public Level Level;
        public Timeline Timeline = new Timeline();

        public void Init()
        {
            ItemTypeList.InitTypes();
            TileTypeList.InitTypes();

            Level = new Level();

            var spawner = new MonsterSpawner(this);
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
 