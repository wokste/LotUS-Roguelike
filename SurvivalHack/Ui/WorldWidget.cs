using System;
using System.Diagnostics;
using HackConsole;

namespace SurvivalHack.Ui
{
    class WorldWidget : Widget, IMouseEventSuscriber, IKeyEventSuscriber
    {
        private readonly World _world;
        private readonly FieldOfView _view;
        private readonly Player _player;

        private Vec _offset;

        public WorldWidget(World world, FieldOfView view, Player following)
        {
            _world = world;
            _view = view;
            _player = following;
        }

        public override void Render(bool forceUpdate)
        {
            _offset = _player.Position - Size.Center;
            
            Clear();
            RenderGrid();
            RenderCreatures();
        }

        public Action<IDescriptionProvider> OnSelected;
        public Action<int> OnSpendTime;

        private void RenderCreatures()
        {
            foreach (var creature in _world.Creatures) {
                var x = creature.Position.X;
                var y = creature.Position.Y;
                
                if (_view.Visibility[x,y] < 128)
                    continue;

                x -= _offset.X;
                y -= _offset.Y;

                if (!Size.Contains(x,y))
                    continue;

                CellGrid.Cells[x, y] = creature.Symbol;
            }
        }

        private void RenderGrid()
        {
            var x0 = Math.Max(Size.Left, 0 - _offset.X);
            var y0 = Math.Max(Size.Top, 0 - _offset.Y);
            var x1 = Math.Min(Size.Right, _world.Map.Width - _offset.X);
            var y1 = Math.Min(Size.Bottom, _world.Map.Height - _offset.Y);

            for (var y = y0; y < y1; y++)
            {
                if (!_world.InBoundary(0, y + _offset.Y))
                    continue;

                for (var x = x0; x < x1; x++)
                {
                    if (!_world.InBoundary(x + _offset.X, y + _offset.Y))
                        continue;

                    var visibility = _view.Visibility[x + _offset.X, y + _offset.Y];

                    if (visibility == 0)
                        continue;

                    CellGrid.Cells[x, y] = _world.GetTop(x + _offset.X, y + _offset.Y).Char;
                    if (visibility < 255)
                    {
                        CellGrid.Cells[x, y].TextColor.Darken(visibility);
                        CellGrid.Cells[x, y].BackgroundColor.Darken(visibility);
                    }
                }
            }
        }

        public void OnKeyPress(char keyCode, EventFlags flags)
        {
            switch (keyCode)
            {
                case 'e':
                    if (!_player.Alive)
                        throw new Exception("WUT. I am dead");

                    if (_player.Eat())
                        OnSpendTime.Invoke(1000);
                    // TODO: Change this in a mechanic that the player can choose what to eat
                    break;
            }
        }

        public void OnArrowPress(Vec move, EventFlags flags)
        {
            if (!_player.Alive)
            {
                throw new Exception("WUT. I am dead");
            }

            var actPoint = _player.Position + move;
            foreach (var c in _world.Creatures.ToArray())
            {
                if (c.Position == actPoint && c != _player)
                {
                    _player.Attack.Attack(_player, c);
                    OnSpendTime.Invoke(1000);
                }
            }

            if (_player.Walk(move))
            {
                OnSpendTime.Invoke((int)(800 * move.Length));
            }
        }

        public void OnMouseEvent(Vec mousePos, EventFlags flags)
        {
            // Select creatures, etc
            if (flags.HasFlag(EventFlags.LeftButton | EventFlags.MouseEventPress))
            {
                var absPos = mousePos + _offset;
                if (!_world.InBoundary(absPos.X, absPos.Y) || _player.FoV.Visibility[absPos.X, absPos.Y] == 0)
                {
                    OnSelected?.Invoke(null);
                }

                var c = _world.GetCreature(absPos.X, absPos.Y);
                OnSelected?.Invoke(c);
            }
        }

        public void OnMouseMove(Vec mousePos, Vec mouseMove, EventFlags flags)
        {
        }

        public void OnMouseWheel(Vec delta, EventFlags flags)
        {
        }
    }
}
