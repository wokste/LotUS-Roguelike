using System;
using System.Drawing;
using HackLib;
using SFML.Graphics;
using SFML.Window;

namespace SurvivalHack
{
    public class Camera
    {
        public int TileX = 16;
        public int TileY = 16;

        private Vector2f _center;

        public Size WindowSize { get; set; }
        
        private Creature _following;

        public Creature Following {
            get => _following;
            set
            {
                _following = value;
                _center = new Vector2f(_following.Position.X * TileX, _following.Position.Y * TileY);
                // TODO: More stuff
            }
        }

        public Camera(Creature following)
        {
            _following = following;
        }

        public void Update()
        {
            _center = new Vector2f(_following.Position.X * TileX, _following.Position.Y * TileY);
        }

        public Rectangle GetRenderAreaPx()
        {
            var topLeft = new Point((int)(_center.X - WindowSize.Width / 2f), (int)(_center.Y - WindowSize.Height / 2f));
            return new Rectangle(topLeft, WindowSize);
        }
    }
}
