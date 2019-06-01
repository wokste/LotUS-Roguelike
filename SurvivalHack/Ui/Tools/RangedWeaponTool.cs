using HackConsole;
using System.Linq;

namespace SurvivalHack.Ui.Tools
{
    class RangedWeaponTool : ITool
    {
        private readonly TurnController _turnController;

        public RangedWeaponTool(TurnController controller) {
            _turnController = controller;
        }

        public void Apply(Vec dest)
        {
            var map = _turnController.Level;
            var player = _turnController.Player;
            var shape = GetShape(dest);
            var item = player.GetOne<Inventory>()?.Slots[Inventory.SLOT_RANGED].Item;
            var weapon = item?.GetOne<Combat.IWeapon>();
            if (weapon != null) {
                bool effective = false;

                foreach (var enemy in map.GetEntities(shape).ToArray())
                {
                    Eventing.On(new AttackEvent(player, item, enemy, weapon.AttackMove));
                    effective = true;
                }

                if (effective)
                    _turnController.EndTurn();
            }
        }

        public IShape GetShape(Vec dest)
        {
            return dest;
        }
    }
}
