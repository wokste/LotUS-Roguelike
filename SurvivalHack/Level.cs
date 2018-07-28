using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HackConsole;

namespace SurvivalHack
{
    public class Level
    {
        public Grid<int> TileMap;
        public List<Tile> TileDefs => Game.TileDefs;
        private readonly Grid<List<Entity>> _entityChunks;
        public Game Game;

        private const int CHUNK_SIZE = 16;

        public Vec Size => TileMap.Size;

        public Level(Game game, Vec size)
        {
            Game = game;
            TileMap = new Grid<int>(size, TileDefs.Get("rock"));
            _entityChunks = new Grid<List<Entity>>(new Vec((int)Math.Ceiling((float)size.X / CHUNK_SIZE), (int)Math.Ceiling((float)size.Y / CHUNK_SIZE)));

            foreach (var v in _entityChunks.Ids())
            {
                _entityChunks[v] = new List<Entity>();
            }
        }

        public bool InBoundary(Vec v)
        {
            return (v.X >= 0 && v.X < TileMap.Size.X && v.Y >= 0 && v.Y < TileMap.Size.Y);
        }

        public bool HasFlag(Vec v, TerrainFlag flag)
        {
            return GetTile(v).Flags.HasFlag(flag);
        }

        public Tile GetTile(Vec v)
        {
            return TileDefs[TileMap[v]];
        }

        public IEnumerable<Entity> GetEntities(Vec v, int radius = 0)
        {
            if (radius == 0)
                return GetEntities(new Rect(v, new Vec(1, 1)));

            var ls = GetEntities(new Rect(v - new Vec(radius,radius), new Vec(1 + 2 * radius, 1 + 2 * radius)));
            return ls.Where(e => (e.Pos - v).LengthSquared <= radius * radius);
        }

        public IEnumerable<Entity> GetEntities()
        {
            return GetEntities(new Rect(Vec.Zero, Size));
        }

        public IEnumerable<Entity> GetEntities(Rect r)
        {
            var x0 = Math.Max(r.Left / CHUNK_SIZE,0);
            var y0 = Math.Max(r.Top / CHUNK_SIZE, 0);
            var x1 = Math.Min(r.Right / CHUNK_SIZE, _entityChunks.Size.X - 1);
            var y1 = Math.Min(r.Bottom / CHUNK_SIZE, _entityChunks.Size.Y - 1);

            for (var y = y0; y <= y1; ++y)
                for (var x = x0; x <= x1; ++x)
                    foreach (var e in _entityChunks[new Vec(x, y)])
                        if (r.Contains(e.Pos))
                            yield return e;
        }

        public IList<Entity> GetChunck(Vec Pos)
        {
            var c = Pos / CHUNK_SIZE;
            return _entityChunks[c];
        }
    }
}
