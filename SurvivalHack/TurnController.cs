using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;
using HackConsole;
using SurvivalHack.ECM;

namespace SurvivalHack
{
    public class TurnController : IComponent
    {
        public Entity Player;
        public List<Vec> Path;
        public FieldOfView FoV;
        public Inventory Inventory;
        public Level Level => Player.Level;
        [XmlIgnore]
        public Action OnTurnEnd;
        [XmlIgnore]
        public Action OnGameOver;
        public bool GameOver { get; private set; } = false;

        public Entity SelectedTarget = null;
        private IList<Entity> _visibleEnemies = null;
        public IList<Entity> VisibleEnemies
        {
            get
            {
                if (_visibleEnemies == null)
                {
                    _visibleEnemies = Level.GetEntities().Where(e => (
                        e != Player &&
                        e.EntityFlags.HasFlag(EEntityFlag.TeamMonster) &&
                        FoV.ShowLocation(e) != null
                    )).ToArray();
                }
                return _visibleEnemies;
            }
        }

        public TurnController(Game game) {
            Inventory = new Inventory();

            Player = new Entity(new TileGlyph(0, 23, GlyphMethod.Anim), "Player", EEntityFlag.Blocking | EEntityFlag.IsPlayer | EEntityFlag.TeamPlayer)
            {
                Components = new ComponentList
                {
                    this,
                    new Combat.MeleeWeapon(2, Combat.EAttackMove.Thrust, Combat.EDamageType.Bludgeoing),
                    new Combat.StatBlock(100, 20, 0),
                    Inventory
                },
                Attitude = new Ai.Attitude(Ai.ETeam.Player, null),
            };

            Inventory.Add(new Factory.WeaponFactory().GetBasic("ssword"));
            Inventory.Equip(Inventory.Items.Last(), 0);

            Inventory.Add(new Factory.WeaponFactory().GetBasic("sbow"));
            Inventory.Equip(Inventory.Items.Last(), 2);

            Player.OnDestroy += i =>
            {
                GameOver = true;
                OnGameOver?.Invoke();
            };

            (var level, var pos) = game.GetLevel(0);
            Player.SetLevel(level, pos);
            FoV = new FieldOfView(Player);
            Player.Add(FoV);
        }

        public void EndTurn(bool interrupt = true) {
            _visibleEnemies = null; // Triggers a recalculaton;

            // Make sure that auto turns will not be executed anymore after a manual turn.
            if (interrupt)
            {
                Path = null;
            }

            OnTurnEnd?.Invoke();
        }

        public bool TryMove(Vec move, bool interrupt = true)
        {
            if (Player.TryMove(move))
            {
                EndTurn(interrupt);
                return true;
            }
            return false;
        }

        public bool DoAutoAction()
        {
            if (Path == null || Path.Count <= 1)
                return false;

            if (InCombat())
            {
                Path = null;
                return false;
            }

            Debug.Assert(Player.Pos == Path.First());
            
            Path.RemoveAt(0);
            if (TryMove(Path.First() - Player.Pos, false))
            {
                return true;
            }
            else
            {
                Path.Clear();
                return false;
            }
        }

        public bool InCombat()
        {
            foreach (var e in Player.Level.GetEntities())
            {
                if (!e.EntityFlags.HasFlag(EEntityFlag.TeamMonster))
                    continue;

                if (e.LastSeenPos != e.Pos)
                    continue;

                return true;
            }
            return false;
        }
    }
}
