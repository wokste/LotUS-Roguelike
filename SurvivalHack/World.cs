using System.Collections.Generic;
using HackConsole;
using SurvivalHack.ECM;

namespace SurvivalHack
{
    public class World
    {
        public TileGrid Map;
        public List<Entity> Creatures = new List<Entity>();

        public World()
        {
            Map = new TileGrid(64, 64);
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
                MovementType = flag
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

        internal Entity GetEntity(int x, int y)
        {
            foreach (var c in Creatures)
                if (c.Move.Pos.X == x && c.Move.Pos.Y == y)
                    return c;

            return null;
        }
    }
}
