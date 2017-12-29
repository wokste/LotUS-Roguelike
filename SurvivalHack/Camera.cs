using HackLib;

namespace SurvivalHack
{
    public class Camera
    {
        public int TileSize = 32;

        private Vec _center;

        public Vec WindowSize { get; set; }
        
        private Creature _following;

        public Creature Following {
            get => _following;
            set
            {
                _following = value;
                _center = _following.Position * TileSize + new Vec(TileSize / 2, TileSize / 2);
                // TODO: More stuff
            }
        }

        public Camera(Creature following)
        {
            _following = following;
        }

        public void Update()
        {
            _center = _following.Position * TileSize + new Vec(TileSize / 2, TileSize / 2);
        }

        public Rect GetRenderAreaPx()
        {
            var topLeft = new Vec((int)(_center.X - WindowSize.X / 2f), (int)(_center.Y - WindowSize.Y / 2f));
            return new Rect(topLeft, WindowSize);
        }
    }
}
