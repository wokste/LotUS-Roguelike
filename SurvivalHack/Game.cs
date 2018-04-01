using SurvivalHack.ECM;
using System.Linq;

namespace SurvivalHack
{
    public class Game
    {
        public MonsterSpawner Spawner;
        public Level Level;

        public void Init()
        {
            ItemTypeList.InitTypes();
            TileTypeList.InitTypes();

            Level = new Level();

            Spawner = new MonsterSpawner(this);
            Spawner.Spawn(Level, 16);
        }

        public void GameTick(int ticks)
        {
            /*
            foreach (var c in World.GetEntities())
            {
                if (!(c is Monster m))
                    continue;

                m.Act();
            }
            */
        }

        public void Update(int ticks)
        {
        }
    }
}
 