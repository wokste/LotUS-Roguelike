using System;
using System.Collections.Generic;
using System.Diagnostics;
using HackConsole;
using SurvivalHack.ECM;

namespace SurvivalHack
{
    public class Level
    {
        public TileGrid Map;
        private readonly List<Entity>[,] _entityChunks;
        private const int CHUNK_SIZE = 16;

        public Level()
        {
            var size = new Vec(64, 64);
            Map = new TileGrid(size);
            _entityChunks = new List<Entity>[(int)Math.Ceiling((float)size.X / CHUNK_SIZE), (int)Math.Ceiling((float)size.Y / CHUNK_SIZE)];

            for (var y = 0; y < _entityChunks.GetLength(1); ++y)
                for (var x = 0; x < _entityChunks.GetLength(0); ++x)
                    _entityChunks[x, y] = new List<Entity>();
        }
        
        public Vec GetEmptyLocation(TerrainFlag flag = TerrainFlag.Walk)
        {
            Debug.Assert(flag != TerrainFlag.None);

            Vec v;
            do
            {
                v = new Vec (
                    Dicebag.UniformInt(Map.Size.X),
                    Dicebag.UniformInt(Map.Size.Y));
            } while (!Map.HasFlag(v, flag));

            return v;
        }

        public bool InBoundary(Vec v)
        {
            return (v.X >= 0 && v.X < Map.Size.X && v.Y >= 0 && v.Y < Map.Size.Y);
        }

        public bool HasFlag(Vec v, TerrainFlag flag)
        {
            if (Map.HasFlag(v, flag))
                return true;
            /*
            foreach (var c in Creatures)
                if (c.Position.X == x && c.Position.Y == y)
                    return true;
            */
            return false;
        }

        public Tile GetTile(Vec v)
        {
            return Map.Get(v);
        }

        public IEnumerable<Entity> GetEntity(Vec v)
        {
            return GetEntities(new Rect(v, new Vec(1, 1)));
        }

        public IEnumerable<Entity> GetEntities(Rect r)
        {
            var x0 = Math.Max(r.Left / CHUNK_SIZE,0);
            var y0 = Math.Max(r.Top / CHUNK_SIZE, 0);
            var x1 = Math.Min(r.Right / CHUNK_SIZE, _entityChunks.GetLength(0) - 1);
            var y1 = Math.Min(r.Bottom / CHUNK_SIZE, _entityChunks.GetLength(1) - 1);

            List<Entity> ret = new List<Entity>();

            for (var y = y0; y <= y1; ++y)
                for (var x = x0; x <= x1; ++x)
                    foreach (var e in _entityChunks[x, y])
                        if (r.Contains(e.Move.Pos))
                            ret.Add(e);

            return ret;
        }

        public IList<Entity> GetChunck(Vec Pos)
        {
            var cx = Pos.X / CHUNK_SIZE;
            var cy = Pos.Y / CHUNK_SIZE;
            return _entityChunks[cx, cy];
        }
    }
}
