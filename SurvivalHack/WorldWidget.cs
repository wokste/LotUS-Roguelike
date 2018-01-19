using System;
using HackConsole;

namespace SurvivalHack
{
    class WorldWidget : Widget
    {
        private World _world;
        private readonly FieldOfView _view;
        private readonly Creature _following;

        private int offsetX;
        private int offsetY;

        public WorldWidget(World world, FieldOfView view, Creature following)
        {
            _world = world;
            _view = view;
            _following = following;
        }

        public override void Render()
        {
            offsetX = _following.Position.X - Size.Width / 2 - Size.Left;
            offsetY = _following.Position.Y - Size.Height / 2 - Size.Top;

            RenderGrid();
            RenderCreatures();
        }

        private void RenderCreatures()
        {
            foreach (var creature in _world.Creatures) {
                var x = creature.Position.X;
                var y = creature.Position.Y;
                
                if (_view.Visibility[x,y] < 128)
                    continue;

                x -= offsetX;
                y -= offsetY;

                if (!Size.Contains(x,y))
                    continue;

                CellGrid.Cells[x, y] = creature.Symbol;
            }
        }

        private void RenderGrid()
        {
            var x0 = Math.Max(Size.Left, 0 - offsetX);
            var y0 = Math.Max(Size.Top, 0 - offsetY);
            var x1 = Math.Min(Size.Right, _world._map.Width - offsetX);
            var y1 = Math.Min(Size.Bottom, _world._map.Height - offsetY);

            for (var x = x0; x < x1; x++)
            {
                for (var y = y0; y < y1; y++)
                {
                    if (!_world.InBoundary(x, y))
                        continue;
                    
                    if (_view.Visibility[x + offsetX, y + offsetY] == 0)
                        continue;

                    CellGrid.Cells[x, y] = _world.GetTop(x + offsetX, y + offsetY).Char;
                }
            }
        }
    }
}
