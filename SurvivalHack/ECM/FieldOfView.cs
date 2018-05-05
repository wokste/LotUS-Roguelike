using HackConsole;
using SurvivalHack.ECM;
using System;

namespace SurvivalHack
{
    public class FieldOfView
    {
        public const byte FLAG_DISCOVERED = 0x1;
        public const byte FLAG_VISIBLE = 0x2;
        public const byte FLAG_ALWAYSVISIBLE = 0x4;

        public const byte SET_VISIBLE = FLAG_DISCOVERED | FLAG_VISIBLE;
        public const byte SET_ALWAYSVISIBLE = SET_VISIBLE | FLAG_ALWAYSVISIBLE;

        private Vec _entityPos;

        public Level Map;

        private int _visualRange = 32;

        public int VisualRange
        {
            get => _visualRange;
            set
            {
                VisibleToDark();
                _visualRange = value;
                ToVisible();
            }
        }

        public Grid<byte> Visibility;

        public FieldOfView(MoveComponent move)
        {
            Update(move);
        }
        
        public void Update(MoveComponent move)
        {
            if (Map != move.Level)
            {
                Map = move.Level;
                Visibility = new Grid<byte>(Map.TileMap.Size); // 0 initialized so everything is dark.
            }

            VisibleToDark();
            _entityPos = move.Pos;
            ToVisible();
        }

        private void VisibleToDark()
        {
            var r = new Rect(_entityPos, Vec.One).Grow(_visualRange);
            r = r.Intersect(new Rect(Vec.Zero, Visibility.Size));

            foreach (var v in r.Iterator())
            {
                bool alwaysVisible = (Visibility[v] & FLAG_ALWAYSVISIBLE) == FLAG_ALWAYSVISIBLE;

                if (!alwaysVisible)
                    Visibility[v] = (byte)(Visibility[v] & ~FLAG_VISIBLE);
            }
        }
        
        /// <summary>
        /// Start here: go through all the quatrants which surround the player to
        /// determine which open cells are visible
        /// </summary>
        private void ToVisible()
        {
            Visibility[_entityPos] |= FLAG_DISCOVERED | FLAG_VISIBLE;

            ScanQuatrantV(1, 'N', -1.0, 1.0);
            ScanQuatrantV(1, 'S', -1.0, 1.0);
            
            ScanQuatrantH(1, 'W', -1.0, 1.0);
            ScanQuatrantH(1, 'E', -1.0, 1.0);
        }

        /// <summary>
        /// Examine the provided quatrant and calculate the visible cells within it.
        /// </summary>
        /// <param name="depth">Depth of the scan</param>
        /// <param name="direction">1 is east, -1 is west</param>
        /// <param name="minSlope">Start slope of the quatrant</param>
        /// <param name="maxSlope">End slope of the octance</param>
        protected void ScanQuatrantH(int depth, char direction, double minSlope, double maxSlope)
        {
            if (depth > VisualRange)
                return;

            var x = direction == 'W' ? _entityPos.X - depth : _entityPos.X + depth;

            if (x < 0 || x >= Visibility.Size.X)
                return;

            var yMin = (int)Math.Floor(Clamp(_entityPos.Y + minSlope * depth + 0.49, 0, Visibility.Size.Y - 1));
            var yMax = (int)Math.Ceiling(Clamp(_entityPos.Y + maxSlope * depth - 0.49, 0, Visibility.Size.Y - 1));

            for (var y = yMin; y <= yMax; y++)
            {
                UpdateVisibility(new Vec(x,y));

                if (!Map.HasFlag(new Vec(x, y), TerrainFlag.Sight))
                {
                    if (y > yMin && Map.HasFlag(new Vec(x, y-1), TerrainFlag.Sight))
                    {
                        ScanQuatrantH(depth + 1, direction, minSlope, GetSlope(x, y - 0.5, _entityPos.X, _entityPos.Y, direction));
                    }
                }
                else
                {
                    if (y > yMin && !Map.HasFlag(new Vec(x, y - 1), TerrainFlag.Sight))
                        minSlope = GetSlope(x, y - 0.5, _entityPos.X, _entityPos.Y, direction);
                }
            }

            if (Map.HasFlag(new Vec(x, yMax), TerrainFlag.Sight))
                ScanQuatrantH(depth + 1, direction, minSlope, maxSlope);
        }

