using System;
using System.Drawing;
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
        private readonly PlayerController Controller;

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
            
            Controller = new PlayerController(new Creature
                {
                    Name = "Steven",
                    Attack = new AttackComponent
                    {
                        Damage = 7,
                        HitChance = 0.75f
                    },
                    Map = _game.World,
                    SourcePos = new Point(0, 0),
                    Health = new Bar(20),
                    Hunger = new Bar(20),
                    Position = _game.World.GetEmptyLocation(),
                }
            );

            _game.AddCreature(Controller);
            
            _camera = new Camera(Controller.Self)
            {
                WindowSize = new Size((int)_window.Size.X, (int)_window.Size.Y)
            };
            _gameRenderer = new SfmlGameRenderer(_game, Controller.FieldOfView, _camera);
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
                case Keyboard.Key.Numpad1:
                    Controller.DoWalk(new Point(-1, 1));
                    break;
                case Keyboard.Key.Down:
                case Keyboard.Key.Numpad2:
                    Controller.DoWalk(new Point(0, 1));
                    break;
                case Keyboard.Key.Numpad3:
                    Controller.DoWalk(new Point(1, 1));
                    break;
                case Keyboard.Key.Left:
                case Keyboard.Key.Numpad4:
                    Controller.DoWalk(new Point(-1, 0));
                    break;
                case Keyboard.Key.Right:
                case Keyboard.Key.Numpad6:
                    Controller.DoWalk(new Point(1, 0));
                    break;
                case Keyboard.Key.Numpad7:
                    Controller.DoWalk(new Point(-1, -1));
                    break;
                case Keyboard.Key.Up:
                case Keyboard.Key.Numpad8:
                    Controller.DoWalk(new Point(0, -1));
                    break;
                case Keyboard.Key.Numpad9:
                    Controller.DoWalk(new Point(1, -1));
                    break;
                    
                case Keyboard.Key.Space:
                    Controller.Do(s => s.Mine());
                    break;
                case Keyboard.Key.I:
                    Controller.Self.Inventory.Write();
                    break;
                case Keyboard.Key.E:
                    Controller.Do(s => s.Eat());
                    break;
                case Keyboard.Key.Return:
                    {
                        foreach( var c in _game.Controllers)
                        {
                            var ai = c as AiController;
                            if (ai == null)
                                continue;
                            ai.Enemy = Controller.Self;
                        }
                    }
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
            _window.Clear();

            _gameRenderer.Draw(_window, new RenderStates());

            _window.Display();
        }
    }
}
