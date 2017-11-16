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
        private readonly Inventory _inventory = new Inventory();

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
            _inventory.Write();

            var menu = new ActionMenu("What do you want to do?");
            menu.Add("Cut Wood", CutWood);
            menu.Add("Farming", FarmMenu);
            menu.Add("Craft", Craft);
            menu.Show();
        }

        internal void Init()
        {
            ItemTypeList.InitTypes();

        }

        private void Craft()
        {
            /*
            if (wood < 10)
            {
                Console.WriteLine("Can't craft this item. You have too few components.");
                return;
            }

            wood -= 10;
            Console.WriteLine("You craft an item");
            */
        }

        private void FarmMenu()
        {
            var t = ItemTypeList.Get("food");
            var i = t.Make(5);

            _inventory.Add(i);
        }

        private void CutWood()
        {
            var t = ItemTypeList.Get("wood");
            var i = t.Make(5);

            _inventory.Add(i);
        }
    }
}
