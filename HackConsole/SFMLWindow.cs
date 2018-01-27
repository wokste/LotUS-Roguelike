using System;
using SFML.Graphics;
using SFML.Window;

namespace HackConsole
{
    public class SfmlWindow
    {
        private uint _windowWidth = 1280;
        private uint _windowHeight = 800;

        private readonly uint _fontX = 16;
        private readonly uint _fontY = 16;

        public WidgetContainer Widgets = new WidgetContainer{Docking = Docking.Fill}; 
        private readonly RenderWindow _window;
        private readonly Sprite _fontSprite;

        public IInputReader Focus;
        public Action OnUpdate;

        public SfmlWindow(string name)
        {
            var contextSettings = new ContextSettings
            {
                DepthBits = 24
            };

            _window = new RenderWindow(new VideoMode(_windowWidth, _windowHeight), name, Styles.Default, contextSettings);
            _window.SetActive();

            _window.SetVisible(true);
            _window.SetVerticalSyncEnabled(true);

            _window.Closed += OnClosed;
            _window.KeyPressed += OnKeyPressed;
            _window.Resized += OnResized;

            _window.SetFramerateLimit(60);

            _fontSprite = MakeSprite("ascii.png");

            OnResized(null, new SizeEventArgs(new SizeEvent {Width = _windowWidth, Height = _windowHeight }));
        }

        private void OnResized(object sender, SizeEventArgs e)
        {
            _windowWidth = e.Width;
            _windowHeight = e.Height;
            CellGrid.Resize(_windowWidth / _fontX, _windowHeight / _fontY);

            _window.SetView(new View(new FloatRect(0, 0, e.Width, e.Height)));

            var r = new Rect{Width = (int)CellGrid.Width, Height = (int)CellGrid.Height};
            Widgets.Resize(ref r);
        }

        private void OnKeyPressed(object sender, KeyEventArgs keyEventArgs)
        {
            EventFlags flags = EventFlags.None;
            if (keyEventArgs.Alt) flags |= EventFlags.Alt;
            if (keyEventArgs.Control) flags |= EventFlags.Ctrl;
            if (keyEventArgs.Shift) flags |= EventFlags.Shift;

            if (keyEventArgs.Code >= Keyboard.Key.Numpad1 && keyEventArgs.Code <= Keyboard.Key.Numpad9)
            {
                // Arrow key movements.
                var id = keyEventArgs.Code - Keyboard.Key.Numpad1;
                var move = new Vec(id % 3 - 1, 1 - id / 3);
                
                Focus?.OnArrowPress(move, flags);
            }
            else
            {
                // Normal movement
                Focus?.OnKeyPress((char)keyEventArgs.Code, flags);
            }
        }

        private void OnClosed(object sender, EventArgs e)
        {
            _window.Close();
        }

        public void Run()
        {
            while (_window.IsOpen())
            {
                // Dispatch events to work with native event loop
                _window.DispatchEvents();

                OnUpdate?.Invoke();
                Render();
            }
        }

        private void Render()
        {
            _window.Clear();

            Widgets.Render(false); // TODO: First frame and when removing popups needs to be true;
            DrawGrid(_window, new RenderStates());

            _window.Display();
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

        private void DrawGrid(RenderTarget target, RenderStates states)
        {
            _fontSprite.Scale = new Vector2f(1, 1);

            for (var x = 0; x < CellGrid.Width; x++)
            {
                for (var y = 0; y < CellGrid.Height; y++)
                {
                    var vecScreen = new Vector2f(x * _fontX, y * _fontY);

                    _fontSprite.Position = vecScreen;
                    
                    var Char = CellGrid.Cells[x,y];
                    
                    _fontSprite.Color = new SFML.Graphics.Color(Char.TextColor.R, Char.TextColor.G, Char.TextColor.B);
                    _fontSprite.TextureRect = new IntRect((int)((Char.Ascii % 16) * _fontX), (int)((Char.Ascii / 16) * _fontY), (int)_fontX, (int)_fontY);
                    target.Draw(_fontSprite);
                }
            }
        }
    }
}