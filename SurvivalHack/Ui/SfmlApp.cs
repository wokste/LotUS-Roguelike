using HackConsole;
using System;
using System.Collections.Generic;

namespace SurvivalHack.Ui
{
    public class SfmlApp : IKeyEventSuscriber
    {
        private Game _game;
        private readonly BaseWindow _window;
        private Entity _player;

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

            _player = new Entity((char)2, "Player", EEntityFlag.Blocking | EEntityFlag.IsPlayer | EEntityFlag.TeamPlayer)
            {
                Description = "You, as a player",

                Components = new List<ECM.IComponent>()
                {
                    new ECM.AttackComponent(2, EDamageType.Bludgeoing),
                },
                Attitude = new Ai.Attitude(Ai.ETeam.Player, null),
                Flags = TerrainFlag.Walk,
                Health = new Bar(100),
                Hunger = new Bar(100),
            };
            var inventory = new Inventory();

            inventory.Add(new Factory.WeaponFactory().GetBasic("ssword"));

            _player.Add(inventory);
            _player.OnDestroy += PlayerDied;

            var pos = _game.Level.GetEmptyLocation();
            ECM.MoveComponent.Bind(_player, _game.Level, pos);
            _player.Add(new FieldOfView(_player.Move));
        }

        private BaseWindow InitGui()
        {
            var window = new VBOWindow("Lands of the undead sorceress");
            var consoleWidget = new MessageListWidget { Docking = Docking.Bottom, DesiredSize = new Rect { Height = 10 } };
            Message.OnMessage += (m) =>
            {
                var pos2 = m.Pos ?? Vec.Zero;
                if (_player == null || m.Pos == null || _player.GetOne<FieldOfView>().Is(pos2, FieldOfView.FLAG_VISIBLE))
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

            var worldWidget = new MapWidget(_game.Level, _player.GetOne<FieldOfView>(), _player)
            {
                Docking = Docking.Fill
            };
            window.Widgets.Add(worldWidget);

            window.BaseKeyHandler = this;
            worldWidget.OnSelected += c => { infoWidget.Item = c; };

            return window;
        }

        private void PlayerDied(Entity obj)
        {
            var o = new GameOverWidget
            {
                DesiredSize = new Rect(new Vec(), new Vec(25, 25)),
            };
            _window.PopupStack.Push(o);
            _player = null;
        }

        private void NextTurn()
        {
            _game.MonsterTurn();

            // TODO: Update only part of the UI
            WindowData.ForceUpdate = true;
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
                case 'd':
                    {
                        var o = new OptionWidget<Entity>
                        {
                            DesiredSize = new Rect(new Vec(), new Vec(25,25) ),
                            OnSelect = i =>
                            {
                                if (_player.UseItem(i, ECM.EUseMessage.Drink))
                                    NextTurn();
                            },
                            Question = "Drink item",
                            Set = _player.GetOne<Inventory>()._items
                        };
                        _window.PopupStack.Push(o);
                    }
                    break;
                case 'r':
                    {
                        var o = new OptionWidget<Entity>
                        {
                            DesiredSize = new Rect(new Vec(), new Vec(25, 25)),
                            OnSelect = i =>
                            {
                                if (_player.UseItem(i, ECM.EUseMessage.Read))
                                    NextTurn();
                            },
                            Question = "Read item",
                            Set = _player.GetOne<Inventory>()._items
                        };
                        _window.PopupStack.Push(o);
                    }
                    break;
                case 'w':
                    {
                        var o = new OptionWidget<Entity>
                        {
                            DesiredSize = new Rect(new Vec(), new Vec(25, 25)),
                            OnSelect = i =>
                            {
                                if (_player.GetOne<Inventory>().Equip(_player,i,0))
                                    NextTurn();
                            },
                            Question = "Wield item",
                            Set = _player.GetOne<Inventory>()._items
                        };
                        _window.PopupStack.Push(o);
                    }
                    break;
                case 'g':
                    {
                        var pos = _player.Move.Pos;
                        bool didTurn = false;
                        foreach (var i in _game.Level.GetEntities(new Rect(pos, Vec.One)))
                        {
                            if (i.EntityFlags.HasFlag(EEntityFlag.Pickable))
                            {
                                i.Move.Unbind(i);
                                _player.GetOne<Inventory>().Add(i);
                            }
                        }

                        if (didTurn)
                            NextTurn();
                    }
                    break;


#if WIZTOOLS
                case '\\': // Well 'w' will be used for wield/wear and I have 
                    {
                        var ls = new List<Action>();
                        ls.Add(() => {
                            _player.GetOne<FieldOfView>()?.ShowAll(FieldOfView.SET_ALWAYSVISIBLE);
                        });

                        ls.Add(() => {
                            foreach (var e in _game.Level.GetEntities(new Rect(Vec.Zero, _game.Level.Size)))
                            {
                                if (e.Ai != null)
                                    e.TakeDamage(9001, EDamageType.Piercing);
                            }
                        });

                        var o = new OptionWidget<Action>
                        {
                            DesiredSize = new Rect(new Vec(), new Vec(25, 25)),
                            OnSelect = i =>
                            {
                                i();
                            },
                            Question = "Wizard tools",
                            Set = ls
                        };
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
            var actPoint = _player.Move.Pos + move;

            foreach (var e in _game.Level.GetEntity(actPoint))
            {
                if (e.EntityFlags.HasFlag(EEntityFlag.TeamMonster))
                {
                    var weapon = _player; // TODO: Look in equipment for a weapon, before trying punches ets.

                    _player.Attack(e, weapon);

                    NextTurn();
                    return;
                }
            }

            if (_player.Move.Move(_player, move))
            {
                NextTurn();
            }
        }
    }
}
