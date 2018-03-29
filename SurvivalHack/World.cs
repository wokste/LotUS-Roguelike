using System;
using System.Collections.Generic;
using HackConsole;
using SurvivalHack.ECM;

namespace SurvivalHack
{
    public class World
    {
        public TileGrid Map;
        private List<Entity>[,] EntityChunks;
        private const int CHUNK_SIZE = 16;

        public World()
        {
            Vec size = new Vec(64, 64);
            Map = new TileGrid(size.X, size.Y);
            EntityChunks = new List<Entity>[(int)Math.Ceiling((float)size.X / CHUNK_SIZE), (int)Math.Ceiling((float)size.Y / CHUNK_SIZE)];

            for (int y = 0; y < EntityChunks.GetLength(1); ++y)
                for (int x = 0; x < EntityChunks.GetLength(0); ++x)
                    EntityChunks[x, y] = new List<Entity>();
        }
        
        public MoveComponent GetEmptyLocation(TerrainFlag flag = TerrainFlag.Walk)
        {
            int x, y;
            do
            {
                x = Dicebag.UniformInt(Map.Width);
                y = Dicebag.UniformInt(Map.Height);
            } while (!Map.HasFlag(x, y, flag));

            return new MoveComponent
            {
                Pos = new Vec(x, y),
                World = this,
                Flags = flag
            };
        }

        // Encaptulated functions
        public void DestroyWall(int x, int y)
        {
            Map.DestroyWall(x,y);
        }

        public bool InBoundary(int x, int y)
        {
            return (x >= 0 && x < Map.Width && y >= 0 && y < Map.Height);
        }

        public bool HasFlag(int x, int y, TerrainFlag flag)
        {
            if (Map.HasFlag(x, y, flag))
                return true;
            /*
            foreach (var c in Creatures)
                if (c.Position.X == x && c.Position.Y == y)
                    return true;
            */
            return false;
        }

        public TileType GetFloor(int x, int y)
        {
            return Map.GetFloor(x, y);
        }

        public TileType GetWall(int x, int y)
        {
            return Map.GetWall(x,y);
        }

        public TileType GetTop(int x, int y)
        {
            return Map.GetWall(x, y) ?? Map.GetFloor(x, y);
        }

        public IEnumerable<Entity> GetEntity(Vec v)
        {
            return GetEntities(new Rect(v, new Vec(1, 1)));
        }

        public IEnumerable<Entity> GetEntities(Rect r)
        {
            var x0 = Math.Max(r.Left / CHUNK_SIZE,0);
            var y0 = Math.Max(r.Top / CHUNK_SIZE, 0);
            var x1 = Math.Min(r.Right / CHUNK_SIZE, EntityChunks.GetLength(0) - 1);
            var y1 = Math.Min(r.Bottom / CHUNK_SIZE, EntityChunks.GetLength(1) - 1);

            List<Entity> ret = new List<Entity>();

            for (var y = y0; y <= y1; ++y)
                for (var x = x0; x <= x1; ++x)
                    foreach (var e in EntityChunks[x, y])
                        if (r.Contains(e.Move.Pos))
                            ret.Add(e);

            return ret;
        }

        public IList<Entity> GetChunck(Vec Pos)
        {
            var cx = Pos.X / World.CHUNK_SIZE;
            var cy = Pos.Y / World.CHUNK_SIZE;
            return EntityChunks[cx, cy];
        }
    }
}
