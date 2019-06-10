using SFML.Graphics;

namespace HackConsole.Ui
{
    public class Sprites
    {
        public static Texture Ascii = MakeSprite("ascii.png");
        public static Texture Tileset = MakeSprite("tileset.png");
        public static BitmapFont Font = new BitmapFont("font2",18,2,3);

        private static Texture MakeSprite(string texName)
        {
            var image = new Image($"{texName}");
            return new Texture(image);
        }
    }
}
