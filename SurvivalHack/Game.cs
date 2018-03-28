using SurvivalHack.ECM;
using System.Linq;

namespace SurvivalHack
{
    public class Game
    {
        public MonsterSpawner Spawner;
        public World World;

        public void Init()
        {
            ItemTypeList.InitTypes();
            TileTypeList.InitTypes();

            World = new World();

            Spawner = new MonsterSpawner(this);
            Spawner.Spawn(World, 16);
        }

        public void GameTick(int ticks)
        {
            foreach (var c in World.Creatures.ToList())
            {
                if (!(c is Monster m))
                    continue;

                m.Act();
            }
        }

        public void Update(int ticks)
        {
            /*while (true)
            {
                var gameEvent = Time.Peek();
                if (gameEvent.ShouldDelete)
                {
                    Time.Dequeue();
                    continue;
                }

                var steps = gameEvent.Do();

                if (steps <= 0)
                    return;
                
                Time.Dequeue();
                var ticks = (int)Math.Ceiling(1000 * steps / gameEvent.Self.Speed);
                Time.AddRelative(gameEvent, ticks);
            }*/
        }

        /*
        private void TimeAdvance(int ticks)
        {
            _ticksTillHungerLoss -= ticks;

            while (_ticksTillHungerLoss <= 0)
            {
                _ticksTillHungerLoss += HUNGER_TICKS;
                Player.Hunger.Current--;
                if (Player.Hunger.Current == 0)
                    Player.Alive = false;

                Player.DisplayStats();
            }
        }*/

        public void AddCreature(Entity creature)
        {
            //Time.AddRelative(controller, 1000);
            World.Creatures.Add(creature);
        }
    }
}
 