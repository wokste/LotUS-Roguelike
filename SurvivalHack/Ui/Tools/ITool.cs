using HackConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalHack.Ui.Tools
{
    public interface ITool
    {
        void Apply(Vec dest);
    }
}
