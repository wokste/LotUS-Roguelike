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

        public string Describe() => null;
        public Action OnTurnEnd;
        public Action OnMove;
        public Action OnGameOver;
        
        public IEnumerable<UseFunc> GetActions(EUseMessage filter, EUseSource source) => Enumerable.Empty<UseFunc>();

        public TurnController(Game game) {
            var pos = game.Level.GetEmptyLocation();
            Inventory = new Inventory();

            Player = new Entity((char)2, "Player", EEntityFlag.Blocking | EEntityFlag.IsPlayer | EEntityFlag.TeamPlayer)
            {
                Components = new List<IComponent>()
                {
                    this,
                    new Combat.MeleeWeapon(2, Combat.EDamageType.Bludgeoing),
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

            MoveComponent.Bind(Player, game.Level, pos);
            FoV = new FieldOfView(Player.Move);
            Player.Add(FoV);
        }

        public void EndTurn(bool interrupt = true) {
            // Make sure that auto turns will not be executed anymore after a manual turn.
            if (interrupt)
                Path = null;

            OnTurnEnd?.Invoke();
        }

        public void Move(Vec move, bool interrupt = true)
        {
            if (Player.Move.Move(Player, move))
            {
                OnMove?.Invoke();
                EndTurn(interrupt);
            }
        }

        public bool DoAutoAction()
        {
            if (Path == null || Path.Count <= 1)
                return false;

            Debug.Assert(Player.Move.Pos == Path.First());
            
            Path.RemoveAt(0);
            Move(Path.First() - Player.Move.Pos, false);

            return true;
        }
    }
}
