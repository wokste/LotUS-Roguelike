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

        internal float Density { get; set; } = 0.5f;
        internal int Seed { get; private set; }

        internal List<RoomFactory> RoomFactories = new List<RoomFactory>();

        internal Action<ITimeEvent> OnNewEvent;

        public const int MASKID_VOID = -3;
        public const int MASKID_NOFLOOR = -2;
        public const int MASKID_KEEP = -1;

        internal DungeonGenerator()
        {
            RoomFactories.Add(new RectRoomFactory());
            RoomFactories.Add(new BlobRoomFactory());
        }

        internal Level Generate(int seed, Vec size)
        {
            Seed = seed;
            _rnd = new Random(seed);

            // Add rooms
            var map = new Level(size);
            var maskMap = new Grid<int>(size, MASKID_VOID);

            var rooms = new List<Room>();
            for (var i = 0; i < 50; i++)
            {
                var room = GenerateRoom(map, maskMap, rooms);
            }

            // Create MST
            var Connector = new DungeonConnector(map, rooms);
            Connector.Prim();
            Connector.EliminateDeadEnds(_rnd, 1);

            // Populate map

            var spawner = new DungeonPopulator(this, map, _rnd);
            spawner.Spawn(rooms, 1);

            return map;
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

        private Room GenerateRoom(Level map, Grid<int> maskMap, List<Room> rooms)
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

                if (room.TryPlaceOnMap(map, maskMap, rooms))
                {
                    Debug.WriteLine($"Placed room {rooms.Count - 1} on {room.Transform.Offset}");
                    return room;
                }
            }
            return null;
        }
    }
}
