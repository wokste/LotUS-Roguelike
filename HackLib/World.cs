using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackLib
{
    public class World
    {
        public TileGrid _map;
        public List<Creature> Creatures = new List<Creature>();

        public World()
        {
            _map = new TileGrid(256, 256);
        }
        
        public Point GetEmptyLocation()
        {
            int x, y;
            do
            {
                x = Dicebag.UniformInt(_map.Width);
                y = Dicebag.UniformInt(_map.Height);
            } while (_map.HasFlag(x, y, TerrainFlag.BlockWalk));

            return new Point(x, y);
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
            return _map.HasFlag(x, y, flag);
        }

        public TileType GetFloor(int x, int y)
        {
            return _map.GetFloor(x, y);
        }

        public TileType GetWall(int x, int y)
        {
            return _map.GetWall(x,y);
        }
    }
}
