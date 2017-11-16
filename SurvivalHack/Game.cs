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
            menu.Add("Mine", Mine);
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
            CraftingStation station = new CraftingStation();
            station.Init();
            station.OpenCraftingMenu(_inventory);
        }
        
        private void Mine()
        {
            _inventory.Add("stone", Program.Rnd.Next(2, 4));

            if (Program.Rnd.Next(25) == 1)
            {
                _inventory.Add("ore", Program.Rnd.Next(2, 4));
            }
        }

        private void FarmMenu()
        {
            _inventory.Add("food", Program.Rnd.Next(2, 4));
        }

        private void CutWood()
        {
            _inventory.Add("wood", Program.Rnd.Next(3, 6));
        }
    }
}
