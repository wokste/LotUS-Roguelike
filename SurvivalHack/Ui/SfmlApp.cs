using System;
using HackConsole;

namespace SurvivalHack.Ui
{
    public class SfmlApp : IKeyEventSuscriber
    {
        private Game _game;
        private readonly BaseWindow _window;
        private Player _player;

        private static void Main(string[] args)
        {
            var app = new SfmlApp();
            app.Run();
        }

        public SfmlApp()
        {
            InitGame();
            _window = InitGui();

            Message.Write("You wake up in an unknown world.", _player.Position, Color.White);
        }

        private void InitGame()
        {
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
                Symbol = new Symbol((char)2, Color.White)
            };

            _game.AddCreature(_player);
        }

        private BaseWindow InitGui()
        {
            var window = new SfmlWindow("Lands of the undead sorceress");
            var consoleWidget = new MessageListWidget { Docking = Docking.Bottom, DesiredSize = new Rect { Height = 10 } };
            Message.OnMessage += (m) =>
            {
                if (_player.FoV.Visibility[m.Pos.X, m.Pos.Y] > 128)
                    consoleWidget.AddMessage(m);
            };

            window.Widgets.Add(consoleWidget);

            var infoWidget = new InfoWidget { Docking = Docking.Left, DesiredSize = new Rect { Width = 16 } };
            window.Widgets.Add(infoWidget);

            var characterWidget = new CharacterWidget(_player)
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
                        if (!_player.Alive)
                            throw new Exception("WUT. I am dead");

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
            if (!_player.Alive)
            {
                throw new Exception("WUT. I am dead");
            }

            var actPoint = _player.Position + move;
            foreach (var c in _game.World.Creatures.ToArray())
            {
                if (c.Position == actPoint && c != _player)
                {
                    _player.Attack.Attack(_player, c);
                    _game.GameTick(1);
                }
            }

            if (_player.Walk(move))
            {
                _game.GameTick((int)(1 * move.Length));
            }
        }
    }
}
