using HackConsole;
using SurvivalHack.ECM;
using System.Linq;

namespace SurvivalHack.Ui
{
    public class SfmlApp : IKeyEventSuscriber
    {
        private Game _game;
        private readonly BaseWindow _window;
        private TurnController _controller;

        private static void Main(string[] args)
        {
            var app = new SfmlApp();
            app.Run();
        }

        public SfmlApp()
        {
            InitGame();
            _window = InitGui();

            ColoredString.Write("You wake up in an unknown world.", Color.White);
        }

        private void InitGame()
        {
            _game = new Game();
            _game.Init();

            _controller = new TurnController(_game);
            _controller.OnTurnEnd += () => {
                _game.MonsterTurn();
                WindowData.ForceUpdate = true;
            };
            _controller.OnGameOver += () => {
                var o = new GameOverWidget
                {
                    DesiredSize = new Rect(new Vec(), new Vec(25, 25)),
                };
                _window.PopupStack.Push(o);
            };
        }

        private BaseWindow InitGui()
        {
            var window = new VBOWindow("Lands of the undead sorceress");
            var consoleWidget = new MessageListWidget { Docking = Docking.Bottom, DesiredSize = new Rect { Height = 10 } };
            ColoredString.OnMessage += (m) =>
            {
                consoleWidget.Add(m);
            };

            window.Widgets.Add(consoleWidget);

            var infoWidget = new InfoWidget { Docking = Docking.Left, DesiredSize = new Rect { Width = 16 } };
            window.Widgets.Add(infoWidget);

            var characterWidget = new EntityDetailWidget(_controller)
            {
                DesiredSize = { Width = 16 },
                Docking = Docking.Right
            };
            window.Widgets.Add(characterWidget);

            var worldWidget = new MapWidget(_game.Level, _controller)
            {
                Docking = Docking.Fill
            };
            window.Widgets.Add(worldWidget);

            window.BaseKeyHandler = this;
            worldWidget.OnSelected += c => { infoWidget.Item = c; };

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

            _controller.DoAutoAction();
        }

        public void OnKeyPress(char keyCode, EventFlags flags)
        {
            switch (keyCode)
            {
                case 'a':
                    {
                        // TODO: Reserved for interactin with the map. Open doors, pray at altars, etc.
                    }
                    break;
                case 'c':
                    {
                        // TODO: Reserved for casting spells
                    }
                    break;
                case 'd':
                    {
                        /*var l = _controller.Player.GetOne<Inventory>().Items;
                        var o = new OptionWidget($"Drop Item", l, i => {
                            //TODO: drop item
                        });
                        _window.PopupStack.Push(o);*/
                    }
                    break;
                case 'e':
                    {
                        var l = _controller.Player.GetOne<Inventory>().Items.Where(i => i.EntityFlags.HasFlag(EEntityFlag.Consumable)).ToList();
                        var o = new OptionWidget($"Consume", l, i => {
                            if (Eventing.On(new ConsumeEvent(_controller.Player, i)))
                                _controller.EndTurn();
                        });
                        _window.PopupStack.Push(o);
                    }
                    break;
                case 'f':
                    {
                        // TODO: Fight, especially useful for ranged attacks.
                    }
                    break;
                case 'g':
                    {
                        var pos = _controller.Player.Move.Pos;
                        bool didTurn = false;
                        foreach (var i in _game.Level.GetEntities(new Rect(pos, Vec.One)))
                        {
                            if (i.EntityFlags.HasFlag(EEntityFlag.Pickable))
                            {
                                i.Move.Unbind(i);
                                _controller.Player.GetOne<Inventory>().Add(i);
                            }
                        }

                        if (didTurn)
                            _controller.EndTurn();
                    }
                    break;
                case 's':
                    {
                        // TODO: Reserved for searching
                    }
                    break;
                case 't':
                    {
                        var l = _controller.Player.GetOne<Inventory>().Items.Where(i => i.EntityFlags.HasFlag(EEntityFlag.Throwable)).ToList();
                        /* TODO: Reserved for throwing
                        var o = new OptionWidget($"Throw", l, i => {
                            
                        });
                        _window.PopupStack.Push(o);
                        */
                    }
                    break;
                case 'w':
                    {
                        var o = new InventoryWidget(_controller, _window);
                        _window.PopupStack.Push(o);
                    }
                    break;
                case 'z':
                    {
                        // TODO: Reserved for zapping wands
                    }
                    break;

#if WIZTOOLS
                case '\\': // Well 'w' will be used for wield/wear
                    {
                        var o = new OptionWidget($"Wizard tools", WizTools.Tools.Items, i => {
                            Eventing.On(new CastEvent(_controller.Player, i));
                        });
                        _window.PopupStack.Push(o);
                    }
                    break;
#endif
                default:
                    break;
            }
        }

        public void OnArrowPress(Vec move, EventFlags flags)
        {
            var actPoint = _controller.Player.Move.Pos + move;

            foreach (var enemy in _game.Level.GetEntity(actPoint))
            {
                if (enemy.EntityFlags.HasFlag(EEntityFlag.TeamMonster))
                {
                    (var weapon, var comp) = _controller.Player.GetWeapon(enemy);

                    if (weapon != null)
                    {
                        Eventing.On(new AttackEvent(_controller.Player, weapon, enemy, comp.AttackMove));
                        _controller.EndTurn();
                    }
                    return;
                }
            }

            _controller.Move(move);
        }
    }
}
