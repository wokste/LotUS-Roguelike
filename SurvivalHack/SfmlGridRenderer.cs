using System;
using System.Drawing;
using HackLib;
using SFML.Graphics;
using SFML.Window;
using Image = SFML.Graphics.Image;

namespace SurvivalHack
{
    class SfmlGridRenderer : Drawable
    {
        private readonly TileGrid _grid;
        private readonly Sprite _tileSetSprite;
        private readonly Camera _camera;

        public SfmlGridRenderer(TileGrid grid, Camera camera)
        {
            _grid = grid;
            _camera = camera;

            _tileSetSprite = MakeSprite("tileset.png");
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
            var areaPx = _camera.GetRenderAreaPx();
            var x0 = Math.Max(areaPx.Left / _camera.TileX, 0);
            var y0 = Math.Max(areaPx.Top / _camera.TileY, 0);
            var x1 = Math.Min(areaPx.Right / _camera.TileX + 1, _grid.Width);
            var y1 = Math.Min(areaPx.Bottom / _camera.TileY + 1, _grid.Height);

            _tileSetSprite.Scale = new Vector2f(1, 1);

            for (var x = x0; x < x1; x++)
            {
                for (var y = y0; y < y1; y++)
                {
                    var vecScreen = new Vector2f(x * _camera.TileX - areaPx.X, y * _camera.TileY - areaPx.Y);

                    _tileSetSprite.Position = vecScreen;

                    var tile = _grid.Grid[x, y];
                    var sprite = (tile.Wall != null) ? tile.Wall : tile.Floor;

                    _tileSetSprite.TextureRect = new IntRect((sprite.SourcePos.X) * _camera.TileX, (sprite.SourcePos.Y) * _camera.TileY, _camera.TileX, _camera.TileY);

                    target.Draw(_tileSetSprite);
                }
            }
        }
    }
}
