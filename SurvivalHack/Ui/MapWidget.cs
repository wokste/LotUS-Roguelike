using System;
using System.Collections.Generic;
using System.Linq;
using HackConsole;
using HackConsole.Algo;
using SFML.Graphics;
using SFML.Window;

namespace SurvivalHack.Ui
{
    public class MapWidget : Widget //, IMouseEventSuscriber
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
            _mapView.ResizeVertices(Rect, _controller);
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
        public override void Draw(RenderTarget target)
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

            var area = new Rect(_mapView.RelToAbs, _mapView.Size);

            foreach (var e in _level.GetEntities(area).OrderBy(e => RenderDepth(e.EntityFlags)))
            {
                var absLoc = _controller.FoV.ShowLocation(e);

                if (absLoc is Vec absLoc2)
                {
                    var relLoc = absLoc2 - _mapView.RelToAbs;

                    if (!_mapView.Size.Contains(relLoc))
                        continue;

                    var g = e.Glyph;

                    _entitySprite.Texture = HackConsole.Ui.Sprites.Tileset;
                    _entitySprite.Position = new Vector2f((relLoc.X * 16), (relLoc.Y * 16));
                    _entitySprite.TextureRect = new IntRect(g.X * 16, g.Y * 16, 16, 16);
                    _entitySprite.Draw(target, states);

                    //Data[relLoc] = new Symbol(e.Symbol.Ascii, e.Symbol.TextColor, Data[relLoc].BackgroundColor);
                    

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

        public class MapView : Drawable
        {
            public Size Size;
            public Vec RelToAbs;


            private readonly VertexArray _vertices = new VertexArray();
            protected readonly int _fontX = 16;
            protected readonly int _fontY = 16;


            public void Draw(RenderTarget target, RenderStates states)
            {
                target.Draw(_vertices, states);
            }

            public void ResizeVertices(Rect area, TurnController controller)
            {
                var newSize = new Size(area.Width / 16, area.Height / 16);

                if (Size == newSize)
                    return;

                Size = newSize;

                // Build the vertex buffer
                _vertices.PrimitiveType = PrimitiveType.Quads;
                _vertices.Resize((uint)newSize.Area * 4);

                RenderMap(controller);
            }

            public void RenderMap(TurnController controller)
            {
                var map = controller.Level;
                var fov = controller.FoV;

                RelToAbs = controller.Player.Pos - Size.Center;

                foreach (var rel in Size.Iterator())
                {
                    var abs = rel + RelToAbs;

                    if (!map.InBoundary(abs))
                        continue;

                    var visibility = fov.Visibility[abs];
                    var color = new Color(visibility, visibility, visibility);

                    SetGlyph(rel, ChooseGlyph(map, abs), color);
                }
            }

            TileGlyph ChooseGlyph(Level map, Vec pos)
            {
                var tile = map.GetTile(pos);
                return tile.Glyph;
            }

            void SetGlyph(Vec v, TileGlyph g, Color c)
            {
                var d = new Vector2f[] {
                    new Vector2f(0, 0),
                    new Vector2f(_fontX, 0),
                    new Vector2f(_fontX, _fontY),
                    new Vector2f(0, _fontY),
                };

                var idx = (uint)((v.Y * Size.X) + v.X) * 4;
                var vecScreen = new Vector2f((v.X * _fontX), (v.Y * _fontY));
                var texPos = new Vector2f(g.X * _fontX, g.Y * _fontY);

                for (uint i = 0; i < 4; ++i)
                {
                    _vertices[idx + i] = new Vertex(vecScreen + d[i], c, texPos + d[i]);
                }
            }
        }
    }
}