        /// <summary>
        /// Examine the provided quatrant and calculate the visible cells within it.
        /// </summary>
        /// <param name="depth">Depth of the scan</param>
        /// <param name="direction">1 is south, -1 is north</param>
        /// <param name="minSlope">Start slope of the quatrant</param>
        /// <param name="maxSlope">End slope of the octance</param>
        protected void ScanQuatrantV(int depth, char direction, double minSlope, double maxSlope)
        {
            if (depth > VisualRange)
                return;

            var y = direction == 'N' ? _entityPos.Y - depth : _entityPos.Y + depth;

            if (y < 0 || y >= Visibility.Size.Y)
                return;
            
            var xMin = (int)Math.Floor(Clamp(_entityPos.X + minSlope * depth + 0.49, 0, Visibility.Size.X - 1));
            var xMax = (int)Math.Ceiling(Clamp(_entityPos.X + maxSlope * depth - 0.49, 0, Visibility.Size.X - 1));

            for (var x = xMin; x <= xMax; x++)
            {
                UpdateVisibility(new Vec(x, y));

                if (!Map.HasFlag(new Vec(x, y), TerrainFlag.Sight))
                {
                    if (x > xMin && Map.HasFlag(new Vec(x -1, y), TerrainFlag.Sight))
                    {
                        ScanQuatrantV(depth + 1, direction, minSlope, GetSlope(x - 0.5, y, _entityPos.X, _entityPos.Y, direction));
                    }
                }
                else
                {
                    if (x > xMin && !Map.HasFlag(new Vec(x - 1, y), TerrainFlag.Sight))
                        minSlope = GetSlope(x - 0.5, y, _entityPos.X, _entityPos.Y, direction);
                }
            }

            if (Map.HasFlag(new Vec(xMax, y), TerrainFlag.Sight))
                ScanQuatrantV(depth + 1, direction, minSlope, maxSlope);
        }

        internal void ShowAll(byte flags)
        {
            // This function returns true if it can be seen from any direction.
            bool Reachable(Vec v) {
                for (int y = Math.Max(v.Y - 1, 0); y <= Math.Min(v.Y + 1, Map.Size.Y - 1); y++)
                    for (int x = Math.Max(v.X - 1, 0); x <= Math.Min(v.X + 1, Map.Size.X - 1); x++)
                        if (Map.HasFlag(new Vec(x, y), TerrainFlag.Sight))
                            return true;
                return false;
            }

            // Now update all tiles such that things become visible
            foreach (var v in Visibility.Ids())
            {
                if (Reachable(v))
                    Visibility[v] |= flags;
            }
        }

        private double Clamp(double cur, double min, double max)
        {
            if (cur < min)
                return min;
            if (cur > max)
                return max;
            return cur;
        }

        /// <summary>
        /// Get the gradient of the slope formed by the two points
        /// </summary>
        private double GetSlope(double x1, double y1, double x2, double y2, char direction)
        {
            switch (direction)
            {
                case 'N':
                case 'S':
                    return (x1 - x2) / (Math.Abs(y1 - y2) + 0.5);
                case 'W':
                case 'E':
                    return (y1 - y2) / (Math.Abs(x1 - x2) + 0.5);
                default:
                    throw new ArgumentException($"Character '{direction}' not a valid direction.");
            }
        }
        
        /// <summary>
        /// Calculate the distance between the two points
        /// </summary>
        private void UpdateVisibility(Vec v)
        {
            var dist = (_entityPos - v).Length;
            
            var distanceVisibility = dist < VisualRange ? 1f : 0f;

            Visibility[v] |= SET_VISIBLE;
        }
    }
}
