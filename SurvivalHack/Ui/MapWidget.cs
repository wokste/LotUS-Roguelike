using System;
using System.Collections.Generic;
using System.Linq;
using HackConsole;
using HackConsole.Algo;
using SFML.Graphics;
using SFML.Window;

namespace SurvivalHack.Ui
{
    public class MapWidget : Widget, IMouseEventSuscriber
    {
        private Level _level;
        private MapView _mapView = new MapView();
        private readonly TurnController _controller;
        private readonly Colour _pathColor = new Colour(0, 128, 255, 128);

        //private Vec _relToAbs;
        private Sprite _entitySprite = new Sprite();

        private IEnumerable<Vec> _path = null;
        private AStar _aStar;

        public MapWidget(TurnController controller)
        {
            _controller = controller;
            _controller.OnTurnEnd += ReactNewTurn;
            ReactNewTurn();
        }


        protected override void OnResized()
        {
            base.OnResized();
            _mapView.OnResize(this, _controller);
        }

        private void ReactNewTurn()
        {
            _path = null;
            if (_level != _controller.Level)
            {
                _level = _controller.Level;
                _aStar = new AStar(_controller.Level.Size, CostFunc, true);
            }
            _mapView.RenderMap(_controller);
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

        // Inspired by https://github.com/thebracket/rltk/blob/master/rltk/virtual_terminal.cpp
        protected override void DrawInternal(RenderTarget target)
        {
            var states = new RenderStates(HackConsole.Ui.Sprites.Tileset);
            _mapView.Draw(target, states);
            DrawCreatures(target, states);
            //RenderPath();
        }

        public Action<Entity> OnSelected;

        private void DrawCreatures(RenderTarget target, RenderStates states)
        {
            int RenderDepth(EEntityFlag flags)
            {
                if (flags.HasFlag(EEntityFlag.IsPlayer))
                    return 10;
                else if (flags.HasFlag(EEntityFlag.Blocking))
                    return 5;
                return 1;
            }

            var area = new Rect(_mapView.RelToAbs, _mapView.VisibleSize);

            foreach (var e in _level.GetEntities(area).OrderBy(e => RenderDepth(e.EntityFlags)))
            {
                var absLoc = _controller.FoV.ShowLocation(e);

                if (absLoc is Vec absLoc2)
                {
                    var relLoc = absLoc2 - _mapView.RelToAbs;

                    if (!_mapView.VisibleSize.Contains(relLoc))
                        continue;

                    var g = e.Glyph;

                    if (g.Method == TileGlyph.ANIM && DateTime.Now.Millisecond < 500)
                        g.X++;

                    _entitySprite.Texture = HackConsole.Ui.Sprites.Tileset;
                    _entitySprite.Position = new Vector2f((relLoc.X * 16), (relLoc.Y * 16));
                    _entitySprite.TextureRect = new IntRect(g.X * 16, g.Y * 16, 16, 16);
                    
                    _entitySprite.Draw(target, states);
                }
            }
        }
        /*
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
        */
        
        public void OnMouseEvent(Vec mousePos, EventFlags flags)
        {
            if (flags.HasFlag(EventFlags.RightButton) && flags.HasFlag(EventFlags.MouseEventPress) && _path != null)
            {
                _controller.Path = _path.ToList();
            }
            else if (flags.HasFlag(EventFlags.LeftButton) & flags.HasFlag(EventFlags.MouseEventPress))
            {
                _controller.ActiveTool?.Apply(_mapView.PxToTile(mousePos));
            }
        }

        public void OnMouseMove(Vec mousePos, Vec mouseMove, EventFlags flags)
        {
            var absPos = _mapView.PxToTile(mousePos);
            if (!_level.InBoundary(absPos) || _controller.FoV.Visibility[absPos] == 0 || _controller.GameOver)
            {
                OnSelected?.Invoke(null);
                _path = null;
                _mapView.RenderMap(_controller);
                return;
            }

            var list = _level.GetEntities(absPos);

            foreach (var e in list)
                if (_controller.FoV.ShowLocation(e) != null)
                    OnSelected?.Invoke(e);

            _path = _aStar.Run(_controller.Player.Pos, absPos);
            _mapView.RenderMap(_controller);
        }

        public void OnMouseWheel(Vec delta, EventFlags flags)
        {
        }

        public class MapView : Drawable
        {
            public Size VisibleSize;
            public Vec RelToAbs;
            public Color VisibleColor = new Color(255, 255, 255, 255);
            public Color KnownColor = new Color(64, 96, 80, 255);
            public Color Black = new Color(0,0,0,0);

            private readonly VertexArray _vertices = new VertexArray();
            protected readonly int _fontX = 16;
            protected readonly int _fontY = 16;

            public MapView()
            {
                _vertices.PrimitiveType = PrimitiveType.Quads;
            }

            internal Vec PxToTile(Vec mousePos) => new Vec(mousePos.X / _fontX, mousePos.Y / _fontY) + RelToAbs;

            public void Draw(RenderTarget target, RenderStates states)
            {
                target.Draw(_vertices, states);
            }

            public void OnResize(Widget widget, TurnController controller)
            {
                var newSize = new Size(widget.Rect.Width / 16, widget.Rect.Height / 16);

                if (VisibleSize == newSize)
                    return;

                VisibleSize = newSize;

                RenderMap(controller);
            }

            public void RenderMap(TurnController controller)
            {
                _vertices.Clear();

                var map = controller.Level;
                var fov = controller.FoV;

                RelToAbs = controller.Player.Pos - VisibleSize.Center;

                foreach (var rel in VisibleSize.Iterator())
                {
                    var abs = rel + RelToAbs;

                    if (!map.InBoundary(abs))
                        continue;

                    var visibility = fov.Visibility[abs];

                    if ((visibility & FieldOfView.FLAG_VISIBLE) != 0)
                        SetGlyph(rel, ChooseGlyph(map, abs), VisibleColor);
                    else if ((visibility & FieldOfView.FLAG_DISCOVERED) != 0)
                        SetGlyph(rel, ChooseGlyph(map, abs), KnownColor);
                }
            }

            TileGlyph ChooseGlyph(Level map, Vec pos)
            {
                var tile = map.GetTile(pos);
                var glyph = tile.Glyph;

                switch (glyph.Method)
                {
                    case TileGlyph.TERRAIN:
                    {
                        var l = map.IsSameTile(pos, pos + new Vec(-1, 0));
                        var r = map.IsSameTile(pos, pos + new Vec(1, 0));

                        if (l && r) glyph.X += 2;
                        if (l && !r) glyph.X += 3;
                        if (!l && r) glyph.X += 1;


                        var t = map.IsSameTile(pos, pos + new Vec(0, -1));
                        var b = map.IsSameTile(pos, pos + new Vec(0, 1));

                        if (t && b) glyph.Y += 2;
                        if (t && !b) glyph.Y += 3;
                        if (!t && b) glyph.Y += 1;
                        break;
                    }
                    case TileGlyph.WALL:
                    {
                        Vec[] dir8 = { new Vec(-1, -1), new Vec(-1, 0), new Vec(-1, 1), new Vec(0, 1), new Vec(1, 1), new Vec(1, 0), new Vec(1, -1), new Vec(0, -1) };
                        var g = dir8.Select(d => map.IsSameTile(pos, pos + d)).ToArray();

                        var t = g[7] && !(g[1] && g[0] && g[6] & g[5]);
                        var b = g[3] && !(g[1] && g[2] && g[4] & g[5]);
                        var l = g[1] && !(g[7] && g[0] && g[2] & g[3]);
                        var r = g[5] && !(g[7] && g[6] && g[4] & g[3]);

                        if (l && r) glyph.X += 2;
                        if (l && !r) glyph.X += 3;
                        if (!l && r) glyph.X += 1;

                        if (t && b) glyph.Y += 2;
                        if (t && !b) glyph.Y += 3;
                        if (!t && b) glyph.Y += 1;
                        break;
                        }
                    case TileGlyph.PIT:
                    {
                        Vec[] dir5 = { new Vec(-1, 0), new Vec(-1, -1), new Vec(0, -1), new Vec(1, -1), new Vec(1, 0)};
                        var g = dir5.Select(d => map.IsSameTile(pos, pos + d)).ToArray();

                        var t = g[2];
                        var l = g[0];
                        var r = g[4];

                        if (l && r) glyph.X += 2;
                        if (l && !r) glyph.X += 3;
                        if (!l && r) glyph.X += 1;

                        if (t) glyph.Y += 1;
                        break;
                    }
                    default:
                        break;
                }
                return glyph;
            }

            void SetGlyph(Vec v, TileGlyph g, Color c)
            {
                var d = new Vector2f[] {
                    new Vector2f(0, 0),
                    new Vector2f(_fontX, 0),
                    new Vector2f(_fontX, _fontY),
                    new Vector2f(0, _fontY),
                };

                var idx = (uint)((v.Y * VisibleSize.X) + v.X) * 4;
                var vecScreen = new Vector2f((v.X * _fontX), (v.Y * _fontY));
                var texPos = new Vector2f(g.X * _fontX, g.Y * _fontY);

                for (uint i = 0; i < 4; ++i)
                {
                    _vertices.Append(new Vertex(vecScreen + d[i], c, texPos + d[i]));
                }
            }
        }
    }
}