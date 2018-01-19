using System;

namespace SurvivalHack
{
    public class Game
    {
        [System.Obsolete]
        public Creature Player;
        public Timeline<Controller> Time = new Timeline<Controller>();

        public MonsterSpawner Spawner;
        public World World;

        public void Init()
        {
            ItemTypeList.InitTypes();
            TileTypeList.InitTypes();

            World = new World();

            Spawner = new MonsterSpawner(this);
            Spawner.Spawn(World, 32);
        }

        public void Update()
        {
            while (true)
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
            }
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

        public void AddCreature(Controller controller)
        {
            Time.AddRelative(controller, 1000);
            World.Creatures.Add(controller.Self);
        }
    }
}
 