using System;
using System.Collections.Generic;
using System.Linq;
using HackConsole;
using HackConsole.Algo;

namespace SurvivalHack.Ui
{
    public class MapWidget : Widget, IMouseEventSuscriber
    {
        private Level _level;
        private readonly TurnController _controller;
        private readonly Color _pathColor = new Color(0, 128, 255, 128);

        private Vec _offset;
        private IEnumerable<Vec> _path = null;
        private AStar _aStar;

        public MapWidget(TurnController controller)
        {
            _controller = controller;
            _controller.OnTurnEnd += ReactNewTurn;
            ReactNewTurn();
        }

        private void ReactNewTurn()
        {
            _path = null;
            if (_level != _controller.Level)
            {
                _level = _controller.Level;
                _aStar = new AStar(_controller.Level.Size, CostFunc, true);
            }
        }

        private float CostFunc(Vec v)
        {
            if ((_controller.FoV.Visibility[v] & FieldOfView.FLAG_DISCOVERED) == 0)
                return float.PositiveInfinity;

            var tile = _level.GetTile(v);

            if (tile.Solid || tile.WalkDanger > 0)
                return float.PositiveInfinity;

            return 1;
        }

        protected override void RenderImpl()
        {
            if (!_controller.Player.EntityFlags.HasFlag(EEntityFlag.Destroyed))
                _offset = _controller.Player.Pos - Size.Center;
            
            Clear();
            RenderGrid();
            RenderCreatures();
            RenderPath();
        }

        public Action<Entity> OnSelected;

        private void RenderCreatures()
        {
            int RenderDepth(EEntityFlag flags) {
                if (flags.HasFlag(EEntityFlag.IsPlayer))
                    return 10;
                else if (flags.HasFlag(EEntityFlag.Blocking))
                    return 5;
                return 1;
            }

            var area = Size + _offset;
            foreach (var e in _level.GetEntities(area).OrderBy(e => RenderDepth(e.EntityFlags))) {
                var p = _controller.FoV.ShowLocation(e);

                if (p is Vec p2)
                {
                    p2 -= _offset;

                    if (!Size.Contains(p2))
                        continue;

                    WindowData.Data[p2] = new Symbol(e.Symbol.Ascii, e.Symbol.TextColor, WindowData.Data[p2].BackgroundColor);
                }
            }
        }

        private void RenderPath()
        {
            if (_path == null)
                return;

            foreach (var absPos in _path)
            {
                var relPos = absPos - _offset;
                if (!Size.Contains(relPos))
                    continue;

                var s = WindowData.Data[relPos];
                s.TextColor.Add(_pathColor);
                s.BackgroundColor.Add(_pathColor);
                WindowData.Data[relPos] = s;
            }
        }

        private void RenderGrid()
        {
            var area = Size.Intersect(new Rect(Vec.Zero - _offset, _level.TileMap.Size));

            foreach (var v in area.Iterator())
            {
                if (!_level.InBoundary(v + _offset))
                    continue;

                var visibility = _controller.FoV.Visibility[v + _offset];

                if (visibility == 0)
                    continue;

                WindowData.Data[v] = _level.GetTile(v + _offset).Symbol;

                if ((visibility & FieldOfView.FLAG_VISIBLE) != FieldOfView.FLAG_VISIBLE)
                    WindowData.Data[v] = WindowData.Data[v].Darken(128);
            }
        }

        public void OnMouseEvent(Vec mousePos, EventFlags flags)
        {
            if (flags.HasFlag(EventFlags.LeftButton & EventFlags.MouseEventPress) && _path != null)
            {
                _controller.Path = _path.ToList();
            }
        }

        public void OnMouseMove(Vec mousePos, Vec mouseMove, EventFlags flags)
        {
            var absPos = mousePos + _offset;
            if (!_level.InBoundary(absPos) || _controller.FoV.Visibility[absPos] == 0 || _controller.Player == null)
            {
                OnSelected?.Invoke(null);
                _path = null;
                Dirty = true;
                return;
            }

            var list = _level.GetEntities(absPos);

            foreach (var e in list)
                if (_controller.FoV.ShowLocation(e) != null)
                    OnSelected?.Invoke(e);

            _path = _aStar.Run(_controller.Player.Pos, absPos);
            Dirty = true;
        }

        public void OnMouseWheel(Vec delta, EventFlags flags)
        {
        }
    }
}
