using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        public Level Level => Player.Move.Level;

        public string Describe() => null;
        public Action OnTurnEnd;
        public Action OnGameOver;

        public TurnController(Game game) {
            Inventory = new Inventory();

            Player = new Entity((char)2, "Player", EEntityFlag.Blocking | EEntityFlag.IsPlayer | EEntityFlag.TeamPlayer)
            {
                Components = new List<IComponent>()
                {
                    this,
                    new Combat.MeleeWeapon(2, Combat.EAttackMove.Thrust, Combat.EDamageType.Bludgeoing),
                    new Combat.Damagable(100),
                    Inventory
                },
                Attitude = new Ai.Attitude(Ai.ETeam.Player, null),
                Flags = TerrainFlag.Walk,
            };

            Inventory.Add(new Factory.WeaponFactory().GetBasic("ssword"));
            Inventory.Equip(Player, Inventory.Items[0], 0);

            Player.OnDestroy += i =>
            {
                Player = null;
                OnGameOver?.Invoke();
            };

            var pos = game.Levels[0].GetEmptyLocation();
            MoveComponent.Bind(Player, game.Levels[0], pos);
            FoV = new FieldOfView(Player.Move);
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
            if (Player.Move.Move(Player, move))
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

            Debug.Assert(Player.Move.Pos == Path.First());
            
            Path.RemoveAt(0);
            if (Move(Path.First() - Player.Move.Pos, false))
            {
                return true;
            }
            else
            {
                Path.Clear();
                return false;
            }
        }

        public void GetActions(Entity self, BaseEvent msg, EUseSource source)
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
