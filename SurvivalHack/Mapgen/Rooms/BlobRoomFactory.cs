using System;
using System.Collections.Generic;
using System.Linq;
using HackConsole;

namespace SurvivalHack.Mapgen.Rooms
{
    public class BlobRoomFactory : RoomFactory
    {
        public Range MaxRadius = new Range("4-8");
        public Range MinRadius = new Range("1");
        public Range Spikes = new Range("4-6");

        public int? FloorTile;
        public int? WallTile;
        public double Roughness = 1;
        
        public BlobRoomFactory(List<Tile> tileDefs)
        {
            Name = "BlobType";
            FloorTile = tileDefs.Get("floor_stone");
            WallTile = tileDefs.Get("rock");
        }

        public override Room Make(Random rnd)
        {
            var blob = new Blob(this, rnd);

            var room = new Room(blob.Rect.Size);

            DrawFloor(room, blob);
            DrawWalls(room);

            return room;
        }

        private void DrawFloor(Room r, Blob blob)
        {
            // Make floor shape
            var offset = blob.Rect.TopLeft;
            foreach (var v in r.Tiles.Ids())
            {
                var pos = v + offset;
                r.Tiles[v] = blob.IsFloor(pos.X, pos.Y) ? FloorTile : null;
            }
        }

        private void DrawWalls(Room r) {

            bool ShouldHaveWall(Vec v)
            {
                if (r.Tiles[v] != null)
                    return false; // Floor never becomes wall.

                // Check if there is an adjacent floor.
                for (var y1 = Math.Max(v.Y - 1, 0); y1 <= Math.Min(v.Y + 1, r.Size.Y - 1); y1++)
                    for (var x1 = Math.Max(v.X - 1, 0); x1 <= Math.Min(v.X + 1, r.Size.X - 1); x1++)
                        if (r.Tiles[new Vec(x1, y1)] == FloorTile)
                            return true;

                return false;
            }

            // Iterate over all tiles drawing walls if needed

            foreach (var v in r.Tiles.Ids())
            {
                if (ShouldHaveWall(v))
                    r.Tiles[v] = WallTile;
            }
        }

        public class Blob
        {
            private readonly double[] _dist;
            public Rect Rect;
            private readonly double _roughness;
            private Random _rnd;

            public Blob(BlobRoomFactory brf, Random rnd)
            {
                _rnd = rnd;
                _dist = new double[brf.Spikes.Rand(_rnd)];

                var max = brf.MaxRadius.Rand(_rnd);
                var min = brf.MinRadius.Rand(_rnd);

                for (int i = 0; i < _dist.Length; i++)
                    _dist[i] = (max-min) * _rnd.NextDouble() + min;

                _roughness = brf.Roughness;

                // Fill the area of the blob.
                var maxLen = (int)Math.Ceiling(_dist.Max() + _roughness);

                Rect = new Rect
                {
                    Left = -maxLen,
                    Top = -maxLen,
                    Width = maxLen * 2 + 1,
                    Height = maxLen * 2 + 1,
                };
            }

            public bool IsFloor(int x, int y)
            {
                if (x == 0 && y == 0)
                    return true;

                // Convert to polar coords;
                var angle = Math.Atan2(y, x);
                var dist = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));

                // Normalize angle for the 
                var normalizedAngle = ((angle / Math.PI / 2) + 2) * _dist.Length;
                var id = (int)Math.Floor(normalizedAngle);
                var f = normalizedAngle - id;
                var maxDist = Lerp(_dist[id % _dist.Length], _dist[(id + 1) % _dist.Length], f);

                return dist < maxDist + _rnd.NextDouble() * _roughness;
            }

            private static double Lerp(double a, double b, double f)
            {
                return (a * (1 - f)) + (b * f);
            }
        }
    }
}
