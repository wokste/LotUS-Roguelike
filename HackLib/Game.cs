using System;
using System.Collections.Generic;

namespace HackLib
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

        private const int HUNGER_TICKS = 50;
        private int _ticksTillHungerLoss = HUNGER_TICKS;

        public void Update()
        {
            while (true)
            {
                var c = Time.Peek();
                if (c.ShouldDelete)
                {
                    Time.Dequeue();
                    World.Creatures.Remove(c.Self);
                    continue;
                }

                var turns = c.Act();

                if (turns <= 0)
                    return;
                
                Time.Dequeue();
                var ticks = (int)Math.Ceiling(1000 * turns / c.Self.Speed);
                Time.AddRelative(c, ticks);
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
 