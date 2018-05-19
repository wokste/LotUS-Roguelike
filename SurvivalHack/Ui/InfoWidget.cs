using HackConsole;

namespace SurvivalHack.Ui
{
    public class InfoWidget : TextWidget
    {
        private Entity _item;

        public Entity Item {
            get => _item;
            set {
                if (_item == value)
                    return;

                _item = value;
                
                MakeLines();
                
                Dirty = true;
            }
        }

        protected override void MakeLines()
        {
            Lines.Clear();
            if (_item != null)
            {
                WordWrap(_item.Name, "", Color.White);
                //if (_item.EntityFlags.HasFlag(EEntityFlag.Identified))
                {
                    foreach (var c in _item.Components)
                    {
                        var s = c.Describe();
                        if (s != null)
                            WordWrap(s, " -", Color.Gray);
                    }
                }
            }
        }
    }
}
