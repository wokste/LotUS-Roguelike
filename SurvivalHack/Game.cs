using System;
using System.Drawing;
using System.Threading;
using HackLib;

namespace SurvivalHack
{
    class Game
    {
        public Creature Player;
        public TileGrid Grid;
        public FieldOfView FieldOfView;

        private void ShowMenu()
        {
            Player.Inventory.Write();

            var menu = new ActionMenu("What do you want to do?");
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
            FieldOfView = new FieldOfView(Grid);
        }

        private void Craft()
        {
            var station = new CraftingStation();
            station.Init();
            station.OpenCraftingMenu(Player.Inventory);
        }

        internal void PlayerWalk(Point point)
        {
            Player.Walk(point, Grid);
            FieldOfView.PlayerPos = Player.Position;
        }
    }
}
 