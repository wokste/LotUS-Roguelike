using HackConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalHack.Ui
{
    class InventoryWidget : Widget, IKeyEventSuscriber, IPopupWidget
    {
        private Entity _player;
        private Inventory _inventory;
        private BaseWindow _window;
        private int _selectedRow = 0;
        private int[] _columnWidth = new int[3];

        public Action OnClose { get ; set ; }
        public bool Interrupt => false;
        public InventoryWidget(Entity player, BaseWindow window)
        {
            _player = player;
            _inventory = player.GetOne<Inventory>();

            _columnWidth[0] = Inventory.SlotNames.Max(p => p.name.Length);
            _columnWidth[1] = 1;
            _columnWidth[2] = 20;
            _window = window;

            DesiredSize = new Rect(0, 0, _columnWidth.Sum() + 2, Inventory.SlotNames.Length);
        }

        public void OnArrowPress(Vec move, EventFlags flags)
        {
            _selectedRow += move.Y;
            if (_selectedRow < 0)
                _selectedRow = Math.Max(Inventory.SlotNames.Length - 1, 0);
            if (_selectedRow >= Inventory.SlotNames.Length)
                _selectedRow = 0;

            Dirty = true;
        }

        public void OnKeyPress(char keyCode, EventFlags flags)
        {

            if (keyCode == (char)13) // Enter
            {
                ShowEquipMenu(_selectedRow);
            }
            else if (keyCode == (char)27) // Escape
            {
                OnClose?.Invoke();
            }
            else
            {
                var keyCodeStr = keyCode.ToString();
                for (var y = 0; y < Inventory.SlotNames.Length; y++)
                {
                    var (type, name) = Inventory.SlotNames[y];
                    if (name.StartsWith(keyCodeStr, StringComparison.InvariantCultureIgnoreCase))
                    {
                        ShowEquipMenu(y);
                        return;
                    }
                }
            }
        }

        private void ShowEquipMenu(int index)
        {
            var o = new OptionWidget<Entity>
            {
                DesiredSize = new Rect(new Vec(), new Vec(25, 25)),
                OnSelect = i =>
                {
                    _inventory.Equip(_player, i, index);
                    // TODO: Nextturn

                    Dirty = true;
                },
                Question = "Wield item",
                Set = _inventory._items
            };
            _window.PopupStack.Push(o);
        }

        protected override void RenderImpl()
        {
            Clear();
            
            for (var y = 0; y < Inventory.SlotNames.Length; y++)
            {
                var (type, name) = Inventory.SlotNames[y];
                var item = _inventory.Equipped[y];

                var color = (y == _selectedRow) ? Color.White : Color.Gray;
                Print(new Vec(0, y), name, color);
                Print(new Vec(0, y), new Symbol(name[0], Color.Yellow));

                if (item != null)
                {
                    var x = _columnWidth[0] + 1;
                    Print(new Vec(x, y), item.Symbol);

                    x += 2;
                    Print(new Vec(x, y), item.Name, Color.White); // TODO: What if the length is too long
                }
            }
            Dirty = false;
        }
    }
}
