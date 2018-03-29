using HackConsole;

namespace SurvivalHack.Ui
{
    public class SfmlApp : IKeyEventSuscriber
    {
        private Game _game;
        private readonly BaseWindow _window;
        private ECM.Player _player;

        private static void Main(string[] args)
        {
            var app = new SfmlApp();
            app.Run();
        }

        public SfmlApp()
        {
            InitGame();
            _window = InitGui();

            Message.Write("You wake up in an unknown world.", _player.Move.Pos, Color.White);
        }

        private void InitGame()
        {
            _game = new Game();
            _game.Init();

            _player = new ECM.Player(_game.World.GetEmptyLocation())
            {
                Name = "Player",
                Description = "You, as a player",
                Attack = new ECM.AttackComponent
                {
                    Damage = 20,
                    HitChance = 75f
                },
                Health = new Bar(100),
                Hunger = new Bar(100),
                Symbol = new Symbol((char)2, Color.White)
            };
            _player.Inventory.Add(ItemTypeList.Get("mushroom").Make(2,2));
            _player.Inventory.Add(ItemTypeList.Get("pumpkin").Make(1));
            _player.Inventory.Add(ItemTypeList.Get("sword1").Make(1));

            _player.OnDestroy += PlayerDied;

            //_game.AddCreature(_player);
            _player.Move.AddToMap(_game.World, _player);
        }

        private BaseWindow InitGui()
        {
            var window = new SfmlWindow("Lands of the undead sorceress");
            var consoleWidget = new MessageListWidget { Docking = Docking.Bottom, DesiredSize = new Rect { Height = 10 } };
            Message.OnMessage += (m) =>
            {
                if (_player == null || m.Pos == Vec.NaV || _player.FoV.Visibility[m.Pos.X, m.Pos.Y] > 128)
                {
                    consoleWidget.AddMessage(m);
                }
            };

            window.Widgets.Add(consoleWidget);

            var infoWidget = new InfoWidget { Docking = Docking.Left, DesiredSize = new Rect { Width = 16 } };
            window.Widgets.Add(infoWidget);

            var characterWidget = new EntityDetailWidget(_player)
            {
                DesiredSize = { Width = 16 },
                Docking = Docking.Right
            };
            window.Widgets.Add(characterWidget);

            var worldWidget = new WorldWidget(_game.World, _player.FoV, _player)
            {
                Docking = Docking.Fill
            };
            window.Widgets.Add(worldWidget);

            window.BaseKeyHandler = this;
            worldWidget.OnSelected += c => { infoWidget.Item = c; };
            worldWidget.OnSpendTime += _game.GameTick;

            return window;
        }

        private void PlayerDied(ECM.Entity obj)
        {
            var o = new GameOverWidget
            {
                DesiredSize = new Rect(new Vec(), new Vec(25, 25)),
            };
            _window.PopupStack.Push(o);
            _player = null;
        }

        public void Run()
        {
            _window.OnUpdate = Update;
            _window.Run();
        }

        private void Update()
        {
            _game.Update(5);
        }

        public void OnKeyPress(char keyCode, EventFlags flags)
        {
            switch (keyCode)
            {
                case 'e':
                    {
                        var o = new OptionWidget<Item>
                        {
                            DesiredSize = new Rect(new Vec(), new Vec(25,25) ),
                            OnSelect = i =>
                            {
                                if (_player.Eat(i))
                                    _game.GameTick(1);
                            },
                            Question = "Choose food",
                            Set = _player.Inventory._items
                        };
                        _window.PopupStack.Push(o);
                    }
                    break;
            }
        }

        public void OnArrowPress(Vec move, EventFlags flags)
        {
            var actPoint = _player.Move.Pos + move;

            foreach (var e in _game.World.GetEntity(actPoint))
            {
                if (e != _player)
                {
                    _player.Attack.Attack(_player, e);
                    _game.GameTick(1);
                    return;
                }
            }

            if (_player.Move.Walk(_player, move))
            {
                _game.GameTick((int)(1 * move.Length));
            }
        }
    }
}
