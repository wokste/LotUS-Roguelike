using HackConsole;

namespace SurvivalHack
{
    public class SfmlApp
    {
        private readonly Game _game;
        private readonly SfmlWindow _window;
        private readonly PlayerController _controller;

        // Widgets
        private readonly MessageList _consoleWidget;


        static void Main(string[] args)
        {
            var app = new SfmlApp();
            app.Run();
        }


        public SfmlApp()
        {
            _window = new SfmlWindow();
            _consoleWidget = new MessageList {Docking = Docking.Bottom, DesiredSize = new HackConsole.Rect {Height = 10}};
            _consoleWidget.AddMessage("You wake up in an unknown world.");
            _window.Widgets.Add(_consoleWidget);

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
                    Symbol = new Symbol((char) 2)
                }
            );

            _game.AddCreature(_controller);

            var worldWidget = new WorldWidget(_game.World, _controller.FieldOfView, _controller.Self);
            worldWidget.Docking = Docking.Fill;

            _window.Widgets.Add(worldWidget);
        }

        public void Run() { 
            _window.Run();
        }
        
        /*
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
        */
    }
}
