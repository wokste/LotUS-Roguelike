﻿using HackConsole;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SurvivalHack.Ui
{
    public class InventoryWidget : Widget, IKeyEventSuscriber, IMouseEventSuscriber, IPopupWidget
    {
        TurnController _controller;
        private BaseWindow _window;
        private int _selectedRow = 0;
        private int[] _columnWidth = new int[4];

        public Action OnClose { get ; set ; }
        public bool Interrupt => false;
        public InventoryWidget(TurnController controller, BaseWindow window)
        {
            _controller = controller;

            _columnWidth[0] = 1;
            _columnWidth[1] = Inventory.SlotNames.Max(p => p.name.Length);
            _columnWidth[2] = 1;
            _columnWidth[3] = 40;
            _window = window;

            DesiredSize = new Rect(0, 0, _columnWidth.Sum() + _columnWidth.Length - 1, Inventory.SlotNames.Length);
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
                    var (type, name, key) = Inventory.SlotNames[y];
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

            var slotType = Inventory.SlotNames[_selectedRow].type;

            var list = new List<Entity>();
            list.AddRange(_controller.Inventory.Items.Where(e => e.Get<Equippable>().Any(c => c.FitsIn(slotType))));
            list.Add(null);

            var o = new OptionWidget($"Wield {Inventory.SlotNames[_selectedRow].name}", list, i => {
                if (_controller.Inventory.Equip(i, _selectedRow))
                    _controller.EndTurn();

                Dirty = true;
            });
            _window.PopupStack.Push(o);
        }

        protected override void RenderImpl()
        {
            Clear();
            
            for (var y = 0; y < Inventory.SlotNames.Length; y++)
            {
                var (type, name, key) = Inventory.SlotNames[y];
                var item = _controller.Inventory.Equipped[y];

                var color = (y == _selectedRow) ? Color.White : Color.Gray;
                Print(new Vec(2, y), name, color);
                Print(new Vec(0, y), new Symbol(key, Color.Yellow));

                if (item != null)
                {
                    var x = _columnWidth[0] + _columnWidth[1] + 2;
                    Print(new Vec(x, y), item.Symbol);

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
            var relMousePos = mousePos - Size.TopLeft;
            _selectedRow = relMousePos.Y;
            Dirty = true;
        }

        public void OnMouseWheel(Vec delta, EventFlags flags)
        {
        }
    }
}
