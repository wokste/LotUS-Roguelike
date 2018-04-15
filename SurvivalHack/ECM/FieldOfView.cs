using HackConsole;
using SurvivalHack.ECM;
using System;

namespace SurvivalHack
{
    public class FieldOfView
    {
        public const byte BRIGHTNESS_LIGHT = 255;
        public const byte BRIGHTNESS_DARK = 100;

        private Vec _entityPos;

        public TileGrid Map;

        private int _visualRange = 10;

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

        public byte[,] Visibility;

        public FieldOfView(MoveComponent move)
        {
            Update(move);
        }
        
        public void Update(MoveComponent move)
        {
            if (Map != move.Level.Map)
            {
                Map = move.Level.Map;
                Visibility = new byte[Map.Size.X, Map.Size.Y]; // 0 initialized so everything is dark.
            }

            VisibleToDark();
            _entityPos = move.Pos;
            ToVisible();
        }

        private void VisibleToDark()
        {
            var r = new Rect(_entityPos, Vec.One).Grow(_visualRange);
            r = r.Intersect(new Rect(Vec.Zero, Map.Size));

            foreach (var v in r.Iterator())
            {
                Visibility[v.X, v.Y] = Math.Min(Visibility[v.X, v.Y], BRIGHTNESS_DARK);
            }
        }
        
        /// <summary>
        /// Start here: go through all the quatrants which surround the player to
        /// determine which open cells are visible
        /// </summary>
        private void ToVisible()
        {
            Visibility[_entityPos.X, _entityPos.Y] = BRIGHTNESS_LIGHT;
            
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

            if (x < 0 || x >= Map.Size.X)
                return;

            var yMin = (int)Math.Floor(Clamp(_entityPos.Y + minSlope * depth + 0.49, 0, Map.Size.Y - 1));
            var yMax = (int)Math.Ceiling(Clamp(_entityPos.Y + maxSlope * depth - 0.49, 0, Map.Size.Y - 1));

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

            if (y < 0 || y >= Map.Size.Y)
                return;
            
            var xMin = (int)Math.Floor(Clamp(_entityPos.X + minSlope * depth + 0.49, 0, Map.Size.X - 1));
            var xMax = (int)Math.Ceiling(Clamp(_entityPos.X + maxSlope * depth - 0.49, 0, Map.Size.X - 1));

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
            
            Visibility[v.X, v.Y] = (byte)Math.Max(Visibility[v.X, v.Y], BRIGHTNESS_LIGHT * distanceVisibility);
        }
    }
}
