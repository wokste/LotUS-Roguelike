using HackConsole;
using SurvivalHack.ECM;
using System;

namespace SurvivalHack
{
    public class FieldOfView : Component
    {
        public const byte FLAG_DISCOVERED = 0x1;    // True when the player seen the tile
        public const byte FLAG_VISIBLE = 0x2;       // True if the player sees the tile. In general, if something is visible it sould also be discovered.
        public const byte FLAG_ALWAYSVISIBLE = 0x4; // Used in wizards tools. Typically false. In general, if something is always visiblie, it should be visible.

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

        // A 2-dimensional grid of visibility flags.
        public Grid<byte> Visibility;

        public FieldOfView(Entity e)
        {
            Update(e);
        }
        
        public void Update(Entity e)
        {
            if (Map != e.Level)
            {
                Map = e.Level;
                Visibility = new Grid<byte>(Map.TileMap.Size); // 0 initialized so everything is dark.
            }

            VisibleToDark();
            _entityPos = e.Pos;
            ToVisible();
        }

        /// <summary>
        /// This function turns all visible tiles to dark tiles. For optimalization, only tiles around the player are considered.
        /// </summary>
        private void VisibleToDark()
        {
            var r = new Rect(_entityPos, Size.One).Grow(_visualRange);
            r = r.Intersect(Visibility.Size.ToRect());

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
            Visibility[_entityPos] |= SET_VISIBLE;

            ScanQuatrantV(1, 'N', -1.0, 1.0);
            ScanQuatrantV(1, 'S', -1.0, 1.0);
            
            ScanQuatrantH(1, 'W', -1.0, 1.0);
            ScanQuatrantH(1, 'E', -1.0, 1.0);
        }

        public bool Is(Vec pos, byte flag)
        {
            return (Visibility[pos] & flag) == flag;
        }

        public Vec? ShowLocation(Entity e) {
            if (Is(e.Pos, FLAG_VISIBLE) || (Is(e.Pos, FLAG_DISCOVERED) && e.EntityFlags.HasFlag(EEntityFlag.FixedPos)))
            {
                e.LastSeenPos = e.Pos;
                return e.Pos;
            }
            else
            {
                if (e.LastSeenPos is Vec p2 && e.EntityFlags.HasFlag(EEntityFlag.Pickable))
                    return e.LastSeenPos;
                else
                    return null;
            }
        }

        /// <summary>
        /// Calculates the visible cells within a cone recursively. Each recursion step is a single column.
        /// </summary>
        /// <param name="distance">Recursion parameter which is the horizontal distance from the player.</param>
        /// <param name="direction">'E' is east, 'W' is west</param>
        /// <param name="minSlope">Start slope (dy/dx) of the quatrant, in the range: [-1; 1]. Use -1 for first iteration.</param>
        /// <param name="maxSlope">End slope (dy/dx) of the quatrant, in the range: [-1; 1]. Use 1 for first iteration.</param>
        protected void ScanQuatrantH(int distance, char direction, double minSlope, double maxSlope)
        {
            // Algorithm explanation:
            // Initially the cone is 90 degrees, but this may be reduced during recursion due to obstacles.
            // The cone is represented using two angles (minSlope and MaxSlope) in slope notation. Calculation: slope = dy/dx and Tan(slope) = angle. All slopes should be in the range [-1; 1].<para/>
            // If cells are found that block sight, the cone will be reduced accordingly. If there is a visible area left and right , this function calls itself multiple times.
            // Blocked vision may increase minSlope in recursion. If nothing blocks the view, the angles and therefore the slopes are the same. If something blocks the view, the slope can change.

            if (distance > VisualRange)
                return;

            var x = direction == 'W' ? _entityPos.X - distance : _entityPos.X + distance;

            if (x < 0 || x >= Visibility.Size.X)
                return;

            // Calculate the range of visible tiles in the column.
            var yMin = (int)Math.Floor(Clamp(_entityPos.Y + minSlope * distance + 0.49, 0, Visibility.Size.Y - 1));
            var yMax = (int)Math.Ceiling(Clamp(_entityPos.Y + maxSlope * distance - 0.49, 0, Visibility.Size.Y - 1));

            for (var y = yMin; y <= yMax; y++)
            {
                UpdateVisibility(new Vec(x,y)); // Makes things visible

                if (Map.GetTile(new Vec(x, y)).BlockSight)
                {
                    if (y > yMin && !Map.GetTile(new Vec(x, y - 1)).BlockSight)
                    {
                        // Found end of visible area.
                        ScanQuatrantH(distance + 1, direction, minSlope, GetSlope(x, y - 0.5, _entityPos.X, _entityPos.Y, direction));
                    }
                }
                else
                {
                    if (y > yMin && Map.GetTile(new Vec(x, y - 1)).BlockSight)
                    {
                        // Found start of visible area.
                        minSlope = GetSlope(x, y - 0.5, _entityPos.X, _entityPos.Y, direction);
                    }
                }
            }

            if (!Map.GetTile(new Vec(x, yMax)).BlockSight)
            {
                // This visible area is not processed yet. Do it now.
                ScanQuatrantH(distance + 1, direction, minSlope, maxSlope);
            }
        }

