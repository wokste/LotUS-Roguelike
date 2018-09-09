using HackConsole;
using SurvivalHack.Ai;
using System;
using System.Collections.Generic;
using SurvivalHack.Factory;

namespace SurvivalHack.Mapgen
{
    public class DungeonPopulator
    {
        //private Dictionary<int, RandomTable<string>> _randomTables = new Dictionary<int, RandomTable<string>>();
        private readonly DungeonGenerator _gen;
        private EntityGenerationInfo _info;
        private Level _level;

        private IEntityFactory _monsterFactory = new MonsterFactory();
        private IEntityFactory _itemFactory;

        public DungeonPopulator(DungeonGenerator gen, Level level, Random rnd)
        {
            _gen = gen;
            _level = level;
            _info = new EntityGenerationInfo
            {
                Rnd = rnd,
                Level = 1
            };
            _itemFactory = new ItemFactory(rnd);
        }

        public void Spawn(List<Room> rooms, int difficulty)
        {
            foreach (var room in rooms)
            {
                //var roll = _info.Rnd.Next(100);

                //if (roll < 30)
                //    continue;

                FillRoom(room, difficulty);

                //if (roll >= 95)
                //    FillRoom(room, difficulty);
            }
        }

        private void FillRoom(Room room, int difficulty)
        {
            PlaceMonsters(room, difficulty);

            //if (_info.Rnd.Next(100) < 80)
            PlaceTreasure(room, difficulty);
        }

        private Vec? GetFreeTile(Room room, Func<Tile, bool> pred)
        {
            Vec v;
            var p0 = room.Transform.TransformVec(Vec.Zero);
            var p1 = room.Transform.TransformVec(room.Size.BottomRight - Vec.One);

            var attempts = 0;

            do
            {
                if (attempts > 10000)
                    return null;

                v = new Vec(
                    new Range(p0.X, p1.X).Rand(Game.Rnd),
                    new Range(p0.Y, p1.Y).Rand(Game.Rnd));
            } while (!pred(_level.GetTile(v))); // TODO: This can be less strict for flying creatures

            return v;
        }

        void PlaceMonsters(Room room, int difficulty)
        {
            var monster = _monsterFactory.Gen(_info);
            var pos = GetFreeTile(room, t => !t.Solid && (t.WalkDanger == 0));
            if (pos is Vec p2)
            {
                monster.SetLevel(_level, p2);
                _gen?.OnNewEvent(new ActEvent(monster));
            }
        }

        private void PlaceTreasure(Room room, int difficulty)
        {
            var item = _itemFactory.Gen(_info);
            var pos = GetFreeTile(room, t => !t.Solid && (t.WalkDanger == 0));
            if (pos is Vec p2)
            {
                item.SetLevel(_level, p2);
            }
        }
    }
}
