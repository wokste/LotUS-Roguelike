using HackLib;
using SFML.Window;
using Size = System.Drawing.Size;
using Rectangle = System.Drawing.Rectangle;

namespace SurvivalHack
{
    public class Camera
    {
        public int TileX = 32;
        public int TileY = 32;

        private Vector2f _center;

        public Size WindowSize { get; set; }
        
        private Creature _following;

        public Creature Following {
            get => _following;
            set
            {
                _following = value;
                _center = new Vector2f((_following.Position.X + 0.5f) * TileX, (_following.Position.Y + 0.5f) * TileY);
                // TODO: More stuff
            }
        }

        public Camera(Creature following)
        {
            _following = following;
        }

        public void Update()
        {
            _center = new Vector2f((_following.Position.X * TileX + 0.5f), (_following.Position.Y + 0.5f) * TileY);
        }

        public Rectangle GetRenderAreaPx()
        {
            var topLeft = new System.Drawing.Point((int)(_center.X - WindowSize.Width / 2f), (int)(_center.Y - WindowSize.Height / 2f));
            return new Rectangle(topLeft, WindowSize);
        }
    }
}
