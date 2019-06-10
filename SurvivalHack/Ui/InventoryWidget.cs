using HackConsole;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SurvivalHack.Ui
{
    public class InventoryWidget : GridWidget, IKeyEventSuscriber, IMouseEventSuscriber, IPopupWidget
    {
        readonly TurnController _controller;
        private readonly BaseWindow _window;
        private int _selectedRow = 0;
        private readonly int[] _columnCharWidth = new int[4];
        private readonly int[] _columnPxWidth = new int[4];

        public Action OnClose { get; set; }
        public bool Interrupt => false;
        private const int ROW_HEIGHT = 16;
        private const int HEADER_HEIGHT = 16;
        private const int BORDER_WIDTH = 16;

        public InventoryWidget(TurnController controller, BaseWindow window)
        {
            _controller = controller;

            _columnCharWidth[0] = 1;
            _columnCharWidth[1] = Inventory.SlotNames.Max(p => p.name.Length);
            _columnCharWidth[2] = 1;
            _columnCharWidth[3] = 40;
            _window = window;

            for (int i = 0; i < 4; ++i)
            {
                _columnPxWidth[i] = _columnCharWidth[i] * _fontX;
            }

            DesiredSize = new Rect(0, 0, _columnPxWidth.Sum() + (_columnPxWidth.Length - 1) * BORDER_WIDTH, Inventory.SlotNames.Length * ROW_HEIGHT + HEADER_HEIGHT);
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
                ShowEquipMenu();
            }
            else if (keyCode == (char)27) // Escape
            {
                OnClose?.Invoke();
            }
            else
            {
                for (var y = 0; y < Inventory.SlotNames.Length; y++)
                {
                    var (_, _, key) = Inventory.SlotNames[y];
                    if (key == keyCode)
                    {
                        _selectedRow = y;
                        ShowEquipMenu();
                        return;
                    }
                }
            }
        }

        private void ShowEquipMenu()
        {
            if (_selectedRow < 0 || _selectedRow >= Inventory.SlotNames.Length)
                return;

            if (_controller.Inventory.Slots[_selectedRow].Cursed)
                return; // You can't change items in a cursed slot. Period.

            _controller.Inventory.Slots[_selectedRow].NewItems = false;

            var list = new List<Entity>();
            list.AddRange(_controller.Inventory.Items.Where(e => Inventory.CanEquipInSlot(_selectedRow, e)));
            list.Add(null);

            var o = new OptionWidget($"Wield {Inventory.SlotNames[_selectedRow].name}", list, i => {
                if (_controller.Inventory.Equip(i, _selectedRow))
                    _controller.EndTurn();

                Dirty = true;
            });
            _window.PopupStack.Push(o);
        }

        protected override void Render()
        {
            Clear(Color.Black);
            Print(new Vec(0, 0), "  Slot             Item", Color.White);

            var inv = _controller.Inventory;
            for (int i = 0; i < Inventory.SlotNames.Length; ++i)
            {
                var (_, name, key) = Inventory.SlotNames[i];
                var slot = inv.Slots[i];

                var color = (i == _selectedRow) ? Color.White : new Color(128,128,128);
                var bgColor = slot.GetBackgroundColor();

                var y = i + 1;

                Print(new Vec(0, y), new Symbol(key, Color.Yellow));
                Print(new Vec(2, y), name, color, bgColor);

                if (slot.Item is Entity item)
                {
                    var x = _columnCharWidth[0] + _columnCharWidth[1] + 2;
                    Print(new Vec(x, y), new Symbol(item.Name[0], Color.White));

                    x += 2;

                    Print(new Vec(x, y), item.Name, Color.White); // TODO: What if the length is too long
                }
            }
        }

        public void OnMouseEvent(Vec mousePos, EventFlags flags)
        {
            if (flags.HasFlag(EventFlags.LeftButton) && flags.HasFlag(EventFlags.MouseEventRelease))
                ShowEquipMenu();
        }

        public void OnMouseMove(Vec mousePos, Vec mouseMove, EventFlags flags)
        {
            _selectedRow = (mousePos.Y - HEADER_HEIGHT) / ROW_HEIGHT;
            Dirty = true;
        }

        public void OnMouseWheel(Vec delta, EventFlags flags)
        {
        }
    }
}
