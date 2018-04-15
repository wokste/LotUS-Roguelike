﻿using System;
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
            var area = Size.Intersect(new Rect(Vec.Zero - _offset, _level.Map.Size));

            foreach (var v in area.Iterator())
            {
                if (!_level.InBoundary(v + _offset))
                    continue;

                var visibility = _view.Visibility[v.X + _offset.X, v.Y + _offset.Y];

                if (visibility == 0)
                    continue;

                CellGrid.Cells[v.X, v.Y] = _level.GetTile(v + _offset).Char;
                if (visibility < 255)
                {
                    CellGrid.Cells[v.X, v.Y].TextColor.Darken(visibility);
                    CellGrid.Cells[v.X, v.Y].BackgroundColor.Darken(visibility);
                }
            }
        }

        public void OnMouseEvent(Vec mousePos, EventFlags flags)
        {
            // Select creatures, etc
            if (flags.HasFlag(EventFlags.LeftButton | EventFlags.MouseEventPress))
            {
                var absPos = mousePos + _offset;
                if (!_level.InBoundary(absPos) || _player.FoV.Visibility[absPos.X, absPos.Y] == 0)
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
