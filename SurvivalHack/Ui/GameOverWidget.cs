using System;
using HackConsole;
using SFML.Graphics;

namespace SurvivalHack.Ui
{
    public class GameOverWidget : GridWidget, IPopupWidget
    {
        // Well, you can't close the Game Over widget as the game is over.
        public Action OnClose { get; set; }
        public bool Interrupt => true;

        protected override void Render()
        {
            Clear(Color.Black);
            Print(new Vec(0,0), "Game Over", Color.White);
        }
    }
}
