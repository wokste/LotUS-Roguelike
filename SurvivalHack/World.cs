using System.Collections.Generic;
using HackConsole;

namespace SurvivalHack
{
    public class World
    {
        public TileGrid _map;
        public List<Creature> Creatures = new List<Creature>();

        public World()
        {
            _map = new TileGrid(256, 256);
        }
        
        public Vec GetEmptyLocation(TerrainFlag flag = TerrainFlag.Walk)
        {
            int x, y;
            do
            {
                x = Dicebag.UniformInt(_map.Width);
                y = Dicebag.UniformInt(_map.Height);
            } while (!_map.HasFlag(x, y, flag));

            return new Vec(x, y);
        }

        // Encaptulated functions
        public void DestroyWall(int x, int y)
        {
            _map.DestroyWall(x,y);
        }

        public bool InBoundary(int x, int y)
        {
            return (x >= 0 && x < _map.Width && y >= 0 && y < _map.Height);
        }

        public bool HasFlag(int x, int y, TerrainFlag flag)
        {
            if (_map.HasFlag(x, y, flag))
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
            return _map.GetFloor(x, y);
        }

        public TileType GetWall(int x, int y)
        {
            return _map.GetWall(x,y);
        }

        public TileType GetTop(int x, int y)
        {
            return _map.GetWall(x, y) ?? _map.GetFloor(x, y);
        }
    }
}
