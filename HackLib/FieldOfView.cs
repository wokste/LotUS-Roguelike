using System;


namespace HackLib
{
    public class FieldOfView
    {
        public const byte BRIGHTNESS_LIGHT = 255;
        public const byte BRIGHTNESS_DARK = 100;

        private Vec _playerPos;

        //public Vec PlayerPos => _playerPos;

        public readonly TileGrid Map;

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

        public FieldOfView(TileGrid map)
        {
            Map = map;
            Visibility = new byte[map.Width,map.Height]; // 0 initialized so everything is dark.
        }
        
        public void Update(Vec playerPos)
        {
            VisibleToDark();
            _playerPos = playerPos;
            ToVisible();
        }

        private void VisibleToDark()
        {
            var x0 = Math.Max(_playerPos.X - _visualRange, 0);
            var x1 = Math.Min(_playerPos.X + _visualRange + 1, Map.Width);

            var y0 = Math.Max(_playerPos.Y - _visualRange, 0);
            var y1 = Math.Min(_playerPos.Y + _visualRange + 1, Map.Height);

            for (var x = x0; x < x1; x++)
                for (var y = y0; y < y1; y++)
                    Visibility[x, y] = Math.Min(Visibility[x, y], BRIGHTNESS_DARK);
        }
        
        /// <summary>
        /// Start here: go through all the quatrants which surround the player to
        /// determine which open cells are visible
        /// </summary>
        private void ToVisible()
        {
            Visibility[_playerPos.X, _playerPos.Y] = BRIGHTNESS_LIGHT;
            
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

            var x = direction == 'W' ? _playerPos.X - depth : _playerPos.X + depth;

            if (x < 0 || x >= Map.Width)
                return;

            var yMin = (int)Math.Floor(Clamp(_playerPos.Y + minSlope * depth + 0.49, 0, Map.Height - 1));
            var yMax = (int)Math.Ceiling(Clamp(_playerPos.Y + maxSlope * depth - 0.49, 0, Map.Height - 1));

            for (var y = yMin; y <= yMax; y++)
            {
                UpdateVisibility(x,y);

                if (Map.HasFlag(x, y, TerrainFlag.BlockSight))
                {
                    if (y > yMin && !Map.HasFlag(x, y-1, TerrainFlag.BlockSight))
                    {
                        ScanQuatrantH(depth + 1, direction, minSlope, GetSlope(x, y - 0.5, _playerPos.X, _playerPos.Y, direction));
                    }
                }
                else
                {
                    if (y > yMin && Map.HasFlag(x, y - 1, TerrainFlag.BlockSight))
                        minSlope = GetSlope(x, y - 0.5, _playerPos.X, _playerPos.Y, direction);
                }
            }

            if (!Map.HasFlag(x, yMax, TerrainFlag.BlockSight))
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

            var y = direction == 'N' ? _playerPos.Y - depth : _playerPos.Y + depth;

            if (y < 0 || y >= Map.Width)
                return;
            
            var xMin = (int)Math.Floor(Clamp(_playerPos.X + minSlope * depth + 0.49, 0, Map.Width - 1));
            var xMax = (int)Math.Ceiling(Clamp(_playerPos.X + maxSlope * depth - 0.49, 0, Map.Width - 1));

            for (var x = xMin; x <= xMax; x++)
            {
                UpdateVisibility(x, y);

                    if (Map.HasFlag(x, y, TerrainFlag.BlockSight))
                {
                    if (x > xMin && !Map.HasFlag(x-1, y, TerrainFlag.BlockSight))
                    {
                        ScanQuatrantV(depth + 1, direction, minSlope, GetSlope(x - 0.5, y, _playerPos.X, _playerPos.Y, direction));
                    }
                }
                else
                {
                    // TODO: Wut? x > XMin?
                    if (x > xMin && Map.HasFlag(x - 1, y, TerrainFlag.BlockSight))
                        minSlope = GetSlope(x - 0.5, y, _playerPos.X, _playerPos.Y, direction);
                }
            }

            if (!Map.HasFlag(xMax, y, TerrainFlag.BlockSight))
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
            //TODO: Check this. Is the abs at the correct location?
        }
        
        /// <summary>
        /// Calculate the distance between the two points
        /// </summary>
        private void UpdateVisibility(int x, int y)
        {
            var dx = x - _playerPos.X;
            var dy = y - _playerPos.Y;

            var dist = Math.Sqrt(dx * dx + dy * dy);
            //var antiAliasRadius = 2f;
            //var distanceVisibility = dist < VisualRange - antiAliasRadius ? 1f : (VisualRange - dist) / antiAliasRadius;
            
            var distanceVisibility = dist < VisualRange ? 1f : 0f;//(VisualRange - dist) / antiAliasRadius;
            
            Visibility[x, y] = (byte)Math.Max(Visibility[x, y], BRIGHTNESS_LIGHT * distanceVisibility);
        }
    }
}
