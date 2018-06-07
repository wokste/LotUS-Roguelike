using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public void EndTurn() {
            OnTurnEnd?.Invoke();
        }

        public void PlayerMoved()
        {
            OnMove?.Invoke();
        }

        public void Move(Vec move)
        {
            if (Player.Move.Move(Player, move))
            {
                OnTurnEnd?.Invoke();
                OnMove?.Invoke();
            }
        }
    }
}
