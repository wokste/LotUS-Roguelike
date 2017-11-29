using System;
using System.Drawing;
using System.Threading;
using HackLib;

namespace SurvivalHack
{
    class Game
    {
        public Creature Player;
        public readonly Inventory Inventory = new Inventory();
        public TileGrid Grid;
        
        public void Run()
        {
            while (true)
            {
                //Console.Clear();
                Console.CursorVisible = false;

                var windowWidth = Console.WindowWidth;
                var windowHeight = Console.WindowHeight;

                var barLeft = windowWidth / 4;
                var barRight = windowWidth / 4;

                Grid.Render(new Rectangle(barLeft, 0, windowWidth - barLeft - barRight, windowHeight),new Point(0,0));
                Thread.Sleep(50);
                //ShowMenu();
            }
                
        }

        private void ShowMenu()
        {
            Inventory.Write();

            var menu = new ActionMenu("What do you want to do?");
            menu.Add("Cut Wood", CutWood);
            menu.Add("Mine", Mine);
            menu.Add("Farming", FarmMenu);
            menu.Add("Craft", Craft);
            menu.Show();
        }

        public void Init()
        {
            ItemTypeList.InitTypes();
            TileTypeList.InitTypes();

            Grid = new TileGrid(128, 128);
            Player = new Creature
            {
                Name = "Steven",
                Attack = new Attack
                {
                    Damage = 7,
                    HitChance = 0.75f
                },
                HitPoints = new Bar(25)
            };
        }

        private void Craft()
        {
            var station = new CraftingStation();
            station.Init();
            station.OpenCraftingMenu(Inventory);
        }
        
        private void Mine()
        {
            Inventory.Add("stone", Dicebag.UniformInt(2, 4));

            if (Dicebag.UniformInt(25) == 1)
            {
                Inventory.Add("ore", Dicebag.UniformInt(2, 4));
            }
        }

        private void FarmMenu()
        {
            Inventory.Add("food", Dicebag.UniformInt(2, 4));
        }

        private void CutWood()
        {
            Inventory.Add("wood", Dicebag.UniformInt(3, 6));
        }
    }
}
 