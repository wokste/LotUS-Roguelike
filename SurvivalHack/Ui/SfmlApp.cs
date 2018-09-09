using HackConsole;
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

            ColoredString.Write("You wake up in an unknown world.", Colour.White);
        }

        private void InitGame()
        {
            _game = new Game();
            _game.Init();

            _controller = new TurnController(_game);
            _controller.OnTurnEnd += () => {
                _game.MonsterTurn();
            };
            _controller.OnGameOver += () => {
                var o = new GameOverWidget
                {
                    DesiredSize = new Rect(new Vec(), new Size(250, 250)),
                };
                _window.PopupStack.Push(o);
            };
        }

        private BaseWindow InitGui()
        {
            var window = new SFMLWindow("Lands of the undead sorceress");
            var consoleWidget = new MessageListWidget { Docking = Docking.Bottom, DesiredSize = new Rect { Height = 160 } };
            ColoredString.OnMessage += (m) =>
            {
                consoleWidget.Add(m);
            };

            window.Widgets.Add(consoleWidget);

            var infoWidget = new InfoWidget { Docking = Docking.Left, DesiredSize = new Rect { Width = 256 } };
            window.Widgets.Add(infoWidget);

            var characterWidget = new HudWidget(_controller)
            {
                DesiredSize = { Width = 256 },
                Docking = Docking.Right
            };
            window.Widgets.Add(characterWidget);

            var worldWidget = new MapWidget(_controller)
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
            bool didTurn = false;

            switch (keyCode)
            {
                case 'a': // Todo: Map actions
                    {
                        // TODO: Reserved for interactin with the map. Open doors, pray at altars, etc.
                    }
                    break;
                case 'c':
                    {
                        // TODO: Reserved for casting spells
                    }
                    break;
                case 'd': // Drop Item
                    {
                        var inv = _controller.Player.GetOne<Inventory>();
                        var o = new OptionWidget($"Drop Item", inv.Items, i => {
                            if (inv.Remove(i))
                            {
                                i.SetLevel(_controller.Player.Level, _controller.Player.Pos);
                                _controller.EndTurn();
                            }
                        });
                        _window.PopupStack.Push(o);
                    }
                    break;
                case 'e': // Use item
                    {
                        var l = _controller.Player.GetOne<Inventory>().Items.Where(i => i.EntityFlags.HasFlag(EEntityFlag.Consumable)).ToList();
                        var o = new OptionWidget($"Consume", l, i => {
                            if (Eventing.On(new ConsumeEvent(_controller.Player, i)))
                                _controller.EndTurn();
                        });
                        _window.PopupStack.Push(o);
                    }
                    break;
                case 'f': // Fire ranged weapon
                    {
                        // TODO: Fire, especially useful for ranged attacks.
                        _controller.ActiveTool = new Tools.RangedWeaponTool(_controller);
                    }
                    break;
                case 'g':
                    {
                        var pos = _controller.Player.Pos;
                        foreach (var i in _controller.Level.GetEntities(pos).ToArray())
                        {
                            if (i.EntityFlags.HasFlag(EEntityFlag.Pickable))
                            {
                                i.SetLevel(null, Vec.Zero);
                                _controller.Player.GetOne<Inventory>().Add(i);
                                didTurn = true;
                            }
                        }
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
                case '>':
                    {
                        foreach (var entity in _controller.Level.GetEntities(_controller.Player.Pos))
                        {
                            if (_controller.Player == entity)
                                continue;

                            if (Eventing.On(new DownEvent(_controller.Player, entity)))
                            {
                                didTurn = true;
                                break;
                            }
                        }
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


            if (didTurn)
                _controller.EndTurn();
        }

        public void OnArrowPress(Vec move, EventFlags flags)
        {
            var actPoint = _controller.Player.Pos + move;

            foreach (var enemy in _controller.Level.GetEntities(actPoint))
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
