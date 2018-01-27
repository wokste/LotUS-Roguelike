using HackConsole;

namespace SurvivalHack.Ui
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
            _window = new SfmlWindow("Lands of the undead sorceress");
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
                    Symbol = new Symbol((char) 2, Color.White)
                }
            );

            _game.AddCreature(_controller);

            var characterWidget = new CharacterWidget(_controller.Self)
            {
                DesiredSize = {Width = 16},
                Docking = Docking.Right
            };
            _window.Widgets.Add(characterWidget);

            var worldWidget = new WorldWidget(_game.World, _controller.FieldOfView, _controller.Self)
            {
                Docking = Docking.Fill
            };
            _window.Widgets.Add(worldWidget);
        }

        public void Run() { 
            _window.Run();
        }
    }
}
