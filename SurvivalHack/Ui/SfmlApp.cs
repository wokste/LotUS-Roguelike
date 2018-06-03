﻿using HackConsole;
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
                Components = new List<ECM.IComponent>()
                {
                    new Combat.MeleeWeapon(2, Combat.EDamageType.Bludgeoing),
                    new Combat.Damagable(100)
                },
                Attitude = new Ai.Attitude(Ai.ETeam.Player, null),
                Flags = TerrainFlag.Walk,
            };
            var inventory = new Inventory();

            inventory.Add(new Factory.WeaponFactory().GetBasic("ssword"));
            inventory.Equip(_player, inventory._items[0], 0);

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
                        var o = new OptionWidget($"Drink", _player.GetOne<Inventory>()._items, i => {
                            if (_player.Event(i, _player, ECM.EUseMessage.Drink))
                                NextTurn();
                        });
                        _window.PopupStack.Push(o);
                    }
                    break;
                case 'w':
                    {
                        var o = new InventoryWidget(_player, _window);
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
                case 'f':
                    {
                        // TODO: Fight, especially useful for ranged attacks.
                    }
                    break;

#if WIZTOOLS
                case '\\': // Well 'w' will be used for wield/wear
                    {
                        var o = new OptionWidget($"Wizard tools", WizTools.Tools._items, i => {
                            _player.Event(i, _player, ECM.EUseMessage.Cast);
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
            var actPoint = _player.Move.Pos + move;

            foreach (var enemy in _game.Level.GetEntity(actPoint))
            {
                if (enemy.EntityFlags.HasFlag(EEntityFlag.TeamMonster))
                {
                    (var weapon, var weaponComponent) = _player.GetWeapon(enemy);

                    if (weaponComponent != null)
                    {
                        weaponComponent.Attack(_player, weapon, enemy);
                        NextTurn();
                    }
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
