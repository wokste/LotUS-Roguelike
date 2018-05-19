﻿using System;
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

            Level = generator.Generate(Rnd.Next(), new Vec(128,64));
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
 