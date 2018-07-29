using System;
using System.Collections.Generic;
using System.Linq;
using SurvivalHack.Mapgen.Rooms;
using HackConsole;
using System.Diagnostics;

namespace SurvivalHack.Mapgen
{
    public class DungeonGenerator
    {
        private Random _rnd = new Random();

        public float Density { get; set; } = 0.5f;
        public int Seed { get; private set; }

        public List<RoomFactory> RoomFactories = new List<RoomFactory>();

        public Action<ITimeEvent> OnNewEvent;

        public const int MASKID_VOID = -3;
        public const int MASKID_NOFLOOR = -2;
        public const int MASKID_KEEP = -1;

        public DungeonGenerator(List<Tile> tileDefs)
        {
            RoomFactories.Add(new RectRoomFactory(tileDefs));
            RoomFactories.Add(new BlobRoomFactory(tileDefs));
        }

        public (Level,Vec) Generate(Game game, int seed, int difficulty)
        {
            Seed = seed;
            _rnd = new Random(seed);

            var size = new Vec(64, 64);
            var map = new Level(game, size);
            var mask = new Grid<int>(size, MASKID_VOID);
            var rooms = new List<Room>();

            MakeBaseTerrain(map, difficulty);
            PlaceRooms(map, mask, rooms, difficulty);

            ECM.Stairs.MakeStairs(map, rooms[1].Center, difficulty++);

            // TODO: Connection

            SpawnStuff(map, mask, rooms, difficulty);
            
            return (map, rooms[0].Center);
        }

        [Obsolete("Actually not obsolete but I need a better method to make this")]
        private void MakeBaseTerrain(Level map, int difficulty)
        {
            PerlinNoise perlin = new PerlinNoise(_rnd.Next());
            perlin.Scale = 10;

            var water = map.TileDefs.Get("water");
            var rock = map.TileDefs.Get("rock");

            foreach (var v in map.TileMap.Ids()) {
                var f = perlin.Get(v.X, v.Y);

                map.TileMap[v] = f > 0.25 ? water : rock;
            }
        }

        void PlaceRooms(Level map, Grid<int> mask, List<Room> rooms, int depth) {
            
            for (var i = 0; i < 50; i++)
            {
                var room = GenerateRoom(map, mask, rooms);
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

                room.Transform = new Transform
                {
                    Offset = new Vec(rangeX.Rand(_rnd), rangeY.Rand(_rnd))
                };

                if (room.TryPlaceOnMap(map, mask, rooms))
                {
                    return room;
                }
            }
            return null;
        }
    }
}