        /// <summary>
		/// Calculates the visible cells within a cone recursively. Each recursion step is a single row.
        /// </summary>
        /// <param name="distance">Recursion parameter which is the vertical distance from the player.</param>
        /// <param name="direction">'S' is south, 'N' is north</param>
        /// <param name="minSlope">Start slope (dx/dy) of the quatrant, in the range: [-1; 1]. Use -1 for first iteration.</param>
        /// <param name="maxSlope">End slope (dx/dy) of the quatrant, in the range: [-1; 1]. Use 1 for first iteration.</param>
        protected void ScanQuatrantV(int distance, char direction, double minSlope, double maxSlope)
        {
            // Algorithm explanation:
            // Initially the cone is 90 degrees, but this may be reduced during recursion due to obstacles.
            // The cone is represented using two angles (minSlope and MaxSlope) in slope notation. Calculation: slope = dx/dy and Tan(slope) = angle. All slopes should be in the range [-1; 1].<para/>
            // If cells are found that block sight, the cone will be reduced accordingly. If there is a visible area left and right , this function calls itself multiple times.
            // Blocked vision may increase minSlope in recursion. If nothing blocks the view, the angles and therefore the slopes are the same. If something blocks the view, the slope can change.
            if (distance > VisualRange)
                return;

            var y = direction == 'N' ? _entityPos.Y - distance : _entityPos.Y + distance;

            if (y < 0 || y >= Visibility.Size.Y)
                return;

            // Calculate the range of visible tiles in the row.
            var xMin = (int)Math.Floor(Clamp(_entityPos.X + minSlope * distance + 0.49, 0, Visibility.Size.X - 1));
            var xMax = (int)Math.Ceiling(Clamp(_entityPos.X + maxSlope * distance - 0.49, 0, Visibility.Size.X - 1));

            for (var x = xMin; x <= xMax; x++)
            {
                UpdateVisibility(new Vec(x, y)); // Makes things visible

                if (Map.GetTile(new Vec(x, y)).BlockSight)
                {
                    if (x > xMin && !Map.GetTile(new Vec(x - 1, y)).BlockSight)
                    {
                        // Found end of visible area.
                        ScanQuatrantV(distance + 1, direction, minSlope, GetSlope(x - 0.5, y, _entityPos.X, _entityPos.Y, direction));
                    }
                }
                else
                {
                    if (x > xMin && Map.GetTile(new Vec(x - 1, y)).BlockSight)
                    {
                        // Found start of visible area.
                        minSlope = GetSlope(x - 0.5, y, _entityPos.X, _entityPos.Y, direction);
                    }
                }
            }

            if (!Map.GetTile(new Vec(xMax, y)).BlockSight)
            {
                // This visible area is not processed yet. Do it now.
                ScanQuatrantV(distance + 1, direction, minSlope, maxSlope);
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
        /// <returns>(dx/dy) for horizontal directions. (dy/dx) for vertical directions.</returns>
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
