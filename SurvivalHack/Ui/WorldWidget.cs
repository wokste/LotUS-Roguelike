using System;
using System.Diagnostics;
using HackConsole;
using SurvivalHack.ECM;

namespace SurvivalHack.Ui
{
    internal class WorldWidget : Widget, IMouseEventSuscriber
    {
        private readonly World _world;
        private readonly FieldOfView _view;
        private readonly Player _player;

        private Vec _offset;

        public WorldWidget(World world, FieldOfView view, Player following)
        {
            _world = world;
            _view = view;
            _player = following;
        }

        public override void Render(bool forceUpdate)
        {
            _offset = _player.Position - Size.Center;
            
            Clear();
            RenderGrid();
            RenderCreatures();
        }

        public Action<IDescriptionProvider> OnSelected;
        public Action<int> OnSpendTime;

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
            var x1 = Math.Min(Size.Right, _world.Map.Width - _offset.X);
            var y1 = Math.Min(Size.Bottom, _world.Map.Height - _offset.Y);

            for (var y = y0; y < y1; y++)
            {
                if (!_world.InBoundary(0, y + _offset.Y))
                    continue;

                for (var x = x0; x < x1; x++)
                {
                    if (!_world.InBoundary(x + _offset.X, y + _offset.Y))
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

        public void OnMouseEvent(Vec mousePos, EventFlags flags)
        {
            // Select creatures, etc
            if (flags.HasFlag(EventFlags.LeftButton | EventFlags.MouseEventPress))
            {
                var absPos = mousePos + _offset;
                if (!_world.InBoundary(absPos.X, absPos.Y) || _player.FoV.Visibility[absPos.X, absPos.Y] == 0)
                {
                    OnSelected?.Invoke(null);
                }

                var c = _world.GetCreature(absPos.X, absPos.Y);
                OnSelected?.Invoke(c);
            }
        }

        public void OnMouseMove(Vec mousePos, Vec mouseMove, EventFlags flags)
        {
        }

        public void OnMouseWheel(Vec delta, EventFlags flags)
        {
        }
    }
}
