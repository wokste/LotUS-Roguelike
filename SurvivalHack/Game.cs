using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalHack
{
    class Game
    {
        private Creature _player;

        public Game(Creature player)
        {
            _player = player;
        }

        public void Run()
        {
            while (true)
                ShowMenu();
        }

        private void ShowMenu()
        {
            var menu = new OptionMenu("What do you want to do?");
            menu.Add("Cut Wood", CutWood);
            menu.Add("Farm", Farm);
            menu.Add("Craft", Craft);
            menu.Show();
        }
        
        private void Craft()
        {
            Console.WriteLine("You craft an item");
        }

        private void Farm()
        {
            Console.WriteLine("You farm some food");
        }

        private void CutWood()
        {
            Console.WriteLine("You cut some wood");
        }
    }
}
