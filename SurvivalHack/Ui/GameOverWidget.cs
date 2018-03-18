using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HackConsole;

namespace SurvivalHack.Ui
{
    class GameOverWidget : TextWidget
    {
        protected override void MakeLines()
        {
            WordWrap("Game Over", "", Color.White);
        }
    }
}
