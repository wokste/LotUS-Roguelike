﻿using System;
using HackConsole;

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

        protected override void RenderImpl()
        {
            if (!_player.EntityFlags.HasFlag(EEntityFlag.Destroyed))
                _offset = _player.Move.Pos - Size.Center;
            
            Clear();
            RenderGrid();
            RenderCreatures();
        }

        public Action<Entity> OnSelected;

        private void RenderCreatures()
        {
            var area = Size + _offset;
            foreach (var creature in _level.GetEntities(area)) {
                var p = creature.Move.Pos;
                
                if ((_view.Visibility[p] & FieldOfView.FLAG_VISIBLE) == 0)
                    continue;

                p -= _offset;

                if (!Size.Contains(p))
                    continue;

                WindowData.Data[p] = creature.Symbol;
            }
        }

        private void RenderGrid()
        {
            var area = Size.Intersect(new Rect(Vec.Zero - _offset, _level.TileMap.Size));

            foreach (var v in area.Iterator())
            {
                if (!_level.InBoundary(v + _offset))
                    continue;

                var visibility = _view.Visibility[v + _offset];

                if (visibility == 0)
                    continue;

                WindowData.Data[v] = _level.GetTile(v + _offset).Symbol;

                if ((visibility & FieldOfView.FLAG_VISIBLE) != FieldOfView.FLAG_VISIBLE)
                    WindowData.Data[v] = WindowData.Data[v].Darken(128);
            }
        }

        public void OnMouseEvent(Vec mousePos, EventFlags flags)
        {
            // Select creatures, etc
            if (flags.HasFlag(EventFlags.LeftButton | EventFlags.MouseEventPress))
            {
                var absPos = mousePos + _offset;
                if (!_level.InBoundary(absPos) || _view.Visibility[absPos] == 0)
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
