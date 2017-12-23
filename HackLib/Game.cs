using System;
using System.Collections.Generic;
using System.Drawing;

namespace HackLib
{
    public class Game
    {
        [System.Obsolete]
        public Creature Player;
        public Queue<Controller> Controllers = new Queue<Controller>();

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
                var c = Controllers.Peek();
                if (c.ShouldDelete)
                {
                    Controllers.Dequeue();
                    World.Creatures.Remove(c.Self);
                    continue;
                }

                var r = c.Act();
                
                if (r == true)
                    Controllers.Enqueue(Controllers.Dequeue());
                else
                    return;

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
            Controllers.Enqueue(controller);
            World.Creatures.Add(controller.Self);
        }
    }
}
 