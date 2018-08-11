using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackConsole.Ui
{
    public class Sprites
    {
        public static Texture Ascii = MakeSprite("ascii.png");
        public static Texture Tileset = MakeSprite("tileset.png");

        private static Texture MakeSprite(string texName)
        {
            var image = new Image($"{texName}");
            return new Texture(image);
        }
    }
}
