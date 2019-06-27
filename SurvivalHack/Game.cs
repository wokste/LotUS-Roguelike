using System;
using System.Collections.Generic;
using HackConsole;

namespace SurvivalHack
{
    public class Game
    {
        // TODO: remove seed. This is needed for XML comparisons.
        public static readonly Random Rnd = new Random(0);

        public Timeline Timeline = new Timeline();
        public List<Tile> TileDefs;

        public void Init()
        {
            TileDefs = Tile.InitTypes();
#if WIZTOOLS
            WizTools.Init();
#endif
        }

        public (Level,Vec) GetLevel(int difficulty)
        {
            var generator = new Mapgen.DungeonGenerator(TileDefs);
            generator.OnNewEvent += (e) => { Timeline.Insert(e); };

            return generator.Generate(this, Rnd.Next(), difficulty);
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
 