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

        public IKeyEventSuscriber BaseKeyHandler;
        public Action OnUpdate;

        private bool _dirty;
        private Vec _lastMousePos = Vec.NaV;

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
            _window.MouseButtonPressed += OnMouseButtonPressed;
            _window.MouseButtonReleased += OnMouseButtonReleased;
            _window.MouseMoved += OnMouseMoved;
            _window.MouseWheelMoved += OnMouseWheelMoved;

            _window.SetFramerateLimit(60);

            _fontSprite = MakeSprite("ascii.png");

            OnResized(null, new SizeEventArgs(new SizeEvent {Width = _windowWidth, Height = _windowHeight }));
        }

        EventFlags MakeFlags(bool keys, bool mouse) {
            var flags = EventFlags.None;

            if (keys)
            {
                if (Keyboard.IsKeyPressed(Keyboard.Key.RAlt) || Keyboard.IsKeyPressed(Keyboard.Key.LAlt))
                    flags |= EventFlags.Alt;
                if (Keyboard.IsKeyPressed(Keyboard.Key.RControl) || Keyboard.IsKeyPressed(Keyboard.Key.LControl))
                    flags |= EventFlags.Ctrl;
                if (Keyboard.IsKeyPressed(Keyboard.Key.RShift) || Keyboard.IsKeyPressed(Keyboard.Key.LShift))
                    flags |= EventFlags.Shift;
            }

            if (mouse)
            {
                if (Mouse.IsButtonPressed(Mouse.Button.Left))
                    flags |= EventFlags.LeftButton;
                if (Mouse.IsButtonPressed(Mouse.Button.Middle))
                    flags |= EventFlags.MidButton;
                if (Mouse.IsButtonPressed(Mouse.Button.Right))
                    flags |= EventFlags.RightButton;
            }
            return flags;
        }

        Widget WidgetAtPosition(Vec mousePos, WidgetContainer container = null)
        {
            if (container == null)
                container = Widgets;

            Widget top = null;

            foreach (var w in container.Widgets)
                if (w.Size.Contains(mousePos.X, mousePos.Y))
                    top = w;

            return top is WidgetContainer ? WidgetAtPosition(mousePos, top as WidgetContainer) : top;
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
            var flags = MakeFlags(false, true);
            if (keyEventArgs.Alt) flags |= EventFlags.Alt;
            if (keyEventArgs.Control) flags |= EventFlags.Ctrl;
            if (keyEventArgs.Shift) flags |= EventFlags.Shift;

            var handler = BaseKeyHandler;

            if (keyEventArgs.Code == Keyboard.Key.Up)
                handler.OnArrowPress(new Vec(0, -1), flags);
            else if (keyEventArgs.Code == Keyboard.Key.Down)
                handler.OnArrowPress(new Vec(0, 1), flags);
            else if (keyEventArgs.Code == Keyboard.Key.Left)
                handler.OnArrowPress(new Vec(-1, 0), flags);
            else if (keyEventArgs.Code == Keyboard.Key.Right)
                handler.OnArrowPress(new Vec(1, 0), flags);
            else if (keyEventArgs.Code >= Keyboard.Key.Numpad1 && keyEventArgs.Code <= Keyboard.Key.Numpad9)
            {
                // Arrow key movements.
                var id = keyEventArgs.Code - Keyboard.Key.Numpad1;
                var move = new Vec(id % 3 - 1, 1 - id / 3);

                handler.OnArrowPress(move, flags);
            }
            else
            {
                // Normal keys
                handler.OnKeyPress((char)keyEventArgs.Code, flags);
            }
        }

        private void OnMouseWheelMoved(object sender, MouseWheelEventArgs e)
        {
            var mousePos = new Vec((int)(e.X / _fontX), (int)(e.Y / _fontY));
            var delta = new Vec(0, e.Delta > 0 ? 1 : -1);
            
            var widget = WidgetAtPosition(mousePos);
            (widget as IMouseEventSuscriber)?.OnMouseWheel(delta, MakeFlags(true, true));
        }

        private void OnMouseMoved(object sender, MouseMoveEventArgs e)
        {
            var mousePos = new Vec((int)(e.X / _fontX), (int)(e.Y / _fontY));
            var move = (_lastMousePos == Vec.NaV) ? Vec.Zero : (mousePos - _lastMousePos);
            _lastMousePos = mousePos;

            if (move == Vec.Zero)
                return;

            var widget = WidgetAtPosition(mousePos);
            (widget as IMouseEventSuscriber)?.OnMouseMove(mousePos, move, MakeFlags(true, true));
        }

        private void OnMouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            OnMouseButton(e, false);
        }

        private void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            OnMouseButton(e, true);
        }

        private void OnMouseButton(MouseButtonEventArgs e, bool pressed)
        {
            var flags = MakeFlags(true, false);
            flags |= pressed ? EventFlags.MouseEventPress : EventFlags.MouseEventRelease;

            switch (e.Button)
            {
                case Mouse.Button.Left:
                    flags |= EventFlags.LeftButton;
                    break;
                case Mouse.Button.Middle:
                    flags |= EventFlags.MidButton;
                    break;
                case Mouse.Button.Right:
                    flags |= EventFlags.RightButton;
                    break;
            }
            var mousePos = new Vec((int)(e.X / _fontX), (int)(e.Y / _fontY));

            var widget = WidgetAtPosition(mousePos);
            (widget as IMouseEventSuscriber)?.OnMouseEvent(mousePos, flags);
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

            Widgets.Render(_dirty);
            _dirty = false;
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