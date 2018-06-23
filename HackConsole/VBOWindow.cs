﻿using System;
using System.Diagnostics;
using System.Threading;
using SFML.Graphics;
using SFML.Window;

namespace HackConsole
{
    public class VBOWindow : BaseWindow
    {
        private readonly RenderWindow _window;
        private readonly Texture _fontTex;
        private readonly VertexArray _vertices = new VertexArray();

        public VBOWindow(string name)
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
            _window.TextEntered += OnTextEntered;
            _window.Resized += OnResized;
            _window.MouseButtonPressed += OnMouseButtonPressed;
            _window.MouseButtonReleased += OnMouseButtonReleased;
            _window.MouseMoved += OnMouseMoved;
            _window.MouseWheelMoved += OnMouseWheelMoved;

            _window.SetFramerateLimit(60);

            _fontTex = MakeSprite("ascii.png");

            OnResized(null, new SizeEventArgs(new SizeEvent {Width = _windowWidth, Height = _windowHeight }));
        }

        private static EventFlags MakeFlags(bool keys, bool mouse) {
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

        private void OnResized(object sender, SizeEventArgs e)
        {
            ResizeScreen(e.Width, e.Height);
            _window.SetView(new View(new FloatRect(0, 0, e.Width, e.Height)));
            ResizeVertices();
        }


        void ResizeVertices() {
            // Build the vertex buffer
            _vertices.PrimitiveType = PrimitiveType.Quads;
            _vertices.Resize((uint)WindowData.Data.Size.Area * 8);
        }

        private void OnKeyPressed(object sender, KeyEventArgs keyEventArgs)
        {
            var flags = MakeFlags(false, true);
            if (keyEventArgs.Alt) flags |= EventFlags.Alt;
            if (keyEventArgs.Control) flags |= EventFlags.Ctrl;
            if (keyEventArgs.Shift) flags |= EventFlags.Shift;

            var top = PopupStack.Top;
            var handler = (top != null) ? (top as IKeyEventSuscriber) : BaseKeyHandler;

            if (handler == null)
                return;

            switch (keyEventArgs.Code)
            {
                case Keyboard.Key.Up:
                    handler.OnArrowPress(new Vec(0, -1), flags);
                    break;
                case Keyboard.Key.Down:
                    handler.OnArrowPress(new Vec(0, 1), flags);
                    break;
                case Keyboard.Key.Left:
                    handler.OnArrowPress(new Vec(-1, 0), flags);
                    break;
                case Keyboard.Key.Right:
                    handler.OnArrowPress(new Vec(1, 0), flags);
                    break;
                default:
                    if (keyEventArgs.Code >= Keyboard.Key.Numpad1 && keyEventArgs.Code <= Keyboard.Key.Numpad9)
                    {
                        // Arrow key movements.
                        var id = keyEventArgs.Code - Keyboard.Key.Numpad1;
                        var move = new Vec(id % 3 - 1, 1 - id / 3);

                        handler.OnArrowPress(move, flags);
                    }
                    break;
            }
        }

        private void OnTextEntered(object sender, TextEventArgs e)
        {
            var flags = MakeFlags(true, true);
            var top = PopupStack.Top;
            var handler = (top != null) ? (top as IKeyEventSuscriber) : BaseKeyHandler;

            if (handler == null)
                return;

            handler.OnKeyPress(e.Unicode[0], flags);
        }

        private void OnMouseWheelMoved(object sender, MouseWheelEventArgs e)
        {
            var mousePos = new Vec((int)(e.X / _fontX), (int)(e.Y / _fontY));
            var delta = new Vec(0, e.Delta > 0 ? 1 : -1);
            
            var widget = WidgetAt(mousePos);
            (widget as IMouseEventSuscriber)?.OnMouseWheel(delta, MakeFlags(true, true));
        }

        private void OnMouseMoved(object sender, MouseMoveEventArgs e)
        {
            var mousePos = new Vec((int)(e.X / _fontX), (int)(e.Y / _fontY));
            var move = (mousePos - _lastMousePos) ?? Vec.Zero;
            _lastMousePos = mousePos;

            if (move == Vec.Zero)
                return;

            var widget = WidgetAt(mousePos);
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

            var widget = WidgetAt(mousePos);
            (widget as IMouseEventSuscriber)?.OnMouseEvent(mousePos, flags);
        }

        private void OnClosed(object sender, EventArgs e)
        {
            _window.Close();
        }

        public override void Run()
        {
            while (_window.IsOpen())
            {
                // Dispatch events to work with native event loop
                _window.DispatchEvents();

                OnUpdate?.Invoke();
                Render();
                Thread.Sleep(5);
            }
        }

        private void Render()
        {
            if (RenderWidgets())
            {
                _window.Clear();
                DrawGrid(_window, new RenderStates());
                _window.Display();
            }
        }

        private static Texture MakeSprite(string texName)
        {
            var image = new Image($"{texName}");
            return new Texture(image);
        }

        // Inspired by https://github.com/thebracket/rltk/blob/master/rltk/virtual_terminal.cpp
        private void DrawGrid(RenderTarget target, RenderStates states)
        {
            var spaceAscii = 219;
            var spacePos = new Vector2f((spaceAscii % 16) * _fontX, (spaceAscii / 16) * _fontY);

            var d = new Vector2f[] {
                new Vector2f(0, 0),
                new Vector2f(_fontX, 0),
                new Vector2f(_fontX, _fontY),
                new Vector2f(0, _fontY),
            };

            foreach (var v in WindowData.Data.Ids())
            {
                var idx = (uint)((v.Y * WindowData.Data.Size.X) + v.X) * 8;
                var vecScreen = new Vector2f((v.X * _fontX), (v.Y * _fontY));

                var Char = WindowData.Data[v];
                var texPos = new Vector2f((Char.Ascii % 16) * _fontX, (Char.Ascii / 16) * _fontY);

                var bgColor = ColorToSfml(Char.BackgroundColor);
                var fgColor = ColorToSfml(Char.TextColor);

                for (uint i = 0; i < 4; ++i)
                {
                    _vertices[idx + i] = new Vertex(vecScreen + d[i], bgColor, spacePos + d[i]);
                    _vertices[idx + i + 4] = new Vertex(vecScreen + d[i], fgColor, texPos + d[i]);
                }
            }
            RenderStates states2 = new RenderStates(_fontTex);
            target.Draw(_vertices, states2);
        }

        private SFML.Graphics.Color ColorToSfml(HackConsole.Color color) {
            return new SFML.Graphics.Color(color.R, color.G, color.B);
        }
    }
}