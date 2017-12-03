using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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

        private int _visualRange = 16;
        private int VisualRangeSq => _visualRange * _visualRange;

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

        public FieldOfView(TileGrid map)
        {
            Map = map;
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

            for (int x = x0; x < x1; x++)
                for (int y = y0; y < y1; y++)
                    if (Map.Grid[x, y].Visibility == TileVisibility.Visible)
                        Map.Grid[x, y].Visibility = TileVisibility.Dark;
        }
        
        /// <summary>
        /// Start here: go through all the quatrants which surround the player to
        /// determine which open cells are visible
        /// </summary>
        private void ToVisible()
        {
            MarkAsSeen(PlayerPos.X, PlayerPos.Y);

            ScanQuatrantV(1, -1, -1.0, 1.0);
            ScanQuatrantV(1, 1, -1.0, 1.0);

            ScanQuatrantH(1, -1, -1.0, 1.0);
            ScanQuatrantH(1, 1, -1.0, 1.0);
        }

        /// <summary>
        /// Examine the provided quatrant and calculate the visible cells within it.
        /// </summary>
        /// <param name="pDepth">Depth of the scan</param>
        /// <param name="direction">1 is east, -1 is west</param>
        /// <param name="pStartSlope">Start slope of the quatrant</param>
        /// <param name="pEndSlope">End slope of the octance</param>
        protected void ScanQuatrantH(int pDepth, int direction, double pStartSlope, double pEndSlope)
        {
            for (; pDepth < _visualRange; pDepth++)
            {
                int x = _playerPos.X + pDepth * direction;

                if (x < 0 || x >= Map.Width)
                    return;

                var y0 = Clamp(_playerPos.Y + (int)(pStartSlope * pDepth), 0, Map.Height);
                var y1 = Clamp(_playerPos.Y + (int)(pEndSlope * pDepth), 0, Map.Height);

                for (int y = y0; y <= y1; y++)
                {
                    //if (!GetVisDistance(x, y, player.X, player.Y) <= VisualRangeSq)
                    //	continue;

                    MarkAsSeen(x, y);

                    if (Map.Grid[x, y].BlocksSights)
                    {
                        if (y > y0 && !Map.Grid[x, y - 1].BlocksSights)
                        {
                            ScanQuatrantH(pDepth + 1, direction, pStartSlope, GetSlope(x, y, _playerPos.X, _playerPos.Y, true));
                        }
                    }
                    else
                    {
                        if (y > y0 && Map.Grid[x, y - 1].BlocksSights)
                            pStartSlope = -GetSlope(x, y, _playerPos.X, _playerPos.Y, true);

                        
                    }
                }

                if (Map.Grid[x, y1].BlocksSights)
                    break;
            }
        }

        /// <summary>
        /// Examine the provided quatrant and calculate the visible cells within it.
        /// </summary>
        /// <param name="pDepth">Depth of the scan</param>
        /// <param name="direction">1 is south, -1 is north</param>
        /// <param name="pStartSlope">Start slope of the quatrant</param>
        /// <param name="pEndSlope">End slope of the octance</param>
        protected void ScanQuatrantV(int pDepth, int direction, double pStartSlope, double pEndSlope)
        {
            for (; pDepth < _visualRange; pDepth++)
            {
                int y = _playerPos.Y + pDepth * direction;

                if (y < 0 || y >= Map.Width)
                    return;

                var x0 = Clamp(_playerPos.X + (int)(pStartSlope * pDepth), 0, Map.Width);
                var x1 = Clamp(_playerPos.X + (int)(pEndSlope * pDepth), 0, Map.Width);

                for (int x = x0; x <= x1; x++)
                {
                    //if (!GetVisDistance(x, y, player.X, player.Y) <= VisualRangeSq)
                    //	continue;

                    MarkAsSeen(x, y);

                    if (Map.Grid[x, y].BlocksSights)
                    {
                        if (x > x0 && !Map.Grid[x - 1, y].BlocksSights)
                        {
                            ScanQuatrantV(pDepth + 1, direction, pStartSlope, GetSlope(x, y, _playerPos.X, _playerPos.Y, false));
                        }
                    }
                    else
                    {
                        if (x > x0 && Map.Grid[x - 1, y].BlocksSights)
                            pStartSlope = -GetSlope(x, y, _playerPos.X, _playerPos.Y, false);

                        
                    }
                }

                if (Map.Grid[x1, y].BlocksSights)
                    break;
            }
        }

        private void MarkAsSeen(int x, int y)
        {
            Map.Grid[x, y].Visibility = TileVisibility.Visible;
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
        private double GetSlope(double pX1, double pY1, double pX2, double pY2, bool pInvert)
        {
            if (pInvert)
                return (pY1 - pY2) / (pX1 - pX2);
            else
                return (pX1 - pX2) / (pY1 - pY2);
        }
        
        /// <summary>
        /// Calculate the distance between the two points
        /// </summary>
        private int GetVisDistance(int pX1, int pY1, int pX2, int pY2)
        {
            return ((pX1 - pX2) * (pX1 - pX2)) + ((pY1 - pY2) * (pY1 - pY2));
        }
    }
}
