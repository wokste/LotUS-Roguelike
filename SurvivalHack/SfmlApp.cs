using System;
using SFML.Graphics;
using SFML.Window;
using HackLib;

namespace SurvivalHack
{
    public class SfmlApp
    {
        private readonly Game _game;
        private readonly Camera _camera;
        private readonly SfmlGameRenderer _gameRenderer;
        private readonly RenderWindow _window;
        private readonly PlayerController _controller;

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

            _window = new RenderWindow(new VideoMode(800, 600), "SFML SurvivalHack - How much ore can you collect before you starve?", Styles.Default, contextSettings);
            _window.SetActive();

            _window.SetVisible(true);
            _window.SetVerticalSyncEnabled(true);

            _window.Closed += OnClosed;
            _window.KeyPressed += OnKeyPressed;
            _window.Resized += OnResized;

            _window.SetFramerateLimit(60);
            
            _game = new Game();
            _game.Init();
            
            _controller = new PlayerController(new Creature
                {
                    Name = "Steven",
                    Attack = new AttackComponent
                    {
                        Damage = 7,
                        HitChance = 75f
                    },
                    Map = _game.World,
                    SourcePos = new Vec(0, 0),
                    Health = new Bar(20),
                    Hunger = new Bar(20),
                    Position = _game.World.GetEmptyLocation(),
                }
            );

            _game.AddCreature(_controller);
            
            _camera = new Camera(_controller.Self)
            {
                WindowSize = new Vec((int)_window.Size.X, (int)_window.Size.Y)
            };
            _gameRenderer = new SfmlGameRenderer(_game, _controller.FieldOfView, _camera);
        }

        private void OnResized(object sender, SizeEventArgs e)
        {
            _camera.WindowSize = new Vec((int)e.Width, (int)e.Height);
            _window.SetView(new View(new FloatRect(0, 0, e.Width, e.Height)));
        }

        private void OnKeyPressed(object sender, KeyEventArgs keyEventArgs)
        {
            //TODO: Some of this needs to move to game
            switch (keyEventArgs.Code)
            {
                case Keyboard.Key.Numpad1:
                    _controller.DoWalk(new Vec(-1, 1));
                    break;
                case Keyboard.Key.Down:
                case Keyboard.Key.Numpad2:
                    _controller.DoWalk(new Vec(0, 1));
                    break;
                case Keyboard.Key.Numpad3:
                    _controller.DoWalk(new Vec(1, 1));
                    break;
                case Keyboard.Key.Left:
                case Keyboard.Key.Numpad4:
                    _controller.DoWalk(new Vec(-1, 0));
                    break;
                case Keyboard.Key.Right:
                case Keyboard.Key.Numpad6:
                    _controller.DoWalk(new Vec(1, 0));
                    break;
                case Keyboard.Key.Numpad7:
                    _controller.DoWalk(new Vec(-1, -1));
                    break;
                case Keyboard.Key.Up:
                case Keyboard.Key.Numpad8:
                    _controller.DoWalk(new Vec(0, -1));
                    break;
                case Keyboard.Key.Numpad9:
                    _controller.DoWalk(new Vec(1, -1));
                    break;
                case Keyboard.Key.Space:
                    _controller.Do(s => s.Mine() ? 1 : -1);
                    break;
                case Keyboard.Key.I:
                    _controller.Self.Inventory.Write();
                    break;
                case Keyboard.Key.E:
                    _controller.Do(s => s.Eat() ? 1 : -1);
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
            _game.Update();
            _camera.Update();
        }

        private void Render()
        {
            _window.SetTitle($"{_game.Time.Time}");
            _window.Clear();

            _gameRenderer.Draw(_window, new RenderStates());

            _window.Display();
        }
    }
}
