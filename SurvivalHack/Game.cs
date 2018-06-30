using System;
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
            TileList.InitTypes();
#if WIZTOOLS
            WizTools.Init();
#endif
            var generator = new Mapgen.DungeonGenerator();
            generator.OnNewEvent += (e)=>{ Timeline.Insert(e); };

            Level = new Level(this, 1, new Vec(64, 64));
            generator.Generate(Level, Rnd.Next());
        }

        public void MonsterTurn()
        {
            Timeline.Run();
        }

        public void Update(int ticks)
        {
        }
    }
}
 