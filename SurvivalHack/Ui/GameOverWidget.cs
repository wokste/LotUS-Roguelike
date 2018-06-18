using System;
using HackConsole;

namespace SurvivalHack.Ui
{
    internal class GameOverWidget : Widget
    {
        // Well, you can't close the Game Over widget as the game is over.
        public Action OnClose { get; set; }
        public bool Interrupt => true;

        protected override void RenderImpl()
        {
            Print(new Vec(0,0), "Game Over", Color.White);
        }
    }
}
