using HackLib;
using SFML.Graphics;
using SFML.Window;

namespace SurvivalHack
{
    class SfmlGridRenderer : Drawable
    {
        private readonly TileGrid _grid;
        private readonly Sprite _tileSetSprite;

        public SfmlGridRenderer(TileGrid grid)
        {
            _grid = grid;

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
            var area = GetRenderArea(target);

            _tileSetSprite.Scale = new Vector2f(1, 1);

            for (var x = area.Left; x < area.Left + area.Width; x++)
            {
                for (var y = area.Top; y < area.Top + area.Height; y++)
                {
                    var vecScreen = new Vector2f(x * 16, y * 16);

                    _tileSetSprite.Position = vecScreen;

                    var tileId = _grid.Grid[x, y];
                    var tile = TileTypeList.Get(tileId);

                    _tileSetSprite.TextureRect = new IntRect((tile.SourcePos.X) * 16, (tile.SourcePos.Y) * 16, 16, 16);

                    target.Draw(_tileSetSprite);
                }
            }
        }

        private IntRect GetRenderArea(RenderTarget target)
        {
            var screenX = (int)target.Size.X;
            var screenY = (int)target.Size.Y;
            /*
            var corner00 = View.ScreenPxToWens(new Vector2f(0, 0));
            var corner01 = View.ScreenPxToWens(new Vector2f(0, screenY));
            var corner10 = View.ScreenPxToWens(new Vector2f(screenX, 0));
            var corner11 = View.ScreenPxToWens(new Vector2f(screenX, screenY));

            var x0 = Math.Max(corner00.X, 0);
            var x1 = Math.Min(corner11.X + 1, _grid.Width);
            var y0 = Math.Max(corner10.Y, 0);
            var y1 = Math.Min(corner01.Y + 1, _grid.Height);
            */
            return new IntRect(0, 0, screenX / 16 + 1, screenY / 16 + 1);
        }
    }
}
