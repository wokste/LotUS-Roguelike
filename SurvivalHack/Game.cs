﻿using System.Drawing;
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
                Health = new Bar(20),
                Hunger = new Bar(20),
                Position = GetSpawnPoint(),
            };

            FieldOfView = new FieldOfView(Grid)
            {
                PlayerPos = Player.Position
            };
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
            if (!Player.Alive)
                return;

            if (Player.Walk(point, Grid))
            {
                FieldOfView.PlayerPos = Player.Position;
                TimeAdvance(1);
            }
        }

        internal void PlayerMine()
        {
            if (!Player.Alive)
                return;

            Player.Mine(Grid);
            FieldOfView.OnMapUpdate();

            TimeAdvance(20);
        }

        public const int HUNGER_TICKS = 50;
        private int _ticksTillHungerLoss = HUNGER_TICKS;

        private void TimeAdvance(int ticks)
        {
            _ticksTillHungerLoss -= ticks;

            while (_ticksTillHungerLoss <= 0)
            {
                _ticksTillHungerLoss += HUNGER_TICKS;
                Player.Hunger.Current--;
                if (Player.Hunger.Current == 0)
                    Player.Alive = false;

                Player.DisplayStats();
            }
        }
    }
}
 