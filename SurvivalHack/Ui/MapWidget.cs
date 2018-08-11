using System;
using System.Collections.Generic;
using System.Linq;
using HackConsole;
using HackConsole.Algo;
using SFML.Graphics;

namespace SurvivalHack.Ui
{
    public class MapWidget : GridWidget //, IMouseEventSuscriber
    {
        private Level _level;
        private readonly TurnController _controller;
        private readonly Colour _pathColor = new Colour(0, 128, 255, 128);

        //private Vec _offset;
        private Vec _relToAbs;
        //private Transform _screenToGame;
        //private Transform _gameToScreen;

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
            Dirty = true;
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

        protected override void Render()
        {
            if (!_controller.GameOver)
            {
                //var t = new Transform();
                //t.Translate(Rect.Center);
                //t.Scale(16, 16);
                //t.Translate(_controller.Player.Pos);

                _relToAbs = _controller.Player.Pos - Data.Size.Center;

                //_screenToGame = t;
                //_gameToScreen = t.GetInverse();
            }
            
            Clear(Colour.Black);
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

            var area = new Rect(_relToAbs, Data.Size);

            foreach (var e in _level.GetEntities(area).OrderBy(e => RenderDepth(e.EntityFlags))) {
                var absLoc = _controller.FoV.ShowLocation(e);

                if (absLoc is Vec absLoc2)
                {
                    var relLoc = absLoc2 - _relToAbs;

                    if (!Data.Size.Contains(relLoc))
                        continue;

                    Data[relLoc] = new Symbol(e.Symbol.Ascii, e.Symbol.TextColor, Data[relLoc].BackgroundColor);
                }
            }
        }

        private void RenderPath()
        {
            if (_path == null)
                return;

            foreach (var absPos in _path)
            {
                var relPos = absPos - _relToAbs;
                if (!Rect.Size.Contains(relPos))
                    continue;

                var s = Data[relPos];
                s.TextColor.Add(_pathColor);
                s.BackgroundColor.Add(_pathColor);
                Data[relPos] = s;
            }
        }

        private void RenderGrid()
        {
            var area = Rect.Size;

            foreach (var rel in Data.Ids())
            {
                var abs = rel + _relToAbs;

                if (!_level.InBoundary(abs))
                    continue;

                var visibility = _controller.FoV.Visibility[abs];

                if (visibility == 0)
                    continue;

                Data[rel] = _level.GetTile(abs).Symbol;

                if ((visibility & FieldOfView.FLAG_VISIBLE) != FieldOfView.FLAG_VISIBLE)
                    Data[rel] = Data[rel].Darken(128);
            }
        }

        /*
        public void OnMouseEvent(Vec mousePos, EventFlags flags)
        {
            if (flags.HasFlag(EventFlags.RightButton) && flags.HasFlag(EventFlags.MouseEventPress) && _path != null)
            {
                _controller.Path = _path.ToList();
            }
            else if (flags.HasFlag(EventFlags.LeftButton) & flags.HasFlag(EventFlags.MouseEventPress))
            {
                _controller.ActiveTool?.Apply(mousePos + _relToAbs);
            }
        }

        public void OnMouseMove(Vec mousePos, Vec mouseMove, EventFlags flags)
        {
            var absPos = mousePos + _offset;
            if (!_level.InBoundary(absPos) || _controller.FoV.Visibility[absPos] == 0 || _controller.GameOver)
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
        */
    }
}
