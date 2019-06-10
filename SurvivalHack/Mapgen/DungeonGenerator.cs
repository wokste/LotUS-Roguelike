using System;
using System.Collections.Generic;
using System.Linq;
using SurvivalHack.Mapgen.Rooms;
using HackConsole;
using SurvivalHack.Mapgen.Automata;

namespace SurvivalHack.Mapgen
{
    public class DungeonGenerator
    {
        private Random _rnd = new Random();

        public float Density { get; set; } = 0.5f;
        public int Seed { get; private set; }
        public int NumRooms { get; set; } = 50;

        public List<RoomFactory> RoomFactories = new List<RoomFactory>();

        public Action<ITimeEvent> OnNewEvent;

        public const int MASKID_VOID = -3;
        public const int MASKID_NOFLOOR = -2;
        public const int MASKID_KEEP = -1;

        public DungeonGenerator(List<Tile> tileDefs)
        {
            RoomFactories.Add(new RectRoomFactory(tileDefs) {
                Odds = 40,
                FloorTile = new Room.TileInfo { Id = tileDefs.Get("floor_wood"), Method = Room.PasteMethod.Paste },
                WallTile = new Room.TileInfo { Id = tileDefs.Get("wall_stone"), Method = Room.PasteMethod.Paste },
            });
            RoomFactories.Add(new RectRoomFactory(tileDefs) {
                Odds = 60,
                FloorTile = new Room.TileInfo { Id = tileDefs.Get("floor_stone"), Method = Room.PasteMethod.Paste },
                WallTile = new Room.TileInfo { Id = -1, Method = Room.PasteMethod.Nil },
                RangeX = new Range("1"),
                RangeY = new Range("1"),
            });
            //RoomFactories.Add(new BlobRoomFactory(tileDefs));
        }

        public (Level,Vec) Generate(Game game, int seed, int difficulty)
        {
            Seed = seed;
            _rnd = new Random(seed);

            var size = new Size(64, 32);
            var map = new Level(game, size);
            var mask = new Grid<int>(size, MASKID_VOID);
            var rooms = new List<Room>();

            MakeBaseTerrain(map, difficulty);
            PlaceRooms(map, mask, rooms, difficulty);

            var floor = map.TileDefs.Get("floor_stone");
            var caveAutomata = new BomberAutomata
            {
                TileId = floor,
                AffectPred = (t) => t.Solid && t.Natural
            };
            caveAutomata.Run(_rnd, map);

            ECM.Stairs.MakeStairs(map, rooms[1].Center, difficulty++);

            SpawnStuff(map, mask, rooms, difficulty);
            
            return (map, rooms[0].Center);
        }

        private void MakeBaseTerrain(Level map, int difficulty)
        {
            var table = new RandomTable<int>(new []{
                (map.TileDefs.Get("rock"), 2),
                (map.TileDefs.Get("rock2"), 2),
                (map.TileDefs.Get("acid"), 1),
            });

            var noise = VoronoiNoise<int>.Make(map.Size, 5, _rnd, table);

            foreach (var v in map.TileMap.Ids()) {
                var tileId = noise.Get(v);

                map.TileMap[v] = tileId;
            }
        }

        void PlaceRooms(Level map, Grid<int> mask, List<Room> rooms, int depth) {
            
            for (var i = 0; i < NumRooms; i++)
            {
                _ = GenerateRoom(map, mask, rooms);
            }

            // Create MST
            var Connector = new DungeonConnector(map, rooms);
            Connector.Prim();
            Connector.EliminateDeadEnds(_rnd, 0.5);
        }

        void SpawnStuff(Level map, Grid<int> mask, List<Room> rooms, int depth)
        {
            var spawner = new DungeonPopulator(this, map, _rnd); // TODO FloorData
            spawner.Spawn(rooms, depth);
        }

        private RoomFactory GetFactory()
        {
            var total = RoomFactories.Sum(r => r.Odds);
            var rand = _rnd.Next(total);

            foreach (var rf in RoomFactories)
            {
                rand -= rf.Odds;
                if (rand < 0)
                    return rf;
            }
            throw new ArithmeticException("Divide by zero exception");
        }

        private Room GenerateRoom(Level map, Grid<int> mask, List<Room> rooms)
        {
            var room = GetFactory().Make(_rnd);

            var free = map.Size - room.Size;
            const double a1 = 0;
            const double a2 = 1 - a1;

            for (int i = 0; i < 50; ++i)
            {
                var rangeX = new Range((int)(free.X * a1), (int)(free.X * a2));
                var rangeY = new Range((int)(free.Y * a1), (int)(free.Y * a2));

                room.Transform = TransformExt.MakeTranslateRotate(new Vec(rangeX.Rand(_rnd), rangeY.Rand(_rnd)), 0f);

                if (room.TryPlaceOnMap(map, mask, rooms))
                {
                    return room;
                }
            }
            return null;
        }
    }
}
