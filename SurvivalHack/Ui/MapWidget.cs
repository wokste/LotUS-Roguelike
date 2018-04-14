using System;
using HackConsole;
using SurvivalHack.ECM;

namespace SurvivalHack.Ui
{
    internal class MapWidget : Widget, IMouseEventSuscriber
    {
        private readonly Level _level;
        private readonly FieldOfView _view;
        private readonly Entity _player;

        private Vec _offset;

        public MapWidget(Level level, FieldOfView view, Entity following)
        {
            _level = level;
            _view = view;
            _player = following;
        }

        public override void Render(bool forceUpdate)
        {
            _offset = _player.Move.Pos - Size.Center;
            
            Clear();
            RenderGrid();
            RenderCreatures();
        }

        public Action<IDescriptionProvider> OnSelected;
        public Action<int> OnSpendTime;

        private void RenderCreatures()
        {
            var area = Size + _offset;
            foreach (var creature in _level.GetEntities(area)) {
                var p = creature.Move.Pos;
                
                if (_view.Visibility[p.X,p.Y] < 128)
                    continue;

                p -= _offset;

                if (!Size.Contains(p))
                    continue;

                CellGrid.Cells[p.X, p.Y] = creature.Symbol;
            }
        }

        private void RenderGrid()
        {
            var x0 = Math.Max(Size.Left, 0 - _offset.X);
            var y0 = Math.Max(Size.Top, 0 - _offset.Y);
            var x1 = Math.Min(Size.Right, _level.Map.Width - _offset.X);
            var y1 = Math.Min(Size.Bottom, _level.Map.Height - _offset.Y);

            for (var y = y0; y < y1; y++)
            {
                if (!_level.InBoundary(0, y + _offset.Y))
                    continue;

                for (var x = x0; x < x1; x++)
                {
                    if (!_level.InBoundary(x + _offset.X, y + _offset.Y))
                        continue;

                    var visibility = _view.Visibility[x + _offset.X, y + _offset.Y];

                    if (visibility == 0)
                        continue;

                    CellGrid.Cells[x, y] = _level.GetTop(x + _offset.X, y + _offset.Y).Char;
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
                if (!_level.InBoundary(absPos.X, absPos.Y) || _player.FoV.Visibility[absPos.X, absPos.Y] == 0)
                {
                    OnSelected?.Invoke(null);
                }

                var list = _level.GetEntity(absPos);

                foreach(var e in list)
                    OnSelected?.Invoke(e);
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
