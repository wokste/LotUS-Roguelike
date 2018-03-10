using HackConsole;

namespace SurvivalHack.Ui
{
    public class SfmlApp
    {
        private readonly Game _game;
        private readonly SfmlWindow _window;
        private readonly Player _player;

        // Widgets
        private readonly MessageListWidget _consoleWidget;
        private readonly InfoWidget _infoWidget;
        
        static void Main(string[] args)
        {
            var app = new SfmlApp();
            app.Run();
        }
        
        public SfmlApp()
        {
            _window = new SfmlWindow("Lands of the undead sorceress");
            _consoleWidget = new MessageListWidget {Docking = Docking.Bottom, DesiredSize = new HackConsole.Rect {Height = 10}};
            _consoleWidget.AddMessage("You wake up in an unknown world.");
            _window.Widgets.Add(_consoleWidget);

            _game = new Game();
            _game.Init();

            _player = new Player(_game.World, _game.World.GetEmptyLocation())
            {
                Name = "Player",
                Description = "You, as a player",
                Attack = new AttackComponent
                {
                    Damage = 20,
                    HitChance = 75f
                },
                Health = new Bar(100),
                Hunger = new Bar(100),
                Symbol = new Symbol((char) 2, Color.White)
            };

            _game.AddCreature(_player);

            _infoWidget = new InfoWidget { Docking = Docking.Left, DesiredSize = new HackConsole.Rect { Width = 16 }};
            _window.Widgets.Add(_infoWidget);

            var characterWidget = new CharacterWidget(_player)
            {
                DesiredSize = {Width = 16},
                Docking = Docking.Right
            };
            _window.Widgets.Add(characterWidget);

            var worldWidget = new WorldWidget(_game.World, _player.FoV, _player)
            {
                Docking = Docking.Fill
            };
            _window.Widgets.Add(worldWidget);

            _window.Focus = worldWidget;
            worldWidget.OnSelected += (IDescriptionProvider c) => { _infoWidget.Item = c; };
        }

        public void Run() {
            _window.OnUpdate = Update;
            _window.Run();
        }

        private void Update() {
            _game.Update(5);
        }
    }
}
