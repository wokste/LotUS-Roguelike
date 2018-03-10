using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                
                _dirty = true;
            }
        }

        protected override void MakeLines()
        {
            _lines.Clear();
            if (_item != null)
            {
                WordWrap(_item.Name, "");
                WordWrap(_item.Description, "");
            }
        }
    }

    public interface IDescriptionProvider
    {
        string Name { get; }
        string Description { get; }
    }
}
