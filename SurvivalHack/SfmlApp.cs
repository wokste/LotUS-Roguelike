using System;
using System.Drawing;
using HackLib;
using SFML.Graphics;
using SFML.Window;

namespace SurvivalHack
{
    class SfmlApp
    {
        private readonly Game _game;
        private readonly Camera _camera;
        private readonly SfmlGameRenderer _gameRenderer;
        private readonly RenderWindow _window;

        static void Main(string[] args)
        {
            var app = new SfmlApp();
            app.Run();
        }

        public SfmlApp()
        {
            var contextSettings = new ContextSettings
            {
                DepthBits = 24
            };
            
            _window = new RenderWindow(new VideoMode(640, 480), "SFML SurvivalHack - How much ore can you collect?", Styles.Default, contextSettings);
            _window.SetActive();

            _window.SetVisible(true);
            _window.SetVerticalSyncEnabled(true);

            _window.Closed += OnClosed;
            _window.KeyPressed += OnKeyPressed;
            _window.Resized += OnResized;

            _window.SetFramerateLimit(60);
            
            _game = new Game();
            _game.Init();

            _camera = new Camera(_game.Player);
            _camera.WindowSize = new Size(640,480);
            _gameRenderer = new SfmlGameRenderer(_game, _camera);
        }

        private void OnResized(object sender, SizeEventArgs e)
        {
            _camera.WindowSize = new Size((int)e.Width, (int)e.Height);
            _window.SetView(new View(new FloatRect(0, 0, e.Width, e.Height)));
        }

        private void OnKeyPressed(object sender, KeyEventArgs keyEventArgs)
        {
            //TODO: Some of this needs to move to game
            switch (keyEventArgs.Code)
            {
                case Keyboard.Key.W:
                    _game.PlayerWalk(new Point(0, -1));
                    break;
                case Keyboard.Key.A:
                    _game.PlayerWalk(new Point(-1, 0));
                    break;
                case Keyboard.Key.S:
                    _game.PlayerWalk(new Point(0, 1));
                    break;
                case Keyboard.Key.D:
                    _game.PlayerWalk(new Point(1, 0));
                    break;
                case Keyboard.Key.Space:
                    _game.PlayerMine();
                    break;
                case Keyboard.Key.I:
                    _game.Player.Inventory.Write();
                    break;
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

                Update();
                Render();
            }
        }

        private void Update()
        {
            _camera.Update();
        }

        private void Render()
        {
            _window.Clear();

            _gameRenderer.Draw(_window, new RenderStates());

            _window.Display();
        }
    }
}
