using System;
using System.Drawing;

namespace HackLib
{
    public class FieldOfView
    {
        private Point _playerPos;

        public Point PlayerPos
        {
            get => _playerPos;
            set
            {
                VisibleToDark();
                _playerPos = value;
                ToVisible();
            }
        }

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
        
        public void OnMapUpdate()
        {
            VisibleToDark();
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
                    Visibility[x, y] = Math.Min(Visibility[x, y], (byte)128);
        }
        
        /// <summary>
        /// Start here: go through all the quatrants which surround the player to
        /// determine which open cells are visible
        /// </summary>
        private void ToVisible()
        {
            MarkAsSeen(PlayerPos.X, PlayerPos.Y);
            
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

            var yMin = Clamp(_playerPos.Y + (int)Math.Floor(minSlope * depth + 0.49), 0, Map.Height - 1);
            var yMax = Clamp(_playerPos.Y + (int)Math.Ceiling(maxSlope * depth - 0.49), 0, Map.Height - 1);
            
            for (var y = yMin; y <= yMax; y++)
            {
                if (InSightRadius(x, y))
                    MarkAsSeen(x, y);
                
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
            
            var xMin = Clamp(_playerPos.X + (int)Math.Floor(minSlope * depth + 0.49), 0, Map.Width - 1);
            var xMax = Clamp(_playerPos.X + (int)Math.Ceiling(maxSlope * depth - 0.49), 0, Map.Width - 1);

            for (var x = xMin; x <= xMax; x++)
            {
                if (InSightRadius(x, y))
                    MarkAsSeen(x, y);

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

        private void MarkAsSeen(int x, int y)
        {
            Visibility[x, y] = 255;
        }

        private int Clamp(int cur, int min, int max)
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
        private bool InSightRadius(int x, int y)
        {
            var dx = x - PlayerPos.X;
            var dy = y - PlayerPos.Y;

            return dx * dx + dy * dy < VisualRange * (VisualRange + 1);
        }
    }
}
