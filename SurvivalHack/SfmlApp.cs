using System;
using HackConsole;
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
            var app = new SfmlWindow();
            var msgList = new MessageList {Docking = Docking.Bottom, DesiredSize = new CRect {Height = 10}};

            app.Widgets.Add(msgList);

            msgList.AddMessage("You obtained a small flaming rock and put it, despite your best common sense, in your (very flamable) inventory.");
            msgList.AddMessage("You are now on fire with flames coming out of your backpack.");
            msgList.AddMessage("You die.");

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
                        Damage = 4,
                        HitChance = 75f
                    },
                    Map = _game.World,
                    Health = new Bar(20),
                    Hunger = new Bar(20),
                    Position = _game.World.GetEmptyLocation(),
                    Symbol = new Symbol((char)2)
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
                    _controller.PlanWalk(new Vec(-1, 1));
                    break;
                case Keyboard.Key.Down:
                case Keyboard.Key.Numpad2:
                    _controller.PlanWalk(new Vec(0, 1));
                    break;
                case Keyboard.Key.Numpad3:
                    _controller.PlanWalk(new Vec(1, 1));
                    break;
                case Keyboard.Key.Left:
                case Keyboard.Key.Numpad4:
                    _controller.PlanWalk(new Vec(-1, 0));
                    break;
                case Keyboard.Key.Right:
                case Keyboard.Key.Numpad6:
                    _controller.PlanWalk(new Vec(1, 0));
                    break;
                case Keyboard.Key.Numpad7:
                    _controller.PlanWalk(new Vec(-1, -1));
                    break;
                case Keyboard.Key.Up:
                case Keyboard.Key.Numpad8:
                    _controller.PlanWalk(new Vec(0, -1));
                    break;
                case Keyboard.Key.Numpad9:
                    _controller.PlanWalk(new Vec(1, -1));
                    break;
                case Keyboard.Key.Space:
                    _controller.Plan(s => s.Mine() ? 1 : -1);
                    break;
                case Keyboard.Key.I:
                    _controller.Self.Inventory.Write();
                    break;
                case Keyboard.Key.E:
                    _controller.Plan(s => s.Eat() ? 1 : -1);
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
