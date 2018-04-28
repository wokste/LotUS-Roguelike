using System;
using System.Collections.Generic;
using System.Linq;
using SurvivalHack.Mapgen.Rooms;
using HackConsole;

namespace SurvivalHack.Mapgen
{
    public class DungeonGenerator
    {
        private Random _rnd = new Random();

        internal float Density { get; set; } = 0.5f;
        internal int Seed { get; private set; }

        internal List<RoomFactory> RoomFactories = new List<RoomFactory>();

        internal DungeonGenerator()
        {
            RoomFactories.Add(new RectRoomFactory());
            RoomFactories.Add(new BlobRoomFactory());
        }

        internal (AbstractMap, int) GenerateNew(int seed, Vec size)
        {
            Seed = seed;
            _rnd = new Random(seed);

            // Add rooms
            var map = new AbstractMap(size);
            var rooms = 0;
            GenerateRoom(map);
            for (var i = 0; i < 500; i++)
            {
                if (GenerateRoom(map))
                    rooms++;
            }

            // Create MST
            var Connector = new DungeonConnector(map);
            Connector.Prim();
            Connector.EliminateDeadEnds(_rnd, 1);

            return (map, rooms);
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

        private bool GenerateRoom(AbstractMap map)
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

                if (room.TryPlaceOnMap(map))
                    return true;
            }
            return false;
        }
    }
}
