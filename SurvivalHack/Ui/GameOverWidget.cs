﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HackConsole;

namespace SurvivalHack.Ui
{
    class GameOverWidget : TextWidget, IKeyEventSuscriber, IPopupWidget
    {
        // Well, you can't close the Game Over widget as the game is over.
        public Action OnClose { get; set; }
        public bool Interrupt => true;

        protected override void MakeLines()
        {
            WordWrap("Game Over", "", Color.White);
        }

        public void OnKeyPress(char keyCode, EventFlags flags)
        {
            // You are dead, you can't do shit.
        }
        
        public void OnArrowPress(Vec move, EventFlags flags)
        {
            // You are dead, you can't do shit.
        }
    }
}
