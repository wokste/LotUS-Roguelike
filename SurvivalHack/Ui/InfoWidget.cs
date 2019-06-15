using HackConsole;
using SFML.Graphics;

namespace SurvivalHack.Ui
{
    public class InfoWidget : GridWidget
    {
        private Entity _item;
        //protected readonly List<string> Lines = new List<string>();

        public Entity Item {
            get => _item;
            set {
                if (_item == value)
                    return;

                _item = value;

                Dirty = true;
            }
        }

        protected override void Render()
        {
            Clear(Color.Blue);

            var y = 0;

            if (_item != null)
            {
                Print(new Vec(0, y++), _item.Name, Color.White);
                //if (_item.EntityFlags.HasFlag(EEntityFlag.Identified))
                {
                }
            }
        }
    }
}