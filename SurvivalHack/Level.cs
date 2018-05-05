using System;
using System.Collections.Generic;
using System.Diagnostics;
using HackConsole;

namespace SurvivalHack
{
    public class Level
    {
        public Grid<Tile> TileMap;
        private readonly Grid<List<Entity>> _entityChunks;
        private const int CHUNK_SIZE = 16;

        public Vec Size => TileMap.Size;

        public Level(Vec size)
        {
            TileMap = new Grid<Tile>(size, TileList.Get("rock"));
            _entityChunks = new Grid<List<Entity>>(new Vec((int)Math.Ceiling((float)size.X / CHUNK_SIZE), (int)Math.Ceiling((float)size.Y / CHUNK_SIZE)));

            foreach (var v in _entityChunks.Ids())
            {
                _entityChunks[v] = new List<Entity>();
            }
        }

        [Obsolete]
        public Vec GetEmptyLocation(TerrainFlag flag = TerrainFlag.Walk)
        {
            Debug.Assert(flag != TerrainFlag.None);

            Vec v;
            do
            {
                v = new Vec(
                    new Range(0, TileMap.Size.X - 1).Rand(Game.Rnd),
                    new Range(0, TileMap.Size.Y - 1).Rand(Game.Rnd));
            } while (!TileMap[v].Flags.HasFlag(flag));

            return v;
        }

        public bool InBoundary(Vec v)
        {
            return (v.X >= 0 && v.X < TileMap.Size.X && v.Y >= 0 && v.Y < TileMap.Size.Y);
        }

        public bool HasFlag(Vec v, TerrainFlag flag)
        {
            if (TileMap[v].Flags.HasFlag(flag))
                return true;
            
            foreach (var c in GetEntity(v))
                return true;
            
            return false;
        }

        public Tile GetTile(Vec v)
        {
            return TileMap[v];
        }

        public IEnumerable<Entity> GetEntity(Vec v)
        {
            return GetEntities(new Rect(v, new Vec(1, 1)));
        }

        public IEnumerable<Entity> GetEntities(Rect r)
        {
            var x0 = Math.Max(r.Left / CHUNK_SIZE,0);
            var y0 = Math.Max(r.Top / CHUNK_SIZE, 0);
            var x1 = Math.Min(r.Right / CHUNK_SIZE, _entityChunks.Size.X - 1);
            var y1 = Math.Min(r.Bottom / CHUNK_SIZE, _entityChunks.Size.Y - 1);

            List<Entity> ret = new List<Entity>();

            for (var y = y0; y <= y1; ++y)
                for (var x = x0; x <= x1; ++x)
                    foreach (var e in _entityChunks[new Vec(x, y)])
                        if (r.Contains(e.Move.Pos))
                            ret.Add(e);

            return ret;
        }

        public IList<Entity> GetChunck(Vec Pos)
        {
            var c = Pos / CHUNK_SIZE;
            return _entityChunks[c];
        }
    }
}
