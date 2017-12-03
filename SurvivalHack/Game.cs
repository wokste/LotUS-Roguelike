using System;
using System.Data;
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

        public void Init()
        {
            ItemTypeList.InitTypes();
            TileTypeList.InitTypes();

            Grid = new TileGrid(256, 256);
            Player = new Creature
            {
                Name = "Steven",
                Attack = new Attack
                {
                    Damage = 7,
                    HitChance = 0.75f
                },
                HitPoints = new Bar(25),
                Position = GetSpawnPoint(),
            };
            FieldOfView = new FieldOfView(Grid);
            FieldOfView.PlayerPos = Player.Position;
        }

        private Point GetSpawnPoint()
        {
            int x, y;
            do
            {
                x = Dicebag.UniformInt(Grid.Width);
                y = Dicebag.UniformInt(Grid.Height);
            } while (Grid.Grid[x, y].Wall != null);
            
            return new Point(x, y);
        }

        internal void PlayerWalk(Point point)
        {
            Player.Walk(point, Grid);
            FieldOfView.PlayerPos = Player.Position;
        }

        internal void PlayerMine()
        {
            Player.Mine(Grid);
            FieldOfView.OnMapUpdate();
        }
    }
}
 