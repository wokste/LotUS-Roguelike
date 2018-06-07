using System;
using System.Collections.Generic;
using System.Linq;
using HackConsole;
using HackConsole.Algo;

namespace SurvivalHack.Ui
{
    internal class MapWidget : Widget, IMouseEventSuscriber
    {
        private readonly Level _level;
        private readonly TurnController _controller;
        private readonly Color _pathColor = new Color(0, 128, 255, 128);

        private Vec _offset;
        private IEnumerable<Vec> _path = null;
        private AStar _aStar;

        public MapWidget(Level level, TurnController controller)
        {
            _level = level;
            _controller = controller;
            _controller.OnMove += () => { _path = null; };
            _aStar = new AStar(level.TileMap.Size, CostFunc, true);
        }

        private float CostFunc(Vec v)
        {
            if ((_controller.FoV.Visibility[v] & FieldOfView.FLAG_DISCOVERED) == 0)
                return float.PositiveInfinity;

            if (!_level.TileMap[v].Flags.HasFlag(TerrainFlag.Walk))
                return float.PositiveInfinity;

            return 1;
        }

        protected override void RenderImpl()
        {
            if (!_controller.Player.EntityFlags.HasFlag(EEntityFlag.Destroyed))
                _offset = _controller.Player.Move.Pos - Size.Center;
            
            Clear();
            RenderGrid();
            RenderCreatures();
            RenderPath();
        }

        public Action<Entity> OnSelected;

        private void RenderCreatures()
        {
            var area = Size + _offset;
            foreach (var e in _level.GetEntities(area)) {
                var p = e.Move.Pos;
                
                if (!_controller.FoV.ShouldShow(e))
                    continue;

                p -= _offset;

                if (!Size.Contains(p))
                    continue;

                WindowData.Data[p] = e.Symbol;
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

        }

        public void OnMouseMove(Vec mousePos, Vec mouseMove, EventFlags flags)
        {
            var absPos = mousePos + _offset;
            if (!_level.InBoundary(absPos) || _controller.FoV.Visibility[absPos] == 0 || _controller.Player.Move == null)
            {
                OnSelected?.Invoke(null);
                _path = null;
                Dirty = true;
                return;
            }

            var list = _level.GetEntity(absPos);

            foreach (var e in list)
                if (_controller.FoV.ShouldShow(e))
                    OnSelected?.Invoke(e);

            _path = _aStar.Run(_controller.Player.Move.Pos, absPos);
            Dirty = true;
        }

        public void OnMouseWheel(Vec delta, EventFlags flags)
        {
        }
    }
}
