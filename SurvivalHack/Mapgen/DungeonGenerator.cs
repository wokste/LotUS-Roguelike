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

        public DungeonGenerator()
        {
            RoomFactories.Add(new RectRoomFactory());
            RoomFactories.Add(new BlobRoomFactory());
        }

        struct FloorData {
            public Level map;
            public Grid<int> mask;
            public List<Room> rooms;

            public FloorData(Vec size) {
                map = new Level(size);
                mask = new Grid<int>(size, MASKID_VOID);
                rooms = new List<Room>();
            }
        }

        public Level[] GenerateAll(int seed)
        {
            Seed = seed;
            _rnd = new Random(seed);

            var levels = new FloorData[10];
            for (int i = 0; i < levels.Length; ++i)
            {
                levels[i] = new FloorData(new Vec(64, 64));
            }

            for (int i = 0; i < levels.Length; ++i)
            {
                PlaceRooms(levels[i], i);
            }

            for (int i = 0; i < levels.Length - 1; ++i)
            {
                ECM.Stairs.Link(levels[i].map, levels[i].rooms[0].Center, levels[i+1].map, levels[i+1].rooms[1].Center);
            }

            // TODO: Connection

            for (int i = 0; i < levels.Length; ++i)
            {
                SpawnStuff(levels[i], i);
            }

            return levels.Select(d => d.map).ToArray();
        }

        void PlaceRooms(FloorData data, int depth) {
            
            for (var i = 0; i < 50; i++)
            {
                var room = GenerateRoom(data);
            }

            // Create MST
            var Connector = new DungeonConnector(data.map, data.rooms);
            Connector.Prim();
            Connector.EliminateDeadEnds(_rnd, 0.5);
        }

        void SpawnStuff(FloorData data, int depth)
        {
            var spawner = new DungeonPopulator(this, data.map, _rnd); // TODO FloorData
            spawner.Spawn(data.rooms, depth);
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

        private Room GenerateRoom(FloorData data)
        {
            var room = GetFactory().Make(_rnd);

            var free = data.map.Size - room.Size;
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

                if (room.TryPlaceOnMap(data.map, data.mask, data.rooms))
                {
                    return room;
                }
            }
            return null;
        }
    }
}
