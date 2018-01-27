using System;
using HackConsole;

namespace SurvivalHack.Ui
{
    class WorldWidget : Widget
    {
        private World _world;
        private readonly FieldOfView _view;
        private readonly Creature _following;

        private Vec _offset;

        public WorldWidget(World world, FieldOfView view, Creature following)
        {
            _world = world;
            _view = view;
            _following = following;
        }

        public override void Render(bool forceUpdate)
        {
            _offset.X = _following.Position.X - Size.Width / 2 - Size.Left;
            _offset.Y = _following.Position.Y - Size.Height / 2 - Size.Top;

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

                x -= _offset.X;
                y -= _offset.Y;

                if (!Size.Contains(x,y))
                    continue;

                CellGrid.Cells[x, y] = creature.Symbol;
            }
        }

        private void RenderGrid()
        {
            var x0 = Math.Max(Size.Left, 0 - _offset.X);
            var y0 = Math.Max(Size.Top, 0 - _offset.Y);
            var x1 = Math.Min(Size.Right, _world._map.Width - _offset.X);
            var y1 = Math.Min(Size.Bottom, _world._map.Height - _offset.Y);

            for (var x = x0; x < x1; x++)
            {
                for (var y = y0; y < y1; y++)
                {
                    if (!_world.InBoundary(x, y))
                        continue;

                    var visibility = _view.Visibility[x + _offset.X, y + _offset.Y];

                    if (visibility == 0)
                        continue;

                    CellGrid.Cells[x, y] = _world.GetTop(x + _offset.X, y + _offset.Y).Char;
                    if (visibility < 255)
                    {
                        CellGrid.Cells[x, y].TextColor.Darken(visibility);
                        CellGrid.Cells[x, y].BackgroundColor.Darken(visibility);
                    }
                }
            }
        }
    }
}
