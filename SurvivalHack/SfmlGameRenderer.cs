using System;
using HackLib;
using SFML.Graphics;
using SFML.Window;
using Color = SFML.Graphics.Color;
using Image = SFML.Graphics.Image;

namespace SurvivalHack
{
    class SfmlGameRenderer : Drawable
    {
        private readonly Game _game;
        private readonly FieldOfView _view;
        private readonly Sprite _tileSetSprite;
        private readonly Sprite _creatureSprite;
        private readonly Camera _camera;

        public SfmlGameRenderer(Game game, FieldOfView view, Camera camera)
        {
            _game = game;
            _view = view;
            _camera = camera;

            _tileSetSprite = MakeSprite("tileset32.png");
            _creatureSprite = MakeSprite("player.png");
        }
        
        private Sprite MakeSprite(string texName)
        {
            var image = new Image($"{texName}");
            var texture = new Texture(image);
            return new Sprite
            {
                Texture = texture
            };
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            DrawGrid(target, states);
            DrawCreatures(target, states);
        }

        private void DrawCreatures(RenderTarget target, RenderStates states)
        {
            var areaPx = _camera.GetRenderAreaPx();
            _creatureSprite.Scale = new Vector2f(1, 1);

            foreach (var creature in _game.World.Creatures) {
                var x = creature.Position.X;
                var y = creature.Position.Y;
                
                if (_view.Visibility[x,y] < 128)
                    continue;

                var vecScreen = new Vector2f(x * _camera.TileX - areaPx.X + 8, y * _camera.TileY - areaPx.Y);

                _creatureSprite.Position = vecScreen;

                _creatureSprite.TextureRect = new IntRect((creature.SourcePos.X) * 16, (creature.SourcePos.Y) * 32, 16, 32);

                target.Draw(_creatureSprite);
            }
        }

        private void DrawGrid(RenderTarget target, RenderStates states)
        {
            var grid = _game.World;

            var areaPx = _camera.GetRenderAreaPx();
            var x0 = (int)Math.Floor((float)areaPx.Left / _camera.TileX);
            var y0 = (int)Math.Floor((float)areaPx.Top / _camera.TileY);
            var x1 = (int)Math.Ceiling((float)areaPx.Right / _camera.TileX);
            var y1 = (int)Math.Ceiling((float)areaPx.Bottom / _camera.TileY);

            _tileSetSprite.Scale = new Vector2f(1, 1);

            for (var x = x0; x < x1; x++)
            {
                for (var y = y0; y < y1; y++)
                {
                    if (!_game.World.InBoundary(x, y))
                        continue;

                    var vecScreen = new Vector2f(x * _camera.TileX - areaPx.X, y * _camera.TileY - areaPx.Y);

                    _tileSetSprite.Position = vecScreen;

                    if (_view.Visibility[x, y] == 0)
                        continue;

                    byte v = _view.Visibility[x, y];

                    _tileSetSprite.Color = new Color(v, v, v);

                    var floor = _game.World.GetFloor(x, y);
                    var wall = _game.World.GetWall(x, y);

                    _tileSetSprite.TextureRect = new IntRect((floor.SourcePos.X) * _camera.TileX, (floor.SourcePos.Y) * _camera.TileY, _camera.TileX, _camera.TileY);

                    target.Draw(_tileSetSprite);

                    if (wall != null)
                    {
                        _tileSetSprite.TextureRect = new IntRect((wall.SourcePos.X) * _camera.TileX, (wall.SourcePos.Y) * _camera.TileY, _camera.TileX, _camera.TileY);

                        target.Draw(_tileSetSprite);
                    }
                }
            }
        }
    }
}
