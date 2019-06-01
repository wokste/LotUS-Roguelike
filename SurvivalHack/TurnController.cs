using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HackConsole;
using SurvivalHack.ECM;
using SurvivalHack.Ui.Tools;

namespace SurvivalHack
{
    public class TurnController : Component
    {
        public Entity Player;
        public List<Vec> Path;
        public FieldOfView FoV;
        public Inventory Inventory;
        public Level Level => Player.Level;

        public ITool ActiveTool; // TODO: Event on tool change

        public Action OnTurnEnd;
        public Action OnGameOver;
        public bool GameOver { get; private set; } = false;

        public TurnController(Game game) {
            Inventory = new Inventory();

            Player = new Entity(new TileGlyph(0, 23, TileGlyph.ANIM), "Player", EEntityFlag.Blocking | EEntityFlag.IsPlayer | EEntityFlag.TeamPlayer)
            {
                Components = new List<IComponent>()
                {
                    this,
                    new Combat.MeleeWeapon(2, Combat.EAttackMove.Thrust, Combat.EDamageType.Bludgeoing),
                    new Combat.Damagable(100, 20, 0),
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
            // Make sure that auto turns will not be executed anymore after a manual turn.
            if (interrupt)
                Interrupt();

            OnTurnEnd?.Invoke();
        }

        public bool Move(Vec move, bool interrupt = true)
        {
            if (Player.Move(move))
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

            Debug.Assert(Player.Pos == Path.First());
            
            Path.RemoveAt(0);
            if (Move(Path.First() - Player.Pos, false))
            {
                return true;
            }
            else
            {
                Path.Clear();
                return false;
            }
        }

        public override void GetActions(Entity self, BaseEvent msg, EUseSource source)
        {
            if (source == EUseSource.Target && (msg is AttackEvent || msg is ThreatenEvent))
                msg.OnEvent += (m) => Interrupt();
        }

        private void Interrupt()
        {
            Path = null;
        }
    }
}
