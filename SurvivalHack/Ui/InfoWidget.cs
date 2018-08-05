using HackConsole;
using HackConsole.Algo;

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

        protected override void RenderImpl()
        {
            Clear();

            var y = 0;

            if (_item != null)
            {
                Print(new Vec(0, y++), _item.Name, Color.White);
                //if (_item.EntityFlags.HasFlag(EEntityFlag.Identified))
                {
                    foreach (var c in _item.Components)
                    {
                        var s = c.Describe();
                        if (s != null)
                            foreach (var l in StringExt.Prefix(StringExt.Wrap(s, Rect.Width - 2), " -"))
                                Print(new Vec(0, y++), l, Color.White);
                    }
                }
            }
        }
    }
}