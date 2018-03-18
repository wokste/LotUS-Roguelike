namespace HackConsole
{
    public class InfoWidget : TextWidget
    {
        private IDescriptionProvider _item;

        public IDescriptionProvider Item {
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
                WordWrap(_item.Description, "", Color.Gray);
            }
        }
    }

    public interface IDescriptionProvider
    {
        string Name { get; }
        string Description { get; }
    }
}
