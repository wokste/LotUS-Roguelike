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
        //private readonly Sprite _tileSetSprite;
        //private readonly Sprite _creatureSprite;
        private readonly Sprite _asciiSprite;
        private readonly Camera _camera;

        public SfmlGameRenderer(Game game, FieldOfView view, Camera camera)
        {
            _game = game;
            _view = view;
            _camera = camera;

            //_tileSetSprite = MakeSprite("tileset.png");
            //_creatureSprite = MakeSprite("player.png");
            _asciiSprite = MakeSprite("ascii.png");
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
            _asciiSprite.Scale = new Vector2f(1, 1);

            foreach (var creature in _game.World.Creatures) {
                var x = creature.Position.X;
                var y = creature.Position.Y;
                
                if (_view.Visibility[x,y] < 128)
                    continue;

                var vecScreen = new Vector2f(x * _camera.TileSize - areaPx.Left, y * _camera.TileSize - areaPx.Top);

                _asciiSprite.Position = vecScreen;

                var Char = creature.Symbol;

                _asciiSprite.Color = new Color((byte)(Char.TextColor >> 16 & 0xff), (byte)(Char.TextColor >> 8 & 0xff), (byte)(Char.TextColor << 0 & 0xff));
                _asciiSprite.TextureRect = new IntRect((Char.Ascii % 16) * _camera.TileSize, (Char.Ascii / 16) * _camera.TileSize, _camera.TileSize, _camera.TileSize);

                target.Draw(_asciiSprite);
            }
        }

        private void DrawGrid(RenderTarget target, RenderStates states)
        {
            var grid = _game.World;

            var areaPx = _camera.GetRenderAreaPx();
            var x0 = (int)Math.Floor((float)areaPx.Left / _camera.TileSize);
            var y0 = (int)Math.Floor((float)areaPx.Top / _camera.TileSize);
            var x1 = (int)Math.Ceiling((float)areaPx.Right / _camera.TileSize);
            var y1 = (int)Math.Ceiling((float)areaPx.Bottom / _camera.TileSize);

            _asciiSprite.Scale = new Vector2f(1, 1);

            for (var x = x0; x < x1; x++)
            {
                for (var y = y0; y < y1; y++)
                {
                    if (!_game.World.InBoundary(x, y))
                        continue;

                    var vecScreen = new Vector2f(x * _camera.TileSize - areaPx.Left, y * _camera.TileSize - areaPx.Top);

                    _asciiSprite.Position = vecScreen;

                    if (_view.Visibility[x, y] == 0)
                        continue;

                    var Char = _game.World.GetTop(x, y).Char;

                    int v = _view.Visibility[x, y];

                    _asciiSprite.Color = new Color((byte)(v * (Char.TextColor >> 16 & 0xff) / 256) , (byte)(v * (Char.TextColor >> 8 & 0xff) / 256), (byte)(v * (Char.TextColor << 0 & 0xff) / 256));
                    _asciiSprite.TextureRect = new IntRect((Char.Ascii % 16) * _camera.TileSize, (Char.Ascii / 16) * _camera.TileSize, _camera.TileSize, _camera.TileSize);
                    
                    target.Draw(_asciiSprite);
                }
            }
        }
    }
}